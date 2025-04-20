using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerStatus : DamageableEntity
{
    /*-- Damageable 코드 포함
    public string npcCode;
    public float healthPointMax = 100;                   //최대체력
    public float healthPoint 
    public float armorPoint = 0f;                        //방어내구도
    public float reduction = 0f;                    //피해감소
    */
    public float weightMax;

    public float money;



    public Slider hpSlider;
    public Slider spSlider;

    public NPCData npcData;
    private Animator animator;
    // entityCollider = GetComponent<Collider2D>(); 
    //X private Collider2D npcCollider;
    private AudioSource audioSource;


    public static PlayerStatus instance;



    public override void Awake()
    {
        faction = Faction.Friendly;
        entityCollider = GetComponent<Collider2D>();
        
        PlayerStatus.instance = this;

    }

    void Start()
    {
        initStatusUI();
    }





    // Update is called once per frame
    void Update()
    {
    }

    public override void OnHitDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal, Faction projectileFaction)
    {
        
        base.OnHitDamage(damage, penetration, hitPoint, hitNormal, projectileFaction);
        UpdateStatusUI();
    }

    public void initStatusUI()
    {
        hpSlider.maxValue = healthPointMax;
        UpdateStatusUI();
    }
    public void UpdateStatusUI()
    {
        hpSlider.value = healthPoint;
        //
    }

    public void initNPC()
    {
        if (npcData == null)
        {
            Debug.LogError("initNPC() 호출 전에 NPCData가 설정되어야 합니다!", this);
            return;
        }

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
        entityCollider = GetComponent<Collider2D>();

        Debug.Log($"NPC '{gameObject.name}' 초기화 완료!", this);
    }


    public override void Die() {
        return;
    }





}
