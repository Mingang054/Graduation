using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 이동 & 스프린트
///  └ 스태미나 소모·회복은 PlayerStatus의 staminaPoint를 직접 사용
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /* ────────────────── 싱글턴 ────────────────── */
    public static PlayerMovement Instance { get; private set; }

    /* ────────────────── 레퍼런스 ───────────────── */
    [SerializeField] private Animator animator;
    private Rigidbody2D playerRigidbody2D;
    private Camera mainCamera;
    private PlayerStatus status;                // ★ PlayerStatus 참조

    /* ────────────────── 스피드 ────────────────── */
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7.5f;

    public float WalkSpeed
    {
        get => walkSpeed;
        private set => walkSpeed = Mathf.Clamp(value, 0f, 20f);
    }
    public float SprintSpeed
    {
        get => sprintSpeed;
        private set => sprintSpeed = Mathf.Clamp(value, 0f, 20f);
    }

    /* ────────────────── 스프린트 자원 ──────────── */
    [Header("스태미나 소모·회복 (초당)")]
    [SerializeField] private float staminaCostPerSec = 5f;  // 달릴 때 초당 소모
    [SerializeField] private float staminaRegenPerSec = 1f;  // 걷기/정지 중 초당 회복

    /* ────────────────── 입력 & 상태 ─────────────── */
    private Vector2 moveInput;
    private bool isSprinting = false;  // 버튼 누름 상태
    private bool isSprintable = true;   // 스태미나가 남아 달릴 수 있는지
    private bool isSprintReset = true;   // 버튼 떼었다 → 재눌러야 다시 달림

    /* 애니메이터 캐시 */
    private bool wasWalking = false;
    private bool wasSprinting = false;

    /* ────────────────── Unity 라이프사이클 ──────── */
    private void Awake()
    {
        /* 싱글턴 */
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        playerRigidbody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        /* PlayerStatus 찾기 */
        status = PlayerStatus.instance ?? GetComponent<PlayerStatus>();
        if (status == null)
            Debug.LogError("PlayerStatus 인스턴스를 찾을 수 없습니다!", this);
    }

    private void Start()
    {
        if (status == null)
        {
            status = PlayerStatus.instance ?? GetComponent<PlayerStatus>();
        }
    }
    /* ────────────────── 입력 시스템 ─────────────── */
    public void OnMove(InputValue value)            // WASD
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            if (!isSprintReset) return;             // 버튼 떼기 전에 재입력 방지
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
            isSprintReset = true;
        }
    }

    /* ────────────────── 메인 루프 ──────────────── */
    void FixedUpdate()
    {
        ControlMovementOptimized();   // 이동 + 스태미나
        UpdateAnimatorOptimized();    // 애니메이션 변동 시만 적용
    }

    /* ────────────────── 이동 & 스태미나 ─────────── */
    private void ControlMovementOptimized()
    {
        /* 이동 입력이 없으면 회복만 */
        if (moveInput == Vector2.zero)
        {
            RegainStamina();
            return;
        }

        bool hasStamina = status.staminaPoint > 0f;

        /* 스프린트 가능 여부 재평가 */
        if (!hasStamina)
        {
            isSprintable = false;
            isSprinting = false;
        }
        else if (!isSprinting) isSprintable = true;

        /* 이동 */
        if (isSprinting && isSprintable && hasStamina)
        {
            isSprintReset = false;
            DrainStamina();
            MovePlayer(sprintSpeed);
        }
        else
        {
            RegainStamina();
            MovePlayer(walkSpeed);
        }
    }

    private void MovePlayer(float speed)
    {
        Vector2 movement = moveInput * speed * Time.fixedDeltaTime;
        playerRigidbody2D.MovePosition(playerRigidbody2D.position + movement);
    }

    private void DrainStamina()
    {
        status.staminaPoint = Mathf.Max(
            0f,
            status.staminaPoint - staminaCostPerSec * Time.fixedDeltaTime);
    }

    private void RegainStamina()
    {
        status.staminaPoint = Mathf.Clamp(
            status.staminaPoint + staminaRegenPerSec * Time.fixedDeltaTime,
            0f,
            status.staminaPointMax);
    }

    /* ────────────────── 애니메이터 최적화 ───────── */
    private void UpdateAnimatorOptimized()
    {
        bool isWalkingNow = moveInput != Vector2.zero && !isSprinting;
        bool isSprintingNow = isSprinting && moveInput != Vector2.zero;

        if (isWalkingNow != wasWalking)
        {
            animator.SetBool("isWalk", isWalkingNow);
            wasWalking = isWalkingNow;
        }
        if (isSprintingNow != wasSprinting)
        {
            animator.SetBool("isSprint", isSprintingNow);
            wasSprinting = isSprintingNow;
        }
    }

    /* ────────────────── 외부에서 걷기 속도 조정 ─── */
    public void SetMoveSpeed(float newSpeed) => WalkSpeed = newSpeed;
}
