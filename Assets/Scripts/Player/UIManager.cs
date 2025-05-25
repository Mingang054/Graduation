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
    public GameObject GameOverUI;       // 🔥 게임오버 UI 추가

    public GameObject vestInventoryUI;      // Vest Inventory
    public GameObject bagInventoryUI;       // Bag Inventory
    public GameObject pauseMenuUI;          // Pause Menu
    public GameObject radioUI;
    public GameObject raidUI;
    //
    public GameObject currentPrimaryUI;           // 현재 활성화된 UI
    public GameObject currentSecondaryUI;
    public GameObject current3rdUI;

    //임시
    public AmmoUpdater ammoUpdater;

    private InputAction inventoryAction;    // Tab 키
    private InputAction escapeAction;       // Esc 키

    public Slider tabHoldSlider;

    // 키 관련 플래그
    private bool isEscapeHandled = false;
    private bool isTabHold = false;
    private float tabHoldTime = 0f;

    private CancellationTokenSource tabHoldTaskTokenSource; // Task 취소 토큰

    [SerializeField]
    public AudioClip vestClip;
    public AudioClip bagClip;

    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("UI");
        inventoryAction = actionMap?.FindAction("Inventory");
        escapeAction = actionMap?.FindAction("Escape");

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

        inventoryAction.Enable();
        escapeAction.Enable();

        inventoryAction.started += OnInventoryStarted;
        inventoryAction.performed += OnInventoryPerformed;
        inventoryAction.canceled += OnInventoryCanceled;
        escapeAction.performed += OnEscapePerformed;
        escapeAction.canceled += OnEscapeCanceled;

        if (tabHoldSlider != null)
            tabHoldSlider.value = 0;
    }

    public void OnDisable()
    {
        inventoryAction.Disable();
        escapeAction.Disable();

        inventoryAction.started -= OnInventoryStarted;
        inventoryAction.performed -= OnInventoryPerformed;
        inventoryAction.canceled -= OnInventoryCanceled;
        escapeAction.performed -= OnEscapePerformed;
        escapeAction.canceled -= OnEscapeCanceled;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EnableBagInventory();
        DisableBagInventory();
        EnableVestInventory();
        DisableVestInventory();
        EnablePause();
        DisablePause();
    }

    public void OnInventoryStarted(InputAction.CallbackContext context)
    {
        isTabHold = true;
        tabHoldTime = 0f;
    }

    public void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        // 🔥 GameOverUI가 PrimaryUI인 경우에는 인벤토리 오픈 불가
        if (currentPrimaryUI == GameOverUI)
            return;

        if (current3rdUI == null)
        {
            if (currentSecondaryUI == null)
            {
                if (currentPrimaryUI == vestInventoryUI)
                {
                    if (context.interaction is HoldInteraction)
                    {
                        tabHoldTaskTokenSource?.Cancel();
                        tabHoldTaskTokenSource = new CancellationTokenSource();
                        TrackTabHoldProgress(tabHoldTaskTokenSource.Token).Forget();
                    }
                    else
                    {
                        DisableVestInventory();
                    }
                    return;
                }

                if (currentPrimaryUI != null) return;

                if (context.interaction is HoldInteraction)
                {
                    tabHoldTaskTokenSource?.Cancel();
                    tabHoldTaskTokenSource = new CancellationTokenSource();
                    TrackTabHoldProgress(tabHoldTaskTokenSource.Token).Forget();
                }
                else
                {
                    EnableVestInventory();
                }
            }
        }
    }

    public void OnInventoryCanceled(InputAction.CallbackContext context)
    {
        tabHoldTaskTokenSource?.Cancel();
        ResetTabHoldProgress();
    }

    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        if (context.performed && !isEscapeHandled)
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
            else if (currentPrimaryUI == pauseMenuUI)
            {
                DisablePause();
            }
            else if (currentPrimaryUI == null)
            {
                EnablePause();
            }
            else
            {
                DisableCurrentUI();
            }

            isEscapeHandled = true;
        }
    }
    private void OnEscapeCanceled(InputAction.CallbackContext context)
    {
        isEscapeHandled = false;
    }

    public void EnableVestInventory()
    {
        if (currentPrimaryUI == GameOverUI) return; // 🔥 GameOver시 금지
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
        }
        currentPrimaryUI = vestInventoryUI;
        vestInventoryUI.SetActive(true);
        Debug.Log("Vest Inventory Enabled");

        playerShooter.SetIsActing(true);
        AudioManager.Instance.PlaySFX(vestClip);
        cursorUI.SetUIAsUICursor();
    }
    public void EnableBagInventory()
    {
        if (currentPrimaryUI == GameOverUI) return; // 🔥 GameOver시 금지
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
        }
        currentPrimaryUI = bagInventoryUI;
        bagInventoryUI.SetActive(true);
        Debug.Log("Bag Inventory Enabled");

        playerShooter.SetIsActing(true);
        AudioManager.Instance.PlaySFX(bagClip);
        cursorUI.SetUIAsUICursor();
    }
    public void EnablePause()
    {
        if (currentPrimaryUI == GameOverUI) return; // 🔥 GameOver시 금지
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
        }
        currentPrimaryUI = pauseMenuUI;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Pause Menu Enabled");

        playerShooter.SetIsActing(true);
        cursorUI.SetUIAsUICursor();
    }

    public void EnableRadio()
    {
        if (currentPrimaryUI == GameOverUI) return; // 🔥 GameOver시 금지
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
        }
        currentPrimaryUI = radioUI;
        radioUI.SetActive(true);
        playerShooter.SetIsActing(true);
        cursorUI.SetUIAsUICursor();
    }

    public void EnableRaid()
    {
        if (currentPrimaryUI == GameOverUI) return; // 🔥 GameOver시 금지
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
        }
        currentPrimaryUI = raidUI;
        raidUI.SetActive(true);
        playerShooter.SetIsActing(true);
        cursorUI.SetUIAsUICursor();
    }

    /// <summary>
    /// 🔥 모든 UI(Primary/Secondary/3rd) 비활성화 후, GameOverUI를 Primary로 설정하고 활성화
    /// </summary>
    public void EnableGameOverUI()
    {
        // 모든 UI 비활성화 및 해제
        if (current3rdUI != null)
        {
            current3rdUI.SetActive(false);
            current3rdUI = null;
        }
        if (currentSecondaryUI != null)
        {
            currentSecondaryUI.SetActive(false);
            currentSecondaryUI = null;
        }
        if (currentPrimaryUI != null)
        {
            currentPrimaryUI.SetActive(false);
            currentPrimaryUI = null;
        }

        currentPrimaryUI = GameOverUI;
        GameOverUI.SetActive(true);

        playerShooter.SetIsActing(true);
        cursorUI.SetUIAsUICursor();
    }

    private void DisableCurrentUI()
    {
        if (currentPrimaryUI == pauseMenuUI)
        {
            DisablePause();
            currentPrimaryUI = null;
        }
        else if (currentPrimaryUI == GameOverUI)
        {
            // GameOver는 Disable 불가 (원하면 별도 로직 추가)
            return;
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
        cursorUI.SetUIAsAimCursor();
    }
    public void DisablePause()
    {
        if (currentPrimaryUI == pauseMenuUI)
        {
            currentPrimaryUI = null;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1;
            Debug.Log("Pause Menu Disabled");
        }
        playerShooter.SetIsActing(false);
        cursorUI.SetUIAsAimCursor();
    }
    public void DisableRadio()
    {
        if (currentPrimaryUI == radioUI)
        {
            currentPrimaryUI = null;
            radioUI.SetActive(false);
        }
        playerShooter.SetIsActing(false);
        cursorUI.SetUIAsAimCursor();
    }

    public void DisableRaid()
    {
        if (currentPrimaryUI == raidUI)
        {
            currentPrimaryUI = null;
            raidUI.SetActive(false);
        }
        playerShooter.SetIsActing(false);
        cursorUI.SetUIAsAimCursor();
    }

    private async UniTask TrackTabHoldProgress(CancellationToken cancellationToken)
    {
        while (isTabHold)
        {
            cancellationToken.ThrowIfCancellationRequested();
            tabHoldTime += Time.deltaTime;

            if (tabHoldSlider != null)
                tabHoldSlider.value = tabHoldTime;

            if (tabHoldTime >= 1f)
            {
                EnableBagInventory();
                ResetTabHoldProgress();
                return;
            }
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    private void ResetTabHoldProgress()
    {
        isTabHold = false;
        tabHoldTime = 0f;

        if (tabHoldSlider != null)
            tabHoldSlider.value = 0f;
    }
}
