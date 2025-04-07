using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

public class ZombieScript : NPCBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject effectAnimator; // 공격 이펙트 프리팹
    [SerializeField] private GameObject dieAnimator;    // 사망 애니메이션 프리팹
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private Animator armAnimator;
    [SerializeField] public Collider2D attackCollider;

    private Transform target;
    private DamageableEntity currentTarget;

    private UniTask currentStateTask = UniTask.CompletedTask;
    private UniTask searchTask = UniTask.CompletedTask;

    private string currentRoutineName = string.Empty;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private float lastFindTargetTime = 0f;
    private float detectionTimeout = 5f;

    private int attackPreDelayTime = 500;
    private int attackPostDelayTime = 1000;

    private bool isAttacking = false;

    public override void Awake()
    {
        base.Awake();
        agent.speed = npcData.speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = 50;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    public void Start()
    {
        if (searchTask.Status == UniTaskStatus.Pending)
        {
            Debug.LogWarning("[ZombieScript] 기존 SearchTask가 이미 실행 중입니다.");
            return;
        }

        searchTask = SearchTask();
        StartStayTask();
    }

    public void FixedUpdate()
    {
        UpdateBodyAnimator();
        UpdateFacingDirection(); // ✅ 방향 회전
    }

    /// <summary>
    /// 대기 상태 루틴
    /// </summary>
    private async UniTask StayRoutine()
    {
        await UniTask.Delay(1000);
    }

    /// <summary>
    /// 타겟을 추적하는 루틴
    /// </summary>
    private async UniTask TrackRoutine()
    {
        var token = cancellationTokenSource.Token;

        while (!isDead && !token.IsCancellationRequested)
        {
            if (currentTarget != null && agent.isOnNavMesh)
            {
                float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);

                if (distanceToTarget <= npcData.fireRange && !isAttacking)
                {
                    StartAttackTask();
                }

                if (Time.time - lastFindTargetTime > detectionTimeout)
                {
                    Debug.LogWarning("[ZombieScript] 타겟을 놓쳤거나 감지 시간이 초과되었습니다.");
                }
            }
            else
            {
                Debug.LogWarning("[ZombieScript] currentTarget이 없거나 NavMesh에 없음");
            }

            await UniTask.Delay(200, cancellationToken: token);
        }
    }

    /// <summary>
    /// 공격 루틴
    /// </summary>
    private async UniTask AttackRoutine()
    {
        if (isAttacking || isDead) return;
        isAttacking = true;

        armAnimator.SetTrigger("isAttack");
        await UniTask.Delay(attackPreDelayTime);

        if (!isDead)
            ExecuteAttack();

        await UniTask.Delay(attackPostDelayTime);

        isAttacking = false;
        EndAttackTask();
    }

    /// <summary>
    /// 공격 판정 처리
    /// </summary>
    private void ExecuteAttack()
    {
        if (isDead) return;

        SpawnEffect();

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            attackCollider.bounds.center,
            attackCollider.bounds.size,
            0f,
            LayerMask.GetMask("DamageableEntity")
        );

        foreach (var collider in hitColliders)
        {
            DamageableEntity damageable = collider.GetComponent<DamageableEntity>();
            if (damageable != null && damageable == currentTarget)
            {
                damageable.OnHitDamage(npcData.damage, npcData.penetration, collider.transform.position, Vector2.zero);
            }
        }
    }

    /// <summary>
    /// 이펙트 생성
    /// </summary>
    private void SpawnEffect()
    {
        if (effectAnimator != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0.5f * transform.localScale.x, 0.3f, 0);
            GameObject effect = Instantiate(effectAnimator, spawnPosition, Quaternion.identity);

            Vector3 scale = effect.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * transform.localScale.x;
            effect.transform.localScale = scale;

            Destroy(effect, 1.0f);
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    public override void Die()
    {
        base.Die();

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (dieAnimator != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, -0.3f, 0);
            GameObject dieEffect = Instantiate(dieAnimator, spawnPosition, Quaternion.identity);
            Destroy(dieEffect, 10.0f);
        }

        cancellationTokenSource.Cancel(); // ✅ 비동기 루틴 중단
        Destroy(gameObject);
    }

    /// <summary>
    /// 타겟 탐색 루틴
    /// </summary>
    private async UniTask SearchTask()
    {
        var token = cancellationTokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            if (this == null || gameObject == null || isDead) return;

            DamageableEntity foundTarget = null;

            if (alertState == AlertState.Stay)
                foundTarget = FindClosestTarget(npcData.detectionRange / 2);
            else
                foundTarget = FindClosestTarget(npcData.detectionRange);

            if (foundTarget != null)
            {
                currentTarget = foundTarget;
            }

            await UniTask.Delay(1000, cancellationToken: token);
        }
    }

    /// <summary>
    /// 대기 루틴 시작
    /// </summary>
    private void StartStayTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = StayRoutine();
        alertState = AlertState.Stay;
    }

    /// <summary>
    /// 추적 루틴 시작
    /// </summary>
    private void StartTrackTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = TrackRoutine();
        alertState = AlertState.Track;
    }

    /// <summary>
    /// 공격 루틴 시작
    /// </summary>
    private void StartAttackTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = AttackRoutine();
        alertState = AlertState.Attack;
    }

    /// <summary>
    /// 공격 루틴 종료 후 추적으로 복귀
    /// </summary>
    private void EndAttackTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = TrackRoutine();
        alertState = AlertState.Track;
    }

    /// <summary>
    /// 주변 가장 가까운 타겟 탐색
    /// </summary>
    private DamageableEntity FindClosestTarget(float detectionRange)
    {
        int layerMask = LayerMask.GetMask("DamageableEntity");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, layerMask);

        DamageableEntity closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider == null) continue;
            DamageableEntity damageable = collider.GetComponent<DamageableEntity>();
            if (damageable == null || damageable.gameObject == gameObject || !damageable.gameObject.activeInHierarchy) continue;

            if (damageable.faction != faction && damageable.faction != Faction.Wall)
            {
                float distance = Vector2.Distance(transform.position, damageable.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = damageable;
                }
            }
        }

        if (closestTarget != null)
        {
            lastFindTargetTime = Time.time;
            if (alertState != AlertState.Track)
            {
                StartTrackTask();
            }
        }
        else if (Time.time - lastFindTargetTime > detectionTimeout)
        {
            StartStayTask();
        }

        return closestTarget;
    }

    /// <summary>
    /// 걷기 애니메이션 갱신
    /// </summary>
    private void UpdateBodyAnimator()
    {
        bool isWalking = agent.velocity.sqrMagnitude > 0.1f;
        bodyAnimator.SetBool("isWalk", isWalking);
    }

    /// <summary>
    /// ✅ 좀비가 타겟 방향을 바라보게 좌우 반전 처리 (문워크 방지)
    /// </summary>
    private void UpdateFacingDirection()
    {
        if (currentTarget == null) return;

        Vector2 direction = currentTarget.transform.position - transform.position;

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // 오른쪽 바라봄
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // 왼쪽 바라봄
        }
    }
}