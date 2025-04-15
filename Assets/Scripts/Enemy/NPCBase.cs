using UnityEngine;

using System.Collections;

public class NPCBase : DamageableEntity
{
    public NPCData npcData;
    private Animator animator;
    private Collider2D npcCollider;
    private AudioSource audioSource;

    public bool isAttacking = false;


    public AlertState alertState;



    public override void Awake()
    {
        base.Awake();

        initNPC();                                      //NPC의 데이터 불러오기
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>(); // 애니메이션 컨트롤
        npcCollider = GetComponent<Collider2D>(); // 충돌 비활성화용
    }
    
    public void initNPC()
    {
        if (npcData == null)
        {
            Debug.LogError("initNPC() 호출 전에 NPCData가 설정되어야 합니다!", this);
            return;
        }

        alertState = AlertState.Stay;   //경계상태 초기화
        // ✅ 체력 및 방어력 설정
        healthPointMax = npcData.health;
        healthPoint = npcData.health;
        armorPoint = npcData.armor;

        // ✅ 이동 속도 설정
        float speed = npcData.speed;

        // ✅ 공격력 및 관통력 설정
        float damage = npcData.damage;
        float penetration = npcData.penetration;

        // ✅ 감지 범위 및 공격 사거리 설정
        float detectionRange = npcData.detectionRange;
        float fireRange = npcData.fireRange;

        // ✅ Faction, 공격 타입, 추적 타입 설정
        Faction faction = npcData.faction;
        AttackType attackType = npcData.attckType;
        TrackType trackType = npcData.trackType;

        // ✅ 애니메이터 설정
        animator = GetComponent<Animator>();
        if (animator)
        {
            if (npcData.bodyAnimation) animator.Play(npcData.bodyAnimation.name);
        }

        // ✅ 오디오 소스 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ✅ essential 값이 true라면 특별 처리
        if (npcData.essential)
        {   
            DontDestroyOnLoad(gameObject); // 중요한 NPC는 씬 전환 시 제거되지 않음
        }

        // ✅ 콜라이더 가져오기
        npcCollider = GetComponent<Collider2D>();

        Debug.Log($"NPC '{gameObject.name}' 초기화 완료!", this);
    }


    public override void OnHitDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal, Faction projectileFaction)
    {
        base.OnHitDamage(damage, penetration, hitPoint, hitNormal, projectileFaction);

        // ✅ 피격 오디오 재생 (null 체크)
        if (npcData.onHitAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(npcData.onHitAudio);
        }
    }

    public override void Die()
    {
        base.Die();

        if (npcData)
        {
            // ✅ 사망 애니메이션 실행
            if (animator)
            {
                animator.SetTrigger("Die");
            }

            // ✅ 사망 오디오 재생 (null 체크)
            if (npcData.deathAudio != null && audioSource != null)
            {
                audioSource.PlayOneShot(npcData.deathAudio);
            }

            // ✅ 물리 충돌 제거
            if (npcCollider)
            {
                npcCollider.enabled = false;
            }

            // ✅ AI, 이동 관련 스크립트 비활성화
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) // Die()를 실행하는 NPCBase는 유지
                {
                    script.enabled = false;
                }
            }

            // ✅ 사망 확인용 시각 테스트
            transform.rotation = Quaternion.Euler(0, 0, 90);

            // ✅ 10초 후 삭제
            StartCoroutine(DestroyAfterDelay(10f));
        }
    }



    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }






}
