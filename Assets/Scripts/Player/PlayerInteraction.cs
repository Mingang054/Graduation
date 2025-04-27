using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange;
    public LayerMask interactableLayer;
    private GameObject currentTarget;
    [SerializeField] private TMP_Text targetName;
    [SerializeField] private Text legacyTargetName;         //임시용
    private InputAction interactAction;

    private Time extractTime;
    private float extractCounter;

    private UniTask detectionLoop = UniTask.CompletedTask;


    //Extract 용
    public GameObject extractionUI;
    public TMP_Text extractionText;
    
    private UniTask extractionCounter = UniTask.CompletedTask;
    private CancellationTokenSource extractCTS;   // ⬅ 탈출용 CTS 하나만 관리
    private ExtractionPoint currentExtractPoint;  // 진행 중인 탈출구


    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];

        interactableLayer = LayerMask.GetMask("Interactable");
    }

    private async void Start()
    {
        // 시작 시 감지 루프 실행
        detectionLoop = StartDetectionLoop();
    }

    private void Update()
    {
        // 입력 체크는 매 프레임 수행
        if (interactAction.WasPerformedThisFrame() && currentTarget != null)
        {
            GetInteractable(currentTarget);
        }
    }


    private async UniTask StartDetectionLoop()
    {
        while (true)
        {

            DetectClosestInteractableLegacy();
            //DetectClosestInteractable();
            await Task.Delay(100); // 1초마다 감지
        }
    }


    private void DetectClosestInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        
        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (Collider2D hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closest = hit.gameObject;
                closestDistance = dist;
            }
        }
        
        currentTarget = closest;


        // 이름 표시 처리
        if (targetName != null)
        {
            if (currentTarget != null)
            {
                Interactable interactable = currentTarget.GetComponent<Interactable>();
                targetName.text = interactable != null ? interactable.interactionName : "";
            }
            else
            {
                targetName.text = "";
            }
        }

    }

    private void GetInteractable(GameObject target)
    {
        Interactable interactable = target.GetComponent<Interactable>();
        if (interactable == null) return;

        if (interactable is Loot loot)
        {
            InteractLoot(loot);
        }else if (interactable is ExtractionPoint extraction)
        {
            InteractExtraction(extraction);
        }
        else if (interactable is InteractableTrigger trigger)
        {
            InteractTrigger(trigger);
        }
        else if (interactable is InteractSellBox selBox)
        {
            selBox.SellBox();
        }
        {
            return;
        }
    }

    private void InteractLoot(Loot loot)
    {
        UIManager.Instance.EnableBagInventory();
        BagInventoryManager.Instance.SetOpponentItems(loot.lootItems);
    }

    private void InteractTrigger(InteractableTrigger trigger)
    {
        // 트리거 전용 이벤트 실행 등
        Debug.Log("Interact with Trigger!");
    }

    // 3) extraction 관련 시도, 동장, 취소


    private void InteractExtraction(ExtractionPoint e)
    {
        // ① 이미 진행 중이면 무시
        if (extractCTS != null && !extractCTS.IsCancellationRequested) return;

        // ② 탈출 가능 여부 확인
        if (!e.getAvailable())
        {
            Debug.Log("탈출 조건이 충족되지 않았습니다.");
            return;
        }

        // ③ 태스크 시작
        extractCTS = new CancellationTokenSource();
        currentExtractPoint = e;
        e.gameObject.SetActive(false);
        _ = ExtractionLoop(e, extractCTS.Token);        // fire-and-forget
        extractionUI.SetActive(true);
    }

   

    /* ───────── 루프 ───────── */
    private async UniTask ExtractionLoop(ExtractionPoint e, CancellationToken token)
    {
        float timer = 0f;
        Debug.Log($"[Extraction] {e.interactionName} 시작");

        try
        {
            while (timer < e.timeToExtract)
            {
                token.ThrowIfCancellationRequested();

                // ▶ 거리 체크
                float dist = Vector2.Distance(transform.position, e.transform.position);
                if (dist > e.extractDistance)
                {
                    Debug.Log("탈출 취소: 거리를 벗어났습니다.");
                    StopExtraction();
                    return;
                }

                timer += Time.deltaTime;        // 실시간(타임스케일 영향)
                extractionText.text = (e.timeToExtract - timer).ToString("F1");
                // TODO: UI 갱신(남은 시간 등)

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            // ▶ 완료
            Debug.Log("탈출 성공!");
            ConfirmExtraction();
            extractionUI.SetActive(false);
            extractionText.text = "";
        }
        catch (OperationCanceledException) { /* 취소 시 조용히 종료 */ }
    }


    /* ───────── 취소/확정 ───────── */
    public void StopExtraction()
    {
        if (extractCTS != null && !extractCTS.IsCancellationRequested)
            extractCTS.Cancel();

        // TODO: UI Reset
        currentExtractPoint.gameObject.SetActive(true);
        currentExtractPoint = null;
        extractionUI.SetActive(false);
    }

    private void ConfirmExtraction()
    {
        // 씬 전환 또는 클리어 처리
        Debug.Log(currentExtractPoint.interactionName);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("OutPost");
    }




private void DetectClosestInteractableLegacy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (Collider2D hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closest = hit.gameObject;
                closestDistance = dist;
            }
        }

        currentTarget = closest;


        // 이름 표시 처리
        if (legacyTargetName != null)
        {
            if (currentTarget != null)
            {
                Interactable interactable = currentTarget.GetComponent<Interactable>();
                legacyTargetName.text = interactable != null ? interactable.interactionName : "";
            }
            else
            {
                legacyTargetName.text = "";
            }
        }

    }


    private void OnEnable()
    {
        if (detectionLoop.Status != UniTaskStatus.Pending)
        {
            StartDetectionLoop();
        }
    }
    private void OnDisable()
    {
        detectionLoop = UniTask.CompletedTask;
        extractionCounter = UniTask.CompletedTask;
        if (currentExtractPoint!=null)
        StopExtraction();
    }
}
