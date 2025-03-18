using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInstanceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image itemImage;
    public Text itemCountText;
    private ItemInstance itemInstance;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (itemInstance != null)
        {
            UpdateUI();
            UpdateSize();
        }
    }

    public void Initialize(ItemInstance instance)
    {
        itemInstance = instance;
        UpdateUI();
        UpdatePosition(itemInstance.location);
        UpdateSize();
    }

    public void UpdateUI()
    {
        if (itemInstance != null)
        {
            UpdatePosition(itemInstance.location);
            UpdateSize();

            itemImage.sprite = itemInstance.data.itemSprite;
            itemImage.enabled = true;
            itemCountText.text = itemInstance.count > 1 ? itemInstance.count.ToString() : "";
        }
        else
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemCountText.text = "";
        }
    }

    public void UpdatePosition(Vector2Int location)
    {
        if (rectTransform == null) return;
        rectTransform.anchoredPosition = new Vector2(location.x * 96 - 96, -location.y * 96 + 96);
    }

    public void UpdateSize()
    {
        if (rectTransform == null) return;
        if (itemInstance != null)
        {
            rectTransform.sizeDelta = new Vector2(itemInstance.data.size.x * 96, itemInstance.data.size.y * 96);
        }
    }

    //--- 드래그 & 드롭 기능 ---//

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"드래그 시작: {itemInstance.data.itemName}");
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.SetParent(canvas.transform, true); // 캔버스 최상위로 이동
        itemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"드래그 종료: {itemInstance.data.itemName}");
        itemImage.raycastTarget = true;

        // 배치 가능한 슬롯 찾기
        SlotUI targetSlot = FindClosestSlot();
        if (targetSlot != null)
        {
            if (CanPlaceItem(targetSlot.location))
            {
                UpdatePosition(targetSlot.location);
                Debug.Log($"아이템 '{itemInstance.data.itemName}'을(를) {targetSlot.location}에 배치");
                return;
            }
        }

        // 배치 실패 시 원래 위치로 복귀
        rectTransform.anchoredPosition = originalPosition;
    }

    // 가장 가까운 슬롯 찾기
    private SlotUI FindClosestSlot()
    {
        SlotUI closestSlot = null;
        float minDistance = float.MaxValue;

        foreach (SlotUI slot in FindObjectsOfType<SlotUI>()) // 모든 슬롯을 검사
        {
            float distance = Vector2.Distance(rectTransform.anchoredPosition, slot.GetComponent<RectTransform>().anchoredPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSlot = slot;
            }
        }

        return closestSlot;
    }

    // 배치 가능 여부 검사
    private bool CanPlaceItem(Vector2Int position)
    {
        // 현재 아이템이 어느 인벤토리에 속해 있는지 확인 후 ValidSlots 호출
        if (NewBagInventoryManager.Instance.myItems.Contains(itemInstance))
        {
            return NewBagInventoryManager.Instance.ValidSlots(position, itemInstance.data.size, NewBagInventoryManager.Instance.mySlots, NewBagInventoryManager.Instance.myInventoryVector);
        }
        else if (NewBagInventoryManager.Instance.opponentItems != null && NewBagInventoryManager.Instance.opponentItems.Contains(itemInstance))
        {
            return NewBagInventoryManager.Instance.ValidSlots(position, itemInstance.data.size, NewBagInventoryManager.Instance.opponentSlots, NewBagInventoryManager.Instance.opponentInventoryVector);
        }

        return false;
    }

}
