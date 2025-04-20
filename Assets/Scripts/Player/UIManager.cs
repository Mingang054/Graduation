using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Threading;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public PlayerShooter playerShooter;


    // Input Action Asset 
    public InputActionAsset inputActions;
    //
    public CursorUI cursorUI;
    public Image cursorImage;

    // UI GameObjects
    public GameObject vestInventoryUI;      // Vest Inventory
    public GameObject bagInventoryUI;       // Bag Inventory
    public GameObject pauseMenuUI;          // Pause Menu
    //
    public GameObject currentPrimaryUI;           // 현재 활성화된 UI

    // 세부 UI
    public GameObject currentSecondaryUI;
    public GameObject current3rdUI;


    //임시
    public AmmoUpdater ammoUpdater;

    private InputAction inventoryAction;    // Tab 키
    private InputAction escapeAction;       // Esc 키

    //
    public Slider tabHoldSlider;

    // 키 관련 플래그
    private bool isEscapeHandled = false;   // Escape 키 처리 플래그
    //private bool isTabPressed = false;      // 미사용
    private bool isTabHold = false;

    private float tabHoldTime = 0f;

    private CancellationTokenSource tabHoldTaskTokenSource; // Task 취소 토큰

    [SerializeField]
    public AudioClip vestClip;
    public AudioClip bagClip;



    private void OnEnable()
    {
        // UI ActionMap 할당
        var actionMap = inputActions.FindActionMap("UI");

        // UI 관련 Action 할당
        inventoryAction = actionMap?.FindAction("Inventory");
        escapeAction = actionMap?.FindAction("Escape");
        

        // ActionMap 및 Action 할당 에러 (디버그)
        if (actionMap == null)
        {
            Debug.LogError("UI ActionMap Missing");
            return;
        }
        if (inventoryAction == null)
        {
            Debug.LogError("Action 'Inventory' not found in Action Map 'UI'!");
            return;
        }
        if (escapeAction == null)
        {
            Debug.LogError("Action 'Escape' not found in Action Map 'UI'!");
            return;
        }

        // Input Actions 활성화
        inventoryAction.Enable();
        escapeAction.Enable();

        // 이벤트 핸들러 연결
        inventoryAction.started += OnInventoryStarted;
        inventoryAction.performed += OnInventoryPerformed;
        inventoryAction.canceled += OnInventoryCanceled;
        escapeAction.performed += OnEscapePerformed;
        escapeAction.canceled += OnEscapeCanceled;

        //TabHoldSlider 초기화
        if (tabHoldSlider != null)
        {
            tabHoldSlider.value = 0;
        }

    }
    private void OnDisable()
    {
        // Input Actions 비활성화
        inventoryAction.Disable();
        escapeAction.Disable();

        // 이벤트 핸들러 연결 해제
        inventoryAction.started -= OnInventoryStarted;
        inventoryAction.performed -= OnInventoryPerformed;
        inventoryAction.canceled -= OnInventoryCanceled;
        escapeAction.performed -= OnEscapePerformed;
        escapeAction.canceled -= OnEscapeCanceled;
    }

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지하려면 활성화
    
    }
    private void Start()
    {   
        //인벤토리 초기화실행
        EnableBagInventory();
        DisableBagInventory();
        EnableVestInventory();
        DisableVestInventory();
        EnablePause();
        DisablePause();
    }

    public void OnInventoryStarted(InputAction.CallbackContext context)
    {
        // Hold 여부 초기화
        isTabHold = true;
        tabHoldTime = 0f; // 초기화

    }

    public void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        if (current3rdUI == null)
        {
            if (currentSecondaryUI == null)
            {
                // 현재 UI가 Vest Inventory인 경우
                if (currentPrimaryUI == vestInventoryUI)
                {
                    if (context.interaction is HoldInteraction) // Hold 동작
                    {
                        // 기존 Task 취소
                        tabHoldTaskTokenSource?.Cancel();
                        tabHoldTaskTokenSource = new CancellationTokenSource();

                        // TrackTabHoldProgress 호출
                        TrackTabHoldProgress(tabHoldTaskTokenSource.Token).Forget();
                    }
                    else // Press 동작
                    {
                        DisableVestInventory(); // Vest Inventory를 비활성화
                    }
                    return;
                }

                // 현재 활성화된 UI가 없거나 다른 경우
                if (currentPrimaryUI != null) return;

                if (context.interaction is HoldInteraction) // Hold 동작
                {
                    // 기존 Task 취소
                    tabHoldTaskTokenSource?.Cancel();
                    tabHoldTaskTokenSource = new CancellationTokenSource();

                    // TrackTabHoldProgress 호출
                    TrackTabHoldProgress(tabHoldTaskTokenSource.Token).Forget();
                }
                else // Press 동작
                {
                    EnableVestInventory();
                }
            }
        }


    }

    public void OnInventoryCanceled(InputAction.CallbackContext context)
    {
        // Task 중단
        tabHoldTaskTokenSource?.Cancel();
        ResetTabHoldProgress(); // 슬라이더 초기화
    }

    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        if (context.performed && !isEscapeHandled) // Escape 키가 눌리고 아직 처리되지 않은 경우
        {   
            if (current3rdUI != null)
            {
                current3rdUI.SetActive(false);
                current3rdUI = null;
            }
            else if (currentSecondaryUI != null)
            {
                currentSecondaryUI.SetActive(false);
                currentSecondaryUI = null;
            }
            else if (currentPrimaryUI == pauseMenuUI) // Pause Menu가 활성화된 경우 비활성화
            {
                DisablePause();
            }
            else if (currentPrimaryUI == null) // 다른 UI가 없는 경우 Pause Menu 활성화
            {
                EnablePause();
            }
            else // 다른 UI가 활성화된 경우 비활성화
            {
                DisableCurrentUI();
            }

            isEscapeHandled = true; // Escape 키 처리 완료
        }
    }
    private void OnEscapeCanceled(InputAction.CallbackContext context)
    {
        isEscapeHandled = false;
    }
    public void EnableVestInventory()
    {
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false); // 기존 UI 비활성화
        }
        currentPrimaryUI = vestInventoryUI;
        vestInventoryUI.SetActive(true); // 새로운 UI 활성화
        Debug.Log("Vest Inventory Enabled");

        playerShooter.SetIsActing(true);

        AudioManager.Instance.PlaySFX(vestClip);
        //UI
        cursorUI.SetUIAsUICursor();
        
    }
    public void EnableBagInventory()
    {
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false); // 🔴 기존 UI 비활성화
        }
        currentPrimaryUI = bagInventoryUI;
        bagInventoryUI.SetActive(true); // 새로운 UI 활성화
        Debug.Log("Bag Inventory Enabled");

        playerShooter.SetIsActing(true);

        AudioManager.Instance.PlaySFX(bagClip);
        //UI
        cursorUI.SetUIAsUICursor();
    }
    public void EnablePause()
    {
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false); // 기존 UI 비활성화
        }
        currentPrimaryUI = pauseMenuUI;
        pauseMenuUI.SetActive(true); // 새로운 UI 활성화
        Time.timeScale = 0; // 게임 시간 정지
        Debug.Log("Pause Menu Enabled");

        playerShooter.SetIsActing(true);

        //UI
        cursorUI.SetUIAsUICursor();
    }
    private void DisableCurrentUI()
    {
        if (currentPrimaryUI == pauseMenuUI)
        {
            DisablePause();
            currentPrimaryUI = null;
        }
        else if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
            currentPrimaryUI = null;
        }
        if (BagInventoryManager.Instance.opponentItems != null)
        {
            BagInventoryManager.Instance.ResetOpponentItems();

        }

        playerShooter.SetIsActing(false);

        //UI
        cursorUI.SetUIAsAimCursor();
    }
    private void DisableVestInventory()
    {
        if (currentPrimaryUI == vestInventoryUI)
        {
            currentPrimaryUI = null;
            vestInventoryUI.SetActive(false);
            Debug.Log("Vest Inventory Disabled");
        }

        playerShooter.SetIsActing(false);

        //UI
        cursorUI.SetUIAsAimCursor();
    }
    private void DisableBagInventory()
    {
        if (currentPrimaryUI == bagInventoryUI)
        {
            currentPrimaryUI = null;
            bagInventoryUI.SetActive(false);
            Debug.Log("Bag Inventory Disabled");
        }

        playerShooter.SetIsActing(false);

        //UI
        cursorUI.SetUIAsAimCursor();
    }
    private void DisablePause()
    {
        if (currentPrimaryUI == pauseMenuUI)
        {
            currentPrimaryUI = null;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1; // 게임 시간 재개
            Debug.Log("Pause Menu Disabled");
        }

        playerShooter.SetIsActing(false);

        //UI
        cursorUI.SetUIAsAimCursor();
    }

    private async UniTask TrackTabHoldProgress(CancellationToken cancellationToken)
    {
        while (isTabHold)
        {
            //Task 중단 체크
            cancellationToken.ThrowIfCancellationRequested();

            tabHoldTime += Time.deltaTime;

            if (tabHoldSlider != null)
            {
                tabHoldSlider.value = tabHoldTime; // 슬라이더 업데이트
            }

            // Bag Inventory를 열기 위한 최대 홀드 시간에 도달
            if (tabHoldTime >= 1f)
            {
                EnableBagInventory();
                ResetTabHoldProgress(); // 슬라이더 초기화
                return;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); // 다음 프레임 대기
        }
    }

    private void ResetTabHoldProgress()
    {
        isTabHold = false; // Hold 상태 해제
        tabHoldTime = 0f;     // Hold 시간 초기화

        if (tabHoldSlider != null)
        {
            tabHoldSlider.value = 0f; // 슬라이더 초기화
        }

    }


    
}


