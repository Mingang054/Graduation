using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public Transform weaponPivotTransform; // 총의 피벗
    public Transform armTransform; // 팔(Arm) Transform
    private Camera mainCamera;

    private float lastFireTime;
    private bool triggered = false;
    private InputAction fireAction;

    public WeaponData currentWeapon;


    [SerializeField]                //
    private Transform playerToFlip;

    // 🔹 캐릭터 반전 관련 변수
    private Vector3 previousMousePosition;
    private float previousDirection = 1f;
    private AudioSource audioSource;

    private void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();

        // 🔹 Input System 설정
        var playerInput = new InputActionMap("Player");
        fireAction = playerInput.AddAction("Fire1", binding: "<Mouse>/leftButton");

        fireAction.started += OnFireStarted;
        fireAction.performed += OnFirePerformed;
        fireAction.canceled += OnFireCanceled;
    }

    private void OnEnable()
    {
        fireAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
    }

    private void Update()
    {
        if (currentWeapon == null) return;

        RotateArmToMouse();
        FlipCharacter();
    }

    private void FixedUpdate()
    {
        if (triggered && Time.time >= lastFireTime + currentWeapon.fireRate)
        {
            TryFire();
        }
        else if (!currentWeapon.fireMode.Equals(FireMode.FullAuto)) // 반자동 모드일 경우 한 번만 발사
        {
            triggered = false;
        }
    }

    // 🔹 마우스 클릭 시작 (사격 준비)
    private void OnFireStarted(InputAction.CallbackContext context)
    {
        triggered = true;
    }

    // 🔹 마우스 클릭 중 (반자동 or 연사)
    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (currentWeapon.fireMode == FireMode.SemiAuto)
        {
            TryFire();
            triggered = false; // 한 번만 발사하도록 설정
        }
    }

    // 🔹 마우스 클릭 해제 (발사 중지)
    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        triggered = false;
    }

    private void TryFire()
    {
        if (currentWeapon == null) return;

        lastFireTime = Time.time;

        // 🔹 발사 성공 시 효과음 재생
        if (currentWeapon.attackClip != null)
        {
            audioSource.PlayOneShot(currentWeapon.attackClip);
        }

        for (int i = 0; i < currentWeapon.pelletCount; i++)
        {
            FireProjectile();
        }
    }
    private void FireProjectile()
    {
        // 🔹 마우스 위치를 가져와 정규화된 방향 벡터 계산
        Vector3 mousePos = GetMouseWorldOnPlane(weaponPivotTransform.position.z);

        Vector2 fireDir = (mousePos - weaponPivotTransform.position).normalized;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // 2D 환경에서 Z 축 제거
        Vector2 fireDirection = (mousePosition - weaponPivotTransform.position).normalized;

        // 🔹 새로운 발사 위치 계산 (마우스 방향으로 barrelLength 만큼 떨어진 위치)
        Vector3 firePosition = weaponPivotTransform.position + (Vector3)(fireDirection * currentWeapon.barrelLength);

        // 🔹 분산도 적용
        float uncertainty = Random.Range(-currentWeapon.dispersion, currentWeapon.dispersion);
        Quaternion rotation = Quaternion.Euler(0, 0, uncertainty);

        // 🔹 최종 발사 방향 (분산 적용)
        Vector2 finalDirection = rotation * fireDirection;

        // 🔹 오브젝트 풀에서 탄환 가져오기
        GameObject newProjectile = ProjectilePoolManager.Instance.GetProjectile(
            Faction.Friendly,  // 🔹 플레이어가 발사한 탄환
            2f,  // 삭제 시간
            currentWeapon.projectileSpeed,  //발사체 속도
            currentWeapon.damage,   //발사체 피해
            currentWeapon.penetration,  //발사체 관통피해
            currentWeapon.colliderSize  //발사체 충돌 크기 설정
        );

        if (newProjectile != null)
        {
            newProjectile.transform.position = firePosition;

            // 🔹 발사 방향을 기반으로 회전 설정 (이동 방향에 맞춰 자동 조정)
            float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
            newProjectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            newProjectile.GetComponent<Projectile>().Launch(finalDirection);
        }
    }

    private Vector3 GetMouseWorldOnPlane(float worldZ)
    {
        Vector3 sp = Input.mousePosition;
        // “카메라에서 이 거리” = 원하는 월드 z – 카메라 z
        sp.z = worldZ - mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(sp);
    }




    // 🔹 마우스 방향을 바라보도록 팔 회전
    private void RotateArmToMouse()
    {
        Vector3 mousePos = GetMouseWorldOnPlane(armTransform.position.z);

        if ((mousePos - previousMousePosition).sqrMagnitude < 0.0001f) return;
        previousMousePosition = mousePos;

        Vector2 dir = (mousePos - armTransform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (playerToFlip.transform.localScale.x < 0)   // 좌우 반전 보정
            angle -= 180f;

        armTransform.rotation = Quaternion.Euler(0, 0, angle);
    }


    // 🔹 캐릭터 좌우 반전
    private void FlipCharacter()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 direction = mousePosition - playerToFlip.transform.position;
        float currentDirection = Mathf.Sign(direction.x);

        if (currentDirection != previousDirection)
        {
            previousDirection = currentDirection;
            playerToFlip.transform.localScale = new Vector3(currentDirection, 1, 1);
        }
    }
}
