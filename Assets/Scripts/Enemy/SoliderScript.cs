using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
/// <summary>
/// 2D 톱다운 병사 AI (Stay → Track → Search ↔ Fire)
/// </summary>
public class SoliderScript : NPCBase
{
    /* ───────────────────── 1. 컴포넌트 & 기본 필드 ───────────────────── */
    [Header("Unity Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private Transform armTransform;
    [SerializeField] private Animator armAnimator;
    [SerializeField] private Collider2D attackCollider;

    private float prevFacing = 1f;               // 직전 좌우 방향  (1:오른쪽, -1:왼쪽)
    private Vector3 prevAim = Vector3.zero;     // 직전 팔 회전 기준 벡터

    private DamageableEntity currentTarget;
    private float currentDistance = Mathf.Infinity;
    private float lastFindTargetTime;
    private const float detectionTimeout = 20f;

    [Header("Combat Settings")]
    private bool isFiring;                       // 🔑 FireTask 실행 중 여부
    public int attackPreDelayTime = 500;
    public int attackPostDelayTime = 1000;
    public int closeFire = 6;
    public int farFire = 3;
    public int reloadCount = 15;
    public const int reloadMax = 15;

    [Header("AI Random Weights")]
    public int rushRate = 2;
    public int stopRate = 1;
    public int aroundRate = 4;

    private bool isContact = false;

    /* ───────────────────── 2. UniTask 핸들러 & CTS ───────────────────── */
    private UniTask currentStateTask = UniTask.CompletedTask; // Stay / Track
    private UniTask currentSearchTask = UniTask.CompletedTask; // Search 루프
    private UniTask currentMoveTask = UniTask.CompletedTask; // Rush / Stop / Around
    private UniTask currentFireTask = UniTask.CompletedTask; // Fire 루틴

    private CancellationTokenSource lifeCTS;    // Enable~Disable/Destroy 전역
    private CancellationTokenSource stateCTS;   // Stay / Track
    private CancellationTokenSource searchCTS;  // Search
    private CancellationTokenSource moveCTS;    // Rush / Stop / Around
    private CancellationTokenSource fireCTS;    // Fire

    private float lastUpdate = 0f;

    /* ───────────────────── 3. Unity 라이프사이클 ───────────────────── */
    public override void Awake()
    {
        base.Awake();

        faction = npcData.faction;
        agent.speed = npcData.speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = 50;
    }


    private void FixedUpdate() => UpdateBodyAnimator();


    private void UpdateBodyAnimator()
    {
        bool isWalking = agent.velocity.sqrMagnitude > 0.1f;
        bodyAnimator.SetBool("isWalk", isWalking);
    }

    private void Update()
    {
        if (agent.enabled == false) { return; }
        if (isContact && currentTarget!=null) { 
            if (Time.time - lastUpdate > 0.05f) //성능 부하 테스트 후 조준 부분을 사격태스크로 이관
            {
                UpdateFacingAndAim(currentTarget.transform.position);
                lastUpdate = Time.time;
            }
        }
    }
    private void OnEnable()
    {
        InitAllCTS();

        StartStayTask();    // 기본 상태
        StartSearchTask();  // 탐색 루프 (상시)
    }

    private void OnDisable() => CancelAllTasks();
    private void OnDestroy() => CancelAllTasks();

    private void InitAllCTS()
    {
        lifeCTS = new CancellationTokenSource();
        stateCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);
        searchCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);
        moveCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);
        fireCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);
    }

    private void CancelAndDispose(ref CancellationTokenSource cts)
    {
        if (cts == null) return;
        if (!cts.IsCancellationRequested) cts.Cancel();
        cts.Dispose();
        cts = null;
    }

    private void CancelAllTasks()
    {
        CancelAndDispose(ref lifeCTS);
        CancelAndDispose(ref stateCTS);
        CancelAndDispose(ref searchCTS);
        CancelAndDispose(ref moveCTS);
        CancelAndDispose(ref fireCTS);

        currentStateTask = UniTask.CompletedTask;
        currentSearchTask = UniTask.CompletedTask;
        currentMoveTask = UniTask.CompletedTask;
        currentFireTask = UniTask.CompletedTask;
    }

    /* ───────────────────── 4. 상태 루틴 시작 / 전환 ───────────────────── */
    private void StartStayTask()
    {
        isContact = false;
        CancelAndDispose(ref stateCTS);
        stateCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);

        alertState = AlertState.Stay;
        currentTarget = null;
        currentStateTask = StayRoutine(stateCTS.Token);
    }

    private void StartTrackTask()
    {
        isContact = true;
        CancelAndDispose(ref stateCTS);
        stateCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);

        alertState = AlertState.Track;
        currentStateTask = TrackRoutine(stateCTS.Token);
    }

    private void StartSearchTask()
    {
        CancelAndDispose(ref searchCTS);
        searchCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);

        currentSearchTask = SearchLoop(searchCTS.Token);
    }

    private void StartMoveTask(System.Func<CancellationToken, UniTask> routineStarter)
    {
        CancelAndDispose(ref moveCTS);
        moveCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);

        currentMoveTask = routineStarter(moveCTS.Token);
    }

    private void StartFireTask()
    {
        if (isFiring) return;      // 중복 방지
        isFiring = true;

        CancelAndDispose(ref fireCTS);
        fireCTS = CancellationTokenSource.CreateLinkedTokenSource(lifeCTS.Token);

        currentFireTask = FireRoutine(fireCTS.Token);
    }

    /* ───────────────────── 5. UniTask 루틴 ───────────────────── */
    private async UniTask StayRoutine(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (UnityEngine.Random.Range(0, 4) == 0)
            {
                MoveToRandomPosition(transform, npcData.fireRange / 2);
                ResetArmRotation();
                FlipByAgentVelocity();
            }

            await UniTask.Delay(2000, cancellationToken: token);
        }
    }

    private async UniTask TrackRoutine(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            ChooseRandomMoveAction();
            await UniTask.Delay(2000, cancellationToken: token);
        }
    }

    private async UniTask SearchLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!isFiring)   // 🔑 Fire 중이 아닐 때만 탐색
            {
                float range = (alertState == AlertState.Stay)
                             ? npcData.detectionRange / 2f
                             : npcData.detectionRange;

                FindClosestTarget(range);
            }

            await UniTask.Delay(1500, cancellationToken: token);
        }
    }

    private async UniTask FireRoutine(CancellationToken token)
    {
        Debug.Log("🔥 FireRoutine 시작");

        try
        {
            await UniTask.Delay(attackPreDelayTime, cancellationToken: token);

            if (currentTarget == null) return;
            
            //for문 내 횟수, Delay 내 횟수를 변수로 치환 
            if (currentDistance < npcData.fireRange / 2)
            {
                reloadCount -= closeFire;
                for (int i = 0; i < closeFire; i++) {
                    await UniTask.Delay(200, cancellationToken: token);
                    FireProjectile();
                }
            }
            else
            {
                reloadCount -= farFire;
                for (int i = 0; i < farFire; i++)
                {
                    await UniTask.Delay(300, cancellationToken: token);
                    FireProjectile();
                }
            }

            if (reloadCount <= 0)
            {
                await UniTask.Delay(6000, cancellationToken: token); // 재장전
                reloadCount = reloadMax;
            }

            await UniTask.Delay(attackPostDelayTime, cancellationToken: token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            isFiring = false;      // 🔑 Fire 종료
            Debug.Log("✅ FireRoutine 종료, isFiring = false");
        }
    }

    /* ───────────────────── 6. 이동 루틴 (Rush/Stop/Around) ───────────────────── */
    private void ChooseRandomMoveAction()
    {
        int total = rushRate + stopRate + aroundRate;
        int rand = UnityEngine.Random.Range(0, total);

        if (rand < rushRate) StartMoveTask(RushRoutine);
        else if (rand < rushRate + stopRate) StartMoveTask(StopRoutine);
        else StartMoveTask(AroundRoutine);
    }

    private async UniTask RushRoutine(CancellationToken token)
    {
        if (currentTarget && agent.isOnNavMesh)
            agent.SetDestination(currentTarget.transform.position);

        await UniTask.Delay(1000, cancellationToken: token);
    }

    private async UniTask StopRoutine(CancellationToken token)
    {
        agent.isStopped = true;
        await UniTask.Delay(1000, cancellationToken: token);
        agent.isStopped = false;
    }

    private async UniTask AroundRoutine(CancellationToken token)
    {
        MoveToRandomPosition(transform, npcData.fireRange);
        await UniTask.Delay(1000, cancellationToken: token);
    }

    /* ───────────────────── 7. 타겟 탐색 / 사격 보조 ───────────────────── */
    private DamageableEntity FindClosestTarget(float detectionRange)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position,
                                                       detectionRange,
                                                       LayerMask.GetMask("DamageableEntity"));

        DamageableEntity closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var col in cols)
        {
            if (!col) continue;
            var dmg = col.GetComponent<DamageableEntity>();
            if (!dmg || dmg.gameObject == gameObject) continue;
            if (dmg.faction == faction || dmg.faction == Faction.Wall) continue;

            float dist = Vector2.Distance(transform.position, dmg.transform.position);
            if (dist < closestDist)
            {
                closest = dmg;
                closestDist = dist;
            }
        }

        if (closest)
        {
            if (alertState != AlertState.Track) StartTrackTask();

            lastFindTargetTime = Time.time;
            currentTarget = closest;
            currentDistance = closestDist;
            if (currentDistance <= npcData.fireRange)
            {
                // 🔍 타겟과의 직선 경로에 Wall이 없는 경우만 발사
                Vector2 from = transform.position;
                Vector2 to = currentTarget.transform.position;

                var hit = Physics2D.Linecast(from, to, LayerMask.GetMask("Wall"));
                if (!hit)  // 막히지 않았을 경우에만 FireTask 실행
                {
                    StartFireTask();
                }
            }

        }
        else if (Time.time - lastFindTargetTime > detectionTimeout)
        {
            StartStayTask();
        }

        return closest;
    }

    private void MoveToRandomPosition(Transform reference, float range)
    {
        if (!reference) return;
        Vector3 pos = GetValidRandomPosition(reference.position, range, 5);
        if (pos != Vector3.zero && agent.isOnNavMesh)
            agent.SetDestination(pos);
    }

    private Vector3 GetValidRandomPosition(Vector3 origin, float range, int attempts)
    {
        for (int i = 0; i < attempts; i++)
        {
            Vector3 offset = new(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range), 0);
            Vector3 pos = origin + offset;
            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero;
    }

    private void FireProjectile()
    {
        if (!currentTarget) return;

        var pool = ProjectilePoolManager.Instance;

        Vector3 targetPos = currentTarget.transform.position;
        Vector2 dir = (targetPos - transform.position).normalized;
        Vector3 firePos = transform.position + (Vector3)(dir * 0.5f);

        float angOff = UnityEngine.Random.Range(-1f, 1f) * Mathf.Deg2Rad;
        Vector2 finalDir = new(
            dir.x * Mathf.Cos(angOff) - dir.y * Mathf.Sin(angOff),
            dir.x * Mathf.Sin(angOff) + dir.y * Mathf.Cos(angOff));

        GameObject proj = pool.GetProjectile(
            npcData.faction, 2f, 16f, npcData.damage, npcData.penetration, 0.05f);

        if (!proj) return;

        proj.transform.position = firePos;
        proj.transform.rotation = Quaternion.Euler(0, 0,
                                 Mathf.Atan2(finalDir.y, finalDir.x) * Mathf.Rad2Deg);
        proj.GetComponent<Projectile>().Launch(finalDir);
        AudioManager.Instance.PlaySFX(npcData.attackAudio);
    }

    /* ───────────────────── 8. 사망 처리 ───────────────────── */
    public override void Die()
    {
        base.Die();
        bodyAnimator.SetBool("isDie", true);
        if (agent)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        CancelAllTasks();
    }

    /*--비주얼처리--*/


    private void UpdateFacingAndAim(Vector3 targetPos)
    {
        /* 1) 좌우 Flip -------------------------------------------------- */
        float sign = Mathf.Sign(targetPos.x - transform.position.x); // 왼쪽:-1, 오른쪽:+1
        if (sign != prevFacing)
        {
            prevFacing = sign;
            // 스프라이트가 오른쪽이 기본이면 (sign,1,1), 왼쪽이 기본이면 (-sign,1,1)
            transform.localScale = new Vector3(sign, 1, 1);
        }

        /* 2) 팔 회전 ---------------------------------------------------- */
        Vector3 dir = (targetPos - armTransform.position);
        if ((dir - prevAim).sqrMagnitude > 0.0001f)     // 변화가 있을 때만 계산
        {
            prevAim = dir;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (sign < 0) angle -= 180f;                // 왼쪽일 땐 뒤집기 보정
            armTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    //팔 초기화
    private void ResetArmRotation()
    {
        armTransform.localRotation = Quaternion.identity;  // (0,0,0)
        prevAim = Vector3.zero;                            // 캐시도 초기화
    }
    //Navmesh 이동방향 기준 방향 초기화
    private void FlipByAgentVelocity(float threshold = 0.05f)
    {
        if (!agent || !agent.isOnNavMesh) return;

        float vx = agent.velocity.x;
        if (Mathf.Abs(vx) < threshold) return;             // 거의 정지면 무시

        float sign = Mathf.Sign(vx);                       // 왼쪽:-1, 오른쪽:+1
        if (sign != prevFacing)
        {
            prevFacing = sign;
            transform.localScale = new Vector3(sign, 1, 1);
        }
    }
}
