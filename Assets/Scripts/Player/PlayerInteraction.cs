using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Cysharp.Threading.Tasks;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange;
    public LayerMask interactableLayer;
    private GameObject currentTarget;
    [SerializeField] private TMP_Text targetName;
    [SerializeField] private Text legacyTargetName;         //임시용
    private InputAction interactAction;

    private UniTask detectionLoop = UniTask.CompletedTask;

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
        }
        else if (interactable is InteractableTrigger trigger)
        {
            InteractTrigger(trigger);
        }
        else
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
        //
    }
    private void OnDisable()
    {
        detectionLoop = UniTask.CompletedTask;
    }
}
