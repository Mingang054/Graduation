    using UnityEngine;
    using UnityEngine.AI;
    using Cysharp.Threading.Tasks;
    using System.Threading;

    public class ZombieScript : NPCBase
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator bodyAnimator;
        [SerializeField] private Animator armAnimator;
        [SerializeField] private Collider2D attackCollider;

        private DamageableEntity currentTarget;

        // ──────────  딜레이/타임아웃  ──────────
        private readonly int attackPreDelayTime = 500;
        private readonly int attackPostDelayTime = 1000;
        private float lastFindTargetTime;
        private readonly float detectionTimeout = 5f;

        // ──────────  태스크 & 토큰  ──────────
        private UniTask currentStateTask = UniTask.CompletedTask;
        private UniTask searchTask = UniTask.CompletedTask;

        private CancellationTokenSource stateCTS = new CancellationTokenSource();
        private CancellationTokenSource searchCTS = new CancellationTokenSource();

        // ──────────  Unity Life‑cycle  ──────────
        public override void Awake()
        {
            base.Awake();
            agent.speed = npcData.speed;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.avoidancePriority = 50;
            agent.isStopped = false;
        }

        private void Start()
        {
            StartStayTask();
            StartSearchTask();
        }

        public override void Die()
        {
            base.Die();
            agent.isStopped = true;
            agent.enabled = false;
            //  팔 애니메이션 정지
            armAnimator.SetBool("isAttack", false); // ✅ 공격 애니메이션 강제 종료

            stateCTS.Cancel();
            searchCTS.Cancel();
        }

        private void OnDestroy()
        {
            stateCTS.Cancel(); stateCTS.Dispose();
            searchCTS.Cancel(); searchCTS.Dispose();
        }

    private void FixedUpdate()
    {
        UpdateBodyAnimator();
        FlipByAgentVelocity(); // ← 방향 처리 추가
    }



    private float prevFacing = 1f;  // 1: 오른쪽, -1: 왼쪽

    private void FlipByAgentVelocity(float threshold = 0.05f)
    {
        if (!agent || !agent.isOnNavMesh) return;

        float vx = agent.velocity.x;
        if (Mathf.Abs(vx) < threshold) return;  // 거의 정지 상태이면 무시

        float sign = Mathf.Sign(vx);  // 왼쪽: -1, 오른쪽: +1
        if (sign != prevFacing)
        {
            prevFacing = sign;
            transform.localScale = new Vector3(sign, 1, 1);
        }
    }

    // ──────────  상태 루틴  ──────────
    private async UniTask StayRoutine(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(1000, cancellationToken: token);
            }
        }

        private async UniTask TrackRoutine(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (currentTarget != null && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(currentTarget.transform.position);

                    // 공격 범위 체크
                    if (IsTargetInsideAttackBox())
                    {
                        StartAttackTask();
                        return;           // ⭐ TrackRoutine 종료 → 중복 루프 방지
                    }

                    if (Time.time - lastFindTargetTime > detectionTimeout)
                    {
                        // SearchTask가 Stay 상태로 돌려줄 것
                    }
                }
                await UniTask.Delay(200, cancellationToken: token);
            }
        }

        private async UniTask AttackRoutine(CancellationToken token)
        {
            armAnimator.SetBool("isAttack", true);
            await UniTask.Delay(attackPreDelayTime, cancellationToken: token);

            ExecuteAttack();

            armAnimator.SetBool("isAttack", false);
            await UniTask.Delay(attackPostDelayTime, cancellationToken: token);

            StartTrackTask();          // 공격 종료 후 추적 재개
        }

        // ──────────  SearchTask  ──────────
        private async UniTask SearchTask(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                float range = (alertState == AlertState.Stay) ? npcData.detectionRange * 0.5f
                                                              : npcData.detectionRange;

                DamageableEntity found = FindClosestTarget(range);

                if (found != null) currentTarget = found;

                await UniTask.Delay(1000, cancellationToken: token);
            }
        }

        // ──────────  태스크 스타터  ──────────
        private void StartSearchTask()
        {
            searchCTS.Cancel();                 // 이전 태스크 중단
            searchCTS = new CancellationTokenSource();
            searchTask = SearchTask(searchCTS.Token);
        }

        private void StartStayTask()
        {
            stateCTS.Cancel();
            stateCTS = new CancellationTokenSource();

            alertState = AlertState.Stay;
            currentTarget = null;
            currentStateTask = StayRoutine(stateCTS.Token);
        }

        private void StartTrackTask()
        {
            stateCTS.Cancel();
            stateCTS = new CancellationTokenSource();

            alertState = AlertState.Track;
            currentStateTask = TrackRoutine(stateCTS.Token);
        }

        private void StartAttackTask()
        {
            stateCTS.Cancel();
            stateCTS = new CancellationTokenSource();

            alertState = AlertState.Attack;
            currentStateTask = AttackRoutine(stateCTS.Token);
        }

        // ──────────  로직 헬퍼  ──────────
        private bool IsTargetInsideAttackBox()
        {
            var hits = Physics2D.OverlapBoxAll(attackCollider.bounds.center,
                                               attackCollider.bounds.size,
                                               0f,
                                               LayerMask.GetMask("DamageableEntity"));
            foreach (var col in hits)
                if (col.gameObject == currentTarget?.gameObject) return true;
            return false;
        }

        private void ExecuteAttack()
        {
            var hits = Physics2D.OverlapBoxAll(attackCollider.bounds.center,
                                               attackCollider.bounds.size,
                                               0f,
                                               LayerMask.GetMask("DamageableEntity"));
            foreach (var col in hits)
            {
                var dmg = col.GetComponent<DamageableEntity>();
                if (dmg != null && dmg == currentTarget)
                    dmg.OnHitDamage(npcData.damage, npcData.penetration, col.transform.position, Vector2.zero, faction);
            }
            AudioManager.Instance.PlaySFX(npcData.attackAudio);
        }

        private DamageableEntity FindClosestTarget(float range)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, range,
                                                       LayerMask.GetMask("DamageableEntity"));

            DamageableEntity closest = null;
            float minDist = float.PositiveInfinity;

            foreach (var col in colliders)
            {
                var dmg = col.GetComponent<DamageableEntity>();
                if (dmg == null || dmg.gameObject == gameObject) continue;
                if (dmg.faction == faction || dmg.faction == Faction.Wall) continue;

                float dist = Vector2.Distance(transform.position, dmg.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = dmg;
                }
            }

            if (closest != null)
            {
                lastFindTargetTime = Time.time;
                if (alertState != AlertState.Track) StartTrackTask();
            }
            else if (Time.time - lastFindTargetTime > detectionTimeout)
            {
                StartStayTask();
            }

            return closest;
        }

        private void UpdateBodyAnimator()
        {
            bool isWalking = agent.velocity.sqrMagnitude > 0.1f;
            bodyAnimator.SetBool("isWalk", isWalking);
        }
    }
