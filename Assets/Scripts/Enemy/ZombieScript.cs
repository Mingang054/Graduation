using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components; // ✅ NavMeshPlus 네임스페이스 사용
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;

public class ZombieScript : NPCBase
{
    [SerializeField]
    private NavMeshAgent agent; // ✅ NavMeshAgent를 인스펙터에서 직접 지정
    private Transform target;
    private bool isAttacking = false;
    private float attackWindupTime = 0.5f; // ✅ 공격 선딜레이
    private float attackCooldownTime = 1.0f; // ✅ 공격 후딜레이

    private UniTask currentRoutine = UniTask.CompletedTask; // ✅ 현재 실행 중인 루틴을 추적 및 초기화
    private UniTask previousRoutine = UniTask.CompletedTask; // ✅ 이전 루틴을 저장

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); // ✅ 루틴 취소 토큰

    private float lastFindTargetTime = 0f; // ✅ 마지막으로 적을 발견한 시간
    private float detectionTimeout = 5f; // ✅ 일정 시간 동안 적을 찾지 못하면 Stay 상태로 전환

    private Transform playerTransform; // ✅ 플레이어 감지용

    public override void Awake()
    {
        base.Awake();

        // ✅ NavMeshAgent 설정
        agent.updateRotation = false;
        agent.updateUpAxis = false; // ✅ Y 축 업데이트 비활성화

        agent.avoidancePriority = 50; // ✅ 충돌 회피 우선순위 설정
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // ✅ 물리적 회피 비활성화

        agent.radius = 0.2f; // ✅ 콜라이더 크기를 줄여 충돌 면적 감소
        agent.stoppingDistance = 0.5f; // ✅ 목표에 도달 시 일정 거리 유지

        // ✅ 플레이어 태그를 가진 오브젝트 감지
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        OnStay(); // 초기 루틴 시작
    }

    private void Update()
    {
        var closestTarget = FindClosestTarget();
        if (closestTarget != null)
        {
            OnTrack();
        }
    }

    private void SetRoutine(UniTask newRoutine)
    {
        if (currentRoutine.Status == UniTaskStatus.Pending)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            previousRoutine = currentRoutine;
            currentRoutine = UniTask.CompletedTask;
        }
        currentRoutine = newRoutine;
    }

    public void OnStay()
    {
        alertState = AlertState.Stay;
        SetRoutine(StayRoutine());
    }

    public void OnTrack()
    {
        alertState = AlertState.Track;
        SetRoutine(TrackRoutine());
    }

    public void OnAttack()
    {
        alertState = AlertState.Attack;
        SetRoutine(AttackRoutine());
    }

    private async UniTask StayRoutine()
    {
        var token = cancellationTokenSource.Token;
        while (alertState == AlertState.Stay)
        {
            token.ThrowIfCancellationRequested();

            await UniTask.Delay(Random.Range(2000, 5000), cancellationToken: token);
            Vector3 randomPosition = transform.position + (Random.insideUnitSphere * 5f);
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(randomPosition);
            }
        }
    }

    private async UniTask TrackRoutine()
    {
        var token = cancellationTokenSource.Token;
        while (alertState == AlertState.Track)
        {
            token.ThrowIfCancellationRequested();

            target = FindClosestTarget();
            if (target != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                Vector3 targetPosition = target.position;
                agent.SetDestination(targetPosition);

                lastFindTargetTime = Time.time;

                float distance = Vector3.Distance(transform.position, target.position);
                if (distance <= npcData.fireRange)
                {
                    OnAttack();
                }
            }

            if (Time.time - lastFindTargetTime > detectionTimeout)
            {
                OnStay();
            }

            await UniTask.Delay(1000, cancellationToken: token);
        }
    }

    private async UniTask AttackRoutine()
    {
        var token = cancellationTokenSource.Token;
        if (isAttacking) return;
        isAttacking = true;
        agent.isStopped = true;

        await UniTask.Delay((int)(attackWindupTime * 1000), cancellationToken: token);

        if (target != null)
        {
            Debug.Log($"Attacking {target.name}!");
            DamageableEntity damageable = target.GetComponent<DamageableEntity>();
            if (damageable != null)
            {
                damageable.OnHitDamage(npcData.damage, npcData.penetration, target.position, Vector2.zero);
            }
        }

        await UniTask.Delay((int)(attackCooldownTime * 1000), cancellationToken: token);

        isAttacking = false;
        agent.isStopped = false;

        SetRoutine(previousRoutine);
    }

    private Transform FindClosestTarget()
    {
        var enemies = FindObjectsOfType<DamageableEntity>()
            .Where(e => e.faction != faction && e.faction != Faction.Wall)
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= npcData.detectionRange) // ✅ 감지 범위 내에 있는 적만
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault()?.transform;

        // ✅ 플레이어도 감지 대상에 포함
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= npcData.detectionRange)
        {
            if (enemies == null || Vector3.Distance(transform.position, playerTransform.position) < Vector3.Distance(transform.position, enemies.position))
            {
                return playerTransform;
            }
        }

        return enemies;
    }
}
