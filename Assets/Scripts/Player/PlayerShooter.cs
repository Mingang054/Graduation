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
            2f,  // 삭제 시간
            currentWeapon.projectileSpeed,
            currentWeapon.damage,
            currentWeapon.penetration,
            currentWeapon.colliderSize
        );

        if (newProjectile != null)
        {
            newProjectile.transform.position = firePosition;

            // 🔹 발사 방향을 기반으로 회전 설정 (90도 보정 제거)
            float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
            newProjectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            newProjectile.GetComponent<Projectile>().Launch(finalDirection);
        }
    }




    // 🔹 마우스 방향을 바라보도록 팔 회전
    private void RotateArmToMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition == previousMousePosition) return;
        previousMousePosition = mousePosition;

        mousePosition.z = 0;
        Vector3 direction = (mousePosition - armTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (transform.localScale.x < 0) // 왼쪽 바라볼 때 반전 보정
        {
            angle -= 180f;
        }

        armTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // 🔹 캐릭터 좌우 반전
    private void FlipCharacter()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 direction = mousePosition - transform.position;
        float currentDirection = Mathf.Sign(direction.x);

        if (currentDirection != previousDirection)
        {
            previousDirection = currentDirection;
            transform.localScale = new Vector3(currentDirection, 1, 1);
        }
    }
}
