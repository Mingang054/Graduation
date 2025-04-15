
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public Transform weaponPivotTransform; // 총의 피벗
    public Transform armTransform; // 팔(Arm) Transform
    private Camera mainCamera;


    [SerializeField]
    GameObject zoomObject;
    //--  --//
    private float lastFireTime;
    private bool triggered = false;

    private InputAction fireAction;
    private InputAction zoomAction;

    public Vector3 mouseWorld = Vector3.zero;
    //
    public Weapon newWeaponTest; //VestInventory의 WeaponOnHand가 바뀔때 관련함수 호출
    public WeaponData currentWeaponData;

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

        zoomAction = playerInput.AddAction("Fire2", binding: "<Mouse>/rightButton");

        fireAction.started += OnFireStarted;
        fireAction.performed += OnFirePerformed;
        fireAction.canceled += OnFireCanceled;
        zoomAction.started += OnZoomStarted;
        zoomAction.canceled += OnZoomCanceled;
    }

    private void OnEnable()
    {
        fireAction.Enable();
        zoomAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
        zoomAction.Disable();
    }

    private void Update()
    {
        /* ① 마우스 월드 좌표 한 번만 계산 (z=0 평면) */
        mouseWorld = GetMouseWorldOnPlane(0f);

        /* ② 팔 회전 & 캐릭터 플립 (좌표 캐시 사용) */
        RotateArmToMouse();
        FlipCharacter();


        /* ③ 사격 체크 */
        if (newWeaponTest == null || currentWeaponData == null) return;

        if (triggered && Time.time >= lastFireTime + currentWeaponData.fireRate)
        {
            TryFire();                          // Semi‑Auto, Full‑Auto 모두 여기서 처리
        }

        /* 🔹 Semi‑Auto일 땐 트리거를 즉시 해제해 중복 발사 방지 */
        if (currentWeaponData.fireMode != FireMode.FullAuto)
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
    {/*
        if (currentWeaponData != null && currentWeaponData.fireMode == FireMode.SemiAuto)
        {
            //TryFire();
            //triggered = false; // 한 번만 발사하도록 설정
        }
        */
    }

    // 🔹 마우스 클릭 해제 (발사 중지)
    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        triggered = false;
    }

    private void TryFire()
    {
        if (currentWeaponData == null || !newWeaponTest.isChamber)
        {
            triggered = false;
            return;
        }

        lastFireTime = Time.time;

        // 🔹 발사 성공 시 효과음 재생
        if (currentWeaponData.attackClip != null)
        {
            audioSource.PlayOneShot(currentWeaponData.attackClip);
        }

        for (int i = 0; i < currentWeaponData.pelletCount; i++)
        {
            FireProjectile();
        }

        newWeaponTest.PullReceiver();

        
    }
    private void FireProjectile()
    {
        // 🔹 마우스 위치를 가져와 정규화된 방향 벡터 계산
        Vector2 fireDirection = (mouseWorld - weaponPivotTransform.position).normalized;

        // 🔹 새로운 발사 위치 계산 (마우스 방향으로 barrelLength 만큼 떨어진 위치)
        Vector3 firePosition = weaponPivotTransform.position + (Vector3)(fireDirection * currentWeaponData.barrelLength);

        // 🔹 분산도 적용
        float uncertainty = Random.Range(-currentWeaponData.dispersion, currentWeaponData.dispersion);
        Quaternion rotation = Quaternion.Euler(0, 0, uncertainty);

        // 🔹 최종 발사 방향 (분산 적용)
        Vector2 finalDirection = rotation * fireDirection;

        // 🔹 오브젝트 풀에서 탄환 가져오기
        GameObject newProjectile = ProjectilePoolManager.Instance.GetProjectile(
            Faction.Friendly,  // 🔹 플레이어가 발사한 탄환
            2f,  // 삭제 시간
            currentWeaponData.projectileSpeed,  //발사체 속도
            currentWeaponData.damage,   //발사체 피해
            currentWeaponData.penetration,  //발사체 관통피해
            currentWeaponData.colliderSize  //발사체 충돌 크기 설정
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
        if ((mouseWorld - previousMousePosition).sqrMagnitude < 0.0001f) return;
        previousMousePosition = mouseWorld;

        Vector2 dir = (mouseWorld - armTransform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (playerToFlip.localScale.x < 0) angle -= 180f;
        armTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // 🔹 캐릭터 좌우 반전
    private void FlipCharacter()
    {
            Vector3 direction = mouseWorld - playerToFlip.position;
        float currentSign = Mathf.Sign(direction.x);
        if (currentSign != previousDirection)
        {
            previousDirection = currentSign;
            playerToFlip.localScale = new Vector3(currentSign, 1, 1);
        }
    }

    private void OnZoomStarted(InputAction.CallbackContext context)
    {
        if (UIManager.Instance.currentUI == null)
        {
            zoomObject.SetActive(true);
        }
    }

    private void OnZoomCanceled(InputAction.CallbackContext context)
    {
        zoomObject.SetActive(false);
    }

    public void SetWeapon(Weapon newWeapon)
    {
        if (newWeapon == null)
        {
            newWeaponTest = null;
            currentWeaponData = null;
        }
        newWeaponTest = newWeapon;
        
        currentWeaponData = newWeapon.data as WeaponData;

    }
    public void SetNoWeapon() {
        newWeaponTest = null;
        currentWeaponData = null;
    }
}
