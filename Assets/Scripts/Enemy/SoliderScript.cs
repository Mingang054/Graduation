using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class SoliderScript : NPCBase
{
    [SerializeField] private NavMeshAgent agent;
    private Transform target;

    private DamageableEntity currentTarget;
    private float currentDistance = Mathf.Infinity;

    // target 갱신과 함께 시간 갱신, 가장 최근에 목표를 찾은 시간
    private float lastFindTargetTime = 0f;
    // target 갱신에 실패한 경우 추적을 중지하는데 걸리는 시간
    private float detectionTimeout = 20f;


    //FireTask 실행용 플래그
    private bool isFiring = false;
    // 공격 선딜레이, 연사 속도, 후딜레이
    
    private int attackPreDelayTime = 500;
    private int attackRateOfFireTime = 200;
    private int attackPostDelayTime = 1000;
    private int reloadCount = 15;
    private int reloadMax = 15;


    //확률
    public int rushRate = 2;
    public int stopRate = 1;
    public int aroundRate = 4;



    [SerializeField] Animator bodyAnimator;
    [SerializeField] Animator armAnimator;

    // 상태변환 관련 태스크 (3개 상태 관련 루틴 중 하나만 등록)
    // target 탐색 태스크 (Die가 아닐 때 반복)
    private UniTask currentStateTask = UniTask.CompletedTask;
    private UniTask searchTask = UniTask.CompletedTask;
    private UniTask currentAttackTask = UniTask.CompletedTask;
    private UniTask shooterTask = UniTask.CompletedTask;

    private string currentStateName = string.Empty;
    private CancellationTokenSource stateCancelToken = new CancellationTokenSource();
    private CancellationTokenSource searchCancelToken = new CancellationTokenSource();

    public Collider2D attackCollider;

    public override void Awake()
    {
        base.Awake();
        faction = npcData.faction;
        agent.speed = npcData.speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = 50;
        agent.isStopped = false;
    }

    public void Start()
    {
        StartStayTask();
        StartSearchTask();
    }

    public override void Die()
    {
        base.Die(); // ✅ 부모 클래스의 Die() 실행 (기본 기능 유지)

        Debug.Log("[SoliderScript] 사망 처리 - 모든 동작 중지");

        // ✅ NavMeshAgent 정지
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // ✅ 실행 중인 모든 태스크 즉시 중지 (CompleteTask 활용)
        if (currentStateTask.Status == UniTaskStatus.Pending)
            currentStateTask = UniTask.CompletedTask;

        if (searchTask.Status == UniTaskStatus.Pending)
            searchTask = UniTask.CompletedTask;

        if (currentAttackTask.Status == UniTaskStatus.Pending)
            currentAttackTask = UniTask.CompletedTask;


        // ✅ CancellationToken을 통한 중지
        stateCancelToken.Cancel();
        searchCancelToken.Cancel();

        Debug.Log("[SoliderScript] 모든 태스크 중지 완료");
    }





    private async UniTask SearchTask(CancellationToken token)
    {
        while (true)
        {
            float detectionRange = (alertState == AlertState.Stay) ? npcData.detectionRange / 2f : npcData.detectionRange;

            DamageableEntity foundTarget = FindClosestTarget(detectionRange);


            await UniTask.Delay(1500);
        }
    }

    private async UniTask FireTask(CancellationToken token)
    {
        await UniTask.Delay(attackPreDelayTime, cancellationToken: token);
        if (currentDistance < npcData.fireRange / 2)
        {
            reloadCount -= 6;
            //6발 연사
            FireProjectile();
            Debug.Log("근접사격");
        }
        else
        {
            reloadCount -= 3;
            //3발 연사
            FireProjectile();
            Debug.Log("정밀사격");
        }
        if (reloadCount <= 0)
        {
            //재장전(일정시간 대기 후
            await UniTask.Delay(6000, cancellationToken: token);
            reloadCount = reloadMax;
        }
        await UniTask.Delay(attackPostDelayTime, cancellationToken: token);
        StartSearchTask();
    }










    // StateTask 관련 루틴 (기본 루프 추가, 취소 가능하게 변경)
    private async UniTask StayRoutine(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Random.Range(0, 4) == 0) // 0~3 사이의 랜덤 값 생성
            {
                MoveToRandomPosition(transform, npcData.fireRange/2);
            }
            await UniTask.Delay(2000, cancellationToken: token);
        }
    }

    private async UniTask TrackRoutine(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            ChooseRandomAction();
            await UniTask.Delay(2000, cancellationToken: token);
        }
    }


    // 공격상태 이동 루틴
    // AttackTask 관련 루틴 (취소 가능하게 변경)
    private async UniTask RushRoutine(CancellationToken token)
    {

        if (currentTarget != null && agent.isOnNavMesh)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

            agent.isStopped = false;
            agent.SetDestination(currentTarget.transform.position);
            MoveToRandomPosition(currentTarget.transform,npcData.fireRange/2);


        }

        //여기까지 작업중


        ///////토큰 과 서순 명확하게 해야함
        await UniTask.Delay(1000, cancellationToken: token);
        StartTrackTask();
    }

    private async UniTask StopRoutine(CancellationToken token)
    {
        //단순 정지대기
        await UniTask.Delay(1000, cancellationToken: token);
        StartTrackTask();
    }

    private async UniTask AroundRoutine(CancellationToken token)
    {

        MoveToRandomPosition(transform, npcData.fireRange);
        await UniTask.Delay(1000, cancellationToken: token);
        StartTrackTask();
    }

    // 루틴 시작 함수

    private void StartSearchTask()
    {
        isFiring = false;
        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            searchCancelToken.Cancel(); // 기존 태스크 취소
            searchCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        Debug.Log("[SoliderScript] SearchTask 시작!");
        searchTask = SearchTask(searchCancelToken.Token);
    }
    
    private void StartFireTask()
    {
        isFiring = true;
        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            searchCancelToken.Cancel(); // 기존 태스크 취소
            searchCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        Debug.Log("[SoliderScript] FireTask 시작!");
        searchTask = FireTask(searchCancelToken.Token);
    }

    // StateTask
    private void StartStayTask()
    {
        Debug.Log("[SoliderScript] StayTask 시작!");

        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            stateCancelToken.Cancel(); // 기존 태스크 취소
            stateCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        alertState = AlertState.Stay;
        currentTarget = null;
        currentStateTask = StayRoutine(stateCancelToken.Token);
    }

    private void StartTrackTask()
    {
        Debug.Log("[SoliderScript] TrackTask 시작!");

        if (currentStateTask.Status == UniTaskStatus.Pending)
        {
            stateCancelToken.Cancel(); // 기존 태스크 취소
            stateCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        alertState = AlertState.Track;
        currentStateTask = TrackRoutine(stateCancelToken.Token);
    }

    // AttackTask
    private void StartRushTask()
    {
        Debug.Log("[SoliderScript] RushTask 시작!");

        if (currentAttackTask.Status == UniTaskStatus.Pending)
        {
            stateCancelToken.Cancel(); // 기존 태스크 취소
            stateCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        currentAttackTask = RushRoutine(stateCancelToken.Token);
    }

    private void StartStopTask()
    {
        Debug.Log("[SoliderScript] StopTask 시작!");

        if (currentAttackTask.Status == UniTaskStatus.Pending)
        {
            stateCancelToken.Cancel(); // 기존 태스크 취소
            stateCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        currentAttackTask = StopRoutine(stateCancelToken.Token);
    }

    private void StartAroundTask()
    {
        Debug.Log("[SoliderScript] AroundTask 시작!");

        if (currentAttackTask.Status == UniTaskStatus.Pending)
        {
            stateCancelToken.Cancel(); // 기존 태스크 취소
            stateCancelToken = new CancellationTokenSource(); // 새로운 토큰 생성
        }
        currentAttackTask = AroundRoutine(stateCancelToken.Token);
    }




    // 탐색
    private DamageableEntity FindClosestTarget(float detectionRange)
    {
        // 🔹 FireTask 실행 중이면 새로운 타겟을 설정하지 않음
        if (isFiring)
        {
            Debug.Log("[SoliderScript] FireTask 실행 중 - 새로운 타겟 설정 안 함.");
            return currentTarget; // 기존 타겟 유지
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, LayerMask.GetMask("DamageableEntity"));
        DamageableEntity closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider == null) continue;

            DamageableEntity damageable = collider.GetComponent<DamageableEntity>();
            if (damageable == null || damageable.gameObject == gameObject) continue;

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
            if (alertState != AlertState.Track)
            {
                StartTrackTask();
            }
            lastFindTargetTime = Time.time;

            // 🔹 새로운 타겟 저장 시 현재 거리도 함께 저장
            currentTarget = closestTarget;
            currentDistance = closestDistance;

            // 🔹 타겟이 사격 범위 내에 있으면 FireTask 시작
            if (currentDistance <= npcData.fireRange)
            {
                StartFireTask();
                Debug.Log($"[SoliderScript] 타겟이 사격 범위 내에 있음! FireTask 시작 (거리: {currentDistance})");
            }
        }
        else if (Time.time - lastFindTargetTime > detectionTimeout)
        {
            StartStayTask();
        }

        return closestTarget;
    }


    //TrackTask에서 행동 Task를 선택하는 함수
    private void ChooseRandomAction()
    {
        (int limit, System.Action startRoutine)[] actions =
        {
            (rushRate, StartRushTask),
            (rushRate + stopRate, StartStopTask),
            (rushRate + stopRate + aroundRate, StartAroundTask)
        };

        int randomValue = Random.Range(0, actions[^1].limit);

        foreach (var (limit, startRoutine) in actions)
        {
            if (randomValue < limit)
            {
                startRoutine();
                break;
            }
        }
    }




    private void MoveToRandomPosition(Transform referenceObject, float range)
    {
        if (referenceObject == null)
        {
            //Debug.LogWarning("[SoliderScript] 기준 오브젝트가 없음.");
            return;
        }

        Vector3 randomPosition = GetValidRandomPosition(referenceObject.position, range, 5);

        if (randomPosition != Vector3.zero)
        {
            agent.SetDestination(randomPosition);
            //Debug.Log($"[SoliderScript] {referenceObject.name} 기준으로 이동 가능 위치 발견: {randomPosition}");
        }
        else if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.transform.position);
            //Debug.Log($"[SoliderScript] 랜덤 위치 실패 → currentTarget({currentTarget.name}) 위치로 이동");
        }
        else
        {
            //Debug.Log("[SoliderScript] 이동할 수 있는 위치를 찾지 못함.");
        }
    }

    private Vector3 GetValidRandomPosition(Vector3 origin, float range, int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0);
            Vector3 potentialPosition = origin + randomOffset;

            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position; // 이동 가능한 위치 반환
            }
        }
        return Vector3.zero; // 실패 시 currentTarget 위치 사용
    }

    private void FireProjectile()
    {
        if (currentTarget == null)
        {
            Debug.Log("[SoliderScript] FireProjectile 중단 - 타겟 없음.");
            return;
        }

        // 🔹 ProjectilePoolManager 최적화
        var poolManager = ProjectilePoolManager.Instance;

        // 🔹 타겟 위치를 가져와 정규화된 방향 벡터 계산
        Vector3 targetPosition = currentTarget.transform.position;
        Vector2 fireDirection = (targetPosition - transform.position).normalized;

        // 🔹 동적 총열 길이 값 적용
        //float barrelLength = npcData.barrelLength > 0 ? npcData.barrelLength : 0.5f;
        Vector3 firePosition = transform.position + (Vector3)(fireDirection * 0.5f);

        // 🔹 최적화된 분산도 적용 (2D 환경에 적합)
        float angleOffset = Random.Range(-1f, 1f);
        float radianOffset = angleOffset * Mathf.Deg2Rad;
        Vector2 finalDirection = new Vector2(
            fireDirection.x * Mathf.Cos(radianOffset) - fireDirection.y * Mathf.Sin(radianOffset),
            fireDirection.x * Mathf.Sin(radianOffset) + fireDirection.y * Mathf.Cos(radianOffset)
        );

        // 🔹 오브젝트 풀에서 탄환 가져오기
        GameObject newProjectile = poolManager.GetProjectile(
            npcData.faction, 2f, 2f, npcData.damage, npcData.penetration, 0.05f
        );

        if (newProjectile == null)
        {
            Debug.LogWarning("[SoliderScript] 탄환 생성 실패 - ProjectilePool이 가득 찼을 수 있음.");
            return;
        }

        newProjectile.transform.position = firePosition;

        // 🔹 발사 방향을 기반으로 회전 설정 (이동 방향에 맞춰 자동 조정)
        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
        newProjectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        newProjectile.GetComponent<Projectile>().Launch(finalDirection);
    }


}