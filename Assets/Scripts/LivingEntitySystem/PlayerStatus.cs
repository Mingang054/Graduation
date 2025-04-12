using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerStatus : DamageableEntity
{

    public float healthMax;
    public float staminaMax;
    
    public float weightMax;

    public float money;



    public NPCData npcData;
    private Animator animator;
    private Collider2D npcCollider;
    private AudioSource audioSource;

    public bool isAttacking = false;


    public AlertState alertState;





    public override void Awake()
    {
    }

    void Start()
    {
        
    }





    // Update is called once per frame
    void Update()
    {
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








}
