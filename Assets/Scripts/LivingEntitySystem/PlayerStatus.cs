using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
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
    public float staminaPoint = 100;
    public float staminaPointMax = 100;

    public float eneregy = 100f;
    public float hydration = 100;
    public float weightMax;
    public bool isBleeding = false;


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
        
        if (PlayerStatus.instance == null)
        {
            PlayerStatus.instance = this;
        }

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


    public void StartBleed()
    {
        isBleeding = true;
    }
    public void StopBleed() {
        isBleeding = false;
    }


    private class RegenEffect
    {
        public float hpPerSec, spPerSec, enPerSec, hyPerSec;
        public float duration;
        public CancellationTokenSource cts;
        public UniTask task;
    }
    private readonly List<RegenEffect> _activeRegens = new();
    /* ── 2. 지속효과 시작 함수 ───────────────────── */
    private void StartRegenRoutine(ConsumableData data)
    {
        if (data.regenDuration <= 0) return;          // 지속시간 0 → 패스

        // effect 구성
        var eff = new RegenEffect
        {
            hpPerSec = data.hpRegen,
            spPerSec = data.spRegen,
            enPerSec = data.energyRegen,
            hyPerSec = data.hydrationRegen,
            duration = data.regenDuration,
            cts = new CancellationTokenSource()
        };

        // 목록에 보관
        _activeRegens.Add(eff);

        // 태스크 시작
        eff.task = RegenLoop(eff, eff.cts.Token).AttachExternalCancellation(eff.cts.Token);
    }

    /* ── 3. UniTask 루틴 ─────────────────────────── */
    private async UniTask RegenLoop(RegenEffect eff, CancellationToken token)
    {
        float elapsed = 0f;

        try
        {
            while (elapsed < eff.duration && !token.IsCancellationRequested)
            {
                float dt = Time.deltaTime;            // ▶ TimeScale 0 에선 0이 됨!
                elapsed += dt;

                // 회복 적용
                healthPoint = Mathf.Min(healthPoint + eff.hpPerSec * dt, healthPointMax);
                staminaPoint = Mathf.Min(staminaPoint + eff.spPerSec * dt, staminaPointMax);
                eneregy += eff.enPerSec * dt;
                hydration += eff.hyPerSec * dt;

                UpdateStatusUI();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        finally   // 종료(만료·취소) 시 정리
        {
            _activeRegens.Remove(eff);
            eff.cts.Dispose();
        }
    }

    /* ── 4. 소비 아이템 사용 로직 ────────────────── */
    public bool UseConsumable(ConsumableData c)
    {
        if (c == null) return false;

        // ① 즉시 회복
        healthPoint = Mathf.Min(healthPoint + c.hp, healthPointMax);
        staminaPoint = Mathf.Min(staminaPoint + c.sp, staminaPointMax);
        eneregy += c.energy;
        hydration += c.hydration;

        // ② 지혈·상태 이상
        if (c.hemostasis && isBleeding) StopBleed();

        // ③ 지속 회복 태스크(있다면) 시작
        StartRegenRoutine(c);

        UpdateStatusUI();
        return true;
    }

    /* ── 5. 씬 종료·사망 시 모든 효과 정리 ───────── */
    private void CancelAllRegens()
    {
        foreach (var eff in _activeRegens)
            eff.cts.Cancel();
        _activeRegens.Clear();
    }
    public override void Die()
    {
        CancelAllRegens();
        UIManager.Instance.EnableGameOverUI();
        base.Die();
        //base.Die();
    }
}
