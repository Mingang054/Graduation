using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

public class ZombieScript : NPCBase
{
<<<<<<< HEAD
    [SerializeField]
    private NavMeshAgent agent;
    private Transform target;

    //공격 선딜레이, 후딜레이
    private int attackPreDelayTime = 500;
    private int attackPostDelayTime = 1000;

    [SerializeField] Animator bodyAnimator;
    [SerializeField] Animator armAnimator;



    // 상태변환 관련 태스크 (3개 상태 관련 루틴 중 하나만 등록)
    // target 탐색 태스크 ( Die가 아닐 때 반복)
    private UniTask currentStateTask = UniTask.CompletedTask;
    private UniTask searchTask = UniTask.CompletedTask;


    private string currentRoutineName = string.Empty;


    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


    // target 갱신과 함께 시간 갱신, 가장 최근에 목표를 찾은 시간
    private float lastFindTargetTime = 0f;
    // target 갱신에 실패한 경우 추적을 중지하는데 걸리는 시간
    private float detectionTimeout = 5f;


    private DamageableEntity currentTarget;

    public Collider2D attackCollider;

=======
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
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)

    public override void Awake()
    {
        base.Awake();
        agent.speed = npcData.speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = 50;
<<<<<<< HEAD
        //agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.isStopped = false;


=======
        agent.isStopped = false;
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
    }

    public void Start()
    {
<<<<<<< HEAD
        // 기존 실행 중인 searchTask가 있다면 취소
        if (searchTask.Status == UniTaskStatus.Pending)
        {
            Debug.LogWarning("[ZombieScript] 기존 SearchTask가 이미 실행 중입니다. 새로 시작할 수 없습니다.");
            return;
        }

        // 새로운 SearchTask를 시작하고 변수에 저장
        searchTask = SearchTask();
        Debug.Log("[ZombieScript] SearchTask를 시작했습니다.");

        // 초기 상태를 Stay로 설정하고 상태 루틴 시작
        StartStayTask();
    }



=======
        if (searchTask.Status == UniTaskStatus.Pending)
        {
            Debug.LogWarning("[ZombieScript] 기존 SearchTask가 이미 실행 중입니다.");
            return;
        }

        searchTask = SearchTask();
        StartStayTask();
    }

>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
    public void FixedUpdate()
    {
        UpdateBodyAnimator();
    }

<<<<<<< HEAD

    private async UniTask StayRoutine() {
        //대기 중
        await UniTask.Delay(1000); // 1초 대기
    }


    private async UniTask TrackRoutine()
    {
        Debug.Log("[ZombieScript] TrackRoutine 시작!");

        while (true)
=======
    private async UniTask StayRoutine()
    {
        await UniTask.Delay(1000);
    }

    private async UniTask TrackRoutine()
    {
        while (!isDead)
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
        {
            if (currentTarget != null && agent.isOnNavMesh)
            {
                float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
<<<<<<< HEAD

                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
                Debug.Log($"[ZombieScript] 타겟을 추적 중: {currentTarget.gameObject.name}, 거리: {distanceToTarget}");

                // ✅ 공격 범위(attackCollider) 내에 타겟이 포함되는지 확인
                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                    attackCollider.bounds.center,
                    attackCollider.bounds.size,
                    0f,
                    LayerMask.GetMask("DamageableEntity")
                );

                foreach (var collider in hitColliders)
                {
                    if (collider.gameObject == currentTarget.gameObject)
                    {
                        Debug.Log($"[ZombieScript] 타겟이 공격 범위 내에 있습니다: {currentTarget.gameObject.name}");
                        StartAttackTask(); // 공격 상태로 전환
                        // TrackRoutine 종료
                    }
=======
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);

                if (distanceToTarget <= npcData.fireRange && !isAttacking)
                {
                    StartAttackTask();
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
                }

                if (Time.time - lastFindTargetTime > detectionTimeout)
                {
                    Debug.LogWarning("[ZombieScript] 타겟을 놓쳤거나 감지 시간이 초과되었습니다.");
<<<<<<< HEAD
                    // Stay 상태 전환은 SearchTask에서 처리
=======
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
                }
            }
            else
            {
<<<<<<< HEAD
                Debug.LogWarning("[ZombieScript] currentTarget이 존재하지 않거나 NavMesh 위에 있지 않습니다.");
                // Stay 상태 전환은 SearchTask에서 처리
=======
                Debug.LogWarning("[ZombieScript] currentTarget이 없거나 NavMesh에 없음");
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
            }

            await UniTask.Delay(200);
        }
    }

<<<<<<< HEAD



    private async UniTask AttackRoutine()
    {
        // 선딜레이 대기
        Debug.Log("공격시작");
        armAnimator.SetBool("isAttack", true);
        await UniTask.Delay(attackPreDelayTime);
        //공격
        ExecuteAttack();

        // 후딜레이 대기
        armAnimator.SetBool("isAttack", false);
        await UniTask.Delay(attackPostDelayTime);
        Debug.Log("공격종료");
        EndAttackTask();
    }


=======
    private async UniTask AttackRoutine()
    {
        if (isAttacking || isDead) return;
        isAttacking = true;

        armAnimator.SetBool("isAttack", true);
        await UniTask.Delay(attackPreDelayTime);

        if (!isDead)
            ExecuteAttack();

        armAnimator.SetBool("isAttack", false);
        await UniTask.Delay(attackPostDelayTime);

        isAttacking = false;
        EndAttackTask();
    }

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

    private void SpawnEffect()
    {
        if (effectAnimator != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0.5f, 0.3f, 0);
            GameObject effect = Instantiate(effectAnimator, spawnPosition, Quaternion.identity);
            Destroy(effect, 1.0f);
        }
    }

    public override void Die()
    {
        base.Die();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        // ✅ 사망 애니메이션 생성 및 10초 후 자동 삭제
        if (dieAnimator != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, -0.3f, 0);
            GameObject dieEffect = Instantiate(dieAnimator, spawnPosition, Quaternion.identity);
            Destroy(dieEffect, 10.0f);
        }

        Destroy(gameObject);
    }
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)

    private async UniTask SearchTask()
    {
        while (true)
        {
            DamageableEntity foundTarget = null;

            if (alertState == AlertState.Stay)
<<<<<<< HEAD
            {
                foundTarget = FindClosestTarget(npcData.detectionRange / 2);
            }
            else
            {
                foundTarget = FindClosestTarget(npcData.detectionRange);
            }

            // 타겟을 찾은 경우에만 currentTarget을 갱신
            if (foundTarget != null)
            {
                currentTarget = foundTarget;
                Debug.Log($"[ZombieScript] 새로운 타겟으로 설정: {currentTarget.gameObject.name}");
            }
            else
            {
                Debug.Log("[ZombieScript] 감지된 타겟이 없습니다. currentTarget을 유지합니다.");
            }

            await UniTask.Delay(1000); // 1초 대기
        }
    }

    // Stay 상태 시작
    private void StartStayTask()
    {
        Debug.Log("[ZombieScript] Stay 상태로 전환을 시작합니다.");

        // 기존 태스크를 안전하게 중단
        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            currentStateTask = UniTask.CompletedTask; // 강제 종료
        }

        // 새로운 StayRoutine을 시작하고 상태 갱신
        currentStateTask = StayRoutine();
        alertState = AlertState.Stay;

        Debug.Log("[ZombieScript] Stay 상태로 진입 완료.");
    }

    // Track 상태 시작
    private void StartTrackTask()
    {
        Debug.Log("[ZombieScript] Track 상태로 전환을 시작합니다.");

        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            currentStateTask = UniTask.CompletedTask;
        }

        currentStateTask = TrackRoutine();
        alertState = AlertState.Track;

        Debug.Log("[ZombieScript] Track 상태로 진입 완료.");
    }

    // Attack 상태 시작
    private void StartAttackTask()
    {
        Debug.Log("[ZombieScript] Attack 상태로 전환을 시작합니다.");

        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            currentStateTask = UniTask.CompletedTask;
        }

        currentStateTask = AttackRoutine();
        alertState = AlertState.Attack;

        Debug.Log("[ZombieScript] Attack 상태로 진입 완료.");
    }

    // Attack 상태 종료 (Track 상태로 복귀)
    private void EndAttackTask()
    {
        Debug.Log("[ZombieScript] Attack 상태를 종료하고 Track 상태로 복귀합니다.");

        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            currentStateTask = UniTask.CompletedTask;
        }

        currentStateTask = TrackRoutine();
        alertState = AlertState.Track;

        Debug.Log("[ZombieScript] Track 상태로 복귀 완료.");
    }





    //공격 실행
    private void ExecuteAttack()
    {
        // 공격 범위를 사각형(OverlapBox)으로 설정
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            attackCollider.bounds.center, // 콜라이더의 중심 위치
            attackCollider.bounds.size,   // 콜라이더의 크기
            0f,                           // 회전 각도 (2D에서는 0)
            LayerMask.GetMask("DamageableEntity") // 감지할 Layer      //잘못됨
        );

        foreach (var collider in hitColliders)
        {
            DamageableEntity damageable = collider.GetComponent<DamageableEntity>();
            if (damageable != null && damageable == currentTarget)
            {
                Debug.Log($"[ZombieScript] 공격 성공! 타겟: {damageable.gameObject.name}");
                damageable.OnHitDamage(npcData.damage, npcData.penetration, collider.transform.position, Vector2.zero);
            }
        }
    }


    //다른 팩션의 Target탐색
    // 다른 팩션의 Target 탐색
    private DamageableEntity FindClosestTarget(float detectionRange)
    {
        int layerMask = LayerMask.GetMask("DamageableEntity");
        Debug.Log($"LayerMask 값: {layerMask}");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, layerMask);

        Debug.Log($"[SoliderScript] 감지된 2D Collider 수: {colliders.Length}");

=======
                foundTarget = FindClosestTarget(npcData.detectionRange / 2);
            else
                foundTarget = FindClosestTarget(npcData.detectionRange);

            if (foundTarget != null)
            {
                currentTarget = foundTarget;
            }

            await UniTask.Delay(1000);
        }
    }

    private void StartStayTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = StayRoutine();
        alertState = AlertState.Stay;
    }

    private void StartTrackTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = TrackRoutine();
        alertState = AlertState.Track;
    }

    private void StartAttackTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = AttackRoutine();
        alertState = AlertState.Attack;
    }

    private void EndAttackTask()
    {
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        currentStateTask = TrackRoutine();
        alertState = AlertState.Track;
    }

    private DamageableEntity FindClosestTarget(float detectionRange)
    {
        int layerMask = LayerMask.GetMask("DamageableEntity");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, layerMask);

>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
        DamageableEntity closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
<<<<<<< HEAD
            if (collider == null) continue; // ✅ collider가 null이면 건너뜀

            Debug.Log($"[SoliderScript] 감지된 2D Collider: {collider.gameObject.name}");

            DamageableEntity damageable = collider.GetComponent<DamageableEntity>();
            if (damageable == null || damageable.gameObject == gameObject) continue; // ✅ damageable이 null이면 건너뜀

            // ✅ Faction이 다르고 Wall이 아닌 경우만 타겟으로 선정
=======
            if (collider == null) continue;
            DamageableEntity damageable = collider.GetComponent<DamageableEntity>();
            if (damageable == null || damageable.gameObject == gameObject) continue;

>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
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
<<<<<<< HEAD
            lastFindTargetTime = Time.time; // ✅ 타겟을 찾은 시점을 갱신
            Debug.Log($"[SoliderScript] 가장 가까운 타겟: {closestTarget.gameObject.name}, 시간 갱신: {lastFindTargetTime}");

            // ✅ 탐색에 성공했고 현재 상태가 Track이 아닌 경우 TrackTask 실행
            if (alertState != AlertState.Track)
            {
                StartTrackTask();
                Debug.Log("[SoliderScript] 타겟을 감지했으므로 Track 상태로 전환.");
            }
        }
        else
        {
            Debug.Log("[SoliderScript] 감지된 타겟이 없습니다.");

            // ✅ 타겟을 찾지 못했고 detectionTimeout이 지났다면 Stay 상태로 전환
            if (Time.time - lastFindTargetTime > detectionTimeout)
            {
                StartStayTask();
                Debug.Log("[SoliderScript] 타겟을 찾지 못해 Stay 상태로 전환.");
            }
=======
            lastFindTargetTime = Time.time;
            if (alertState != AlertState.Track)
            {
                StartTrackTask();
            }
        }
        else if (Time.time - lastFindTargetTime > detectionTimeout)
        {
            StartStayTask();
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
        }

        return closestTarget;
    }

<<<<<<< HEAD





    private void UpdateBodyAnimator()
    {



        // NavMeshAgent의 실제 이동 속도를 기준으로 걷기 애니메이션을 설정
        bool isWalking = agent.velocity.sqrMagnitude > 0.1f;

        bodyAnimator.SetBool("isWalk", isWalking);

        /*
        if (agent.isStopped == false)
        {
            bodyAnimator.SetBool("isWalk", true);
        }
        else
        {
            bodyAnimator.SetBool("isWalk", false);
        }*/
    }

}
=======
    private void UpdateBodyAnimator()
    {
        bool isWalking = agent.velocity.sqrMagnitude > 0.1f;
        bodyAnimator.SetBool("isWalk", isWalking);
    }
}
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
