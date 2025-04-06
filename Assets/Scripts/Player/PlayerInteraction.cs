using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange;
    public LayerMask interactableLayer;
    private GameObject currentTarget;
    [SerializeField] private Text targetName;

    private InputAction interactAction;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];

        interactableLayer = LayerMask.GetMask("Interactable");
    }

    private async void Start()
    {
        // ���� �� ���� ���� ����
        await StartDetectionLoop();
    }

    private void Update()
    {
        // �Է� üũ�� �� ������ ����
        if (interactAction.WasPerformedThisFrame() && currentTarget != null)
        {
            GetInteractable(currentTarget);
        }
    }



    private async Task StartDetectionLoop()
    {
        while (true)
        {
            DetectClosestInteractable();
            await Task.Delay(100); // 1�ʸ��� ����
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


        // �̸� ǥ�� ó��
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
        BagInventoryManager.Instance.SetOpponentItems(loot.lootItems);
        UIManager.Instance.EnableBagInventory();
    }

    private void InteractTrigger(InteractableTrigger trigger)
    {
        // Ʈ���� ���� �̺�Ʈ ���� ��
        Debug.Log("Interact with Trigger!");
    }

}
