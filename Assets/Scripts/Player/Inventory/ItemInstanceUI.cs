using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInstanceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    public Image itemImage;
    public Text itemCountText;

    private ItemInstance itemInstance;
    private RectTransform rectTransform;
    private Canvas canvas;

    // 드래그 전 상태
    private Transform originalParent;
    private Vector2Int originLocation;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
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
        Debug.Log("UpdateUI.좌표: " + itemInstance?.location);
    }

    public void UpdatePosition(Vector2Int location)
    {
        if (rectTransform == null) return;
        // 1칸=96px, (1,1) => anchoredPosition=(0,0) 방식
        rectTransform.anchoredPosition =
            new Vector2(location.x * 96 - 96, -location.y * 96 + 96);
    }

    public void UpdateSize()
    {
        if (rectTransform == null || itemInstance == null) return;
        rectTransform.sizeDelta =
            new Vector2(itemInstance.data.size.x * 96, itemInstance.data.size.y * 96);
    }

    //============ 드래그 & 드롭 기능 ============//

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[OnBeginDrag] 드래그 시작: {itemInstance.data.itemName} (현재 위치: {itemInstance.location})");

        // 1) 드래그 전 상태 저장
        originalParent = transform.parent;
        originLocation = itemInstance.location; // 이 위치로 복귀할 수도 있음
        originalPosition = rectTransform.anchoredPosition;

        // 2) 아이템을 임시로 슬롯에서 해제한다고 표시(-1,-1)
        itemInstance.location = new Vector2Int(-1, -1);

        // 3-1) UI의 이미지 위치를 마우스 위치로 정렬
        rectTransform.pivot = new Vector2(1f, 0f);
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out globalMousePos))
        {
            // 오프셋 적용: (48f, -48f)
            Vector3 offset = new Vector3(48f, -48f, 0f);
            rectTransform.position = globalMousePos + offset;
        }
        

        // 4) 최상위 Canvas로 이동
        rectTransform.SetParent(canvas.transform, true);

        // 5) 드래그 중에는 RaycastTarget 막아서 SlotUI가 OnPointerEnter 등 받을 수 있게
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 기존처럼 delta 기반으로 UI 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //Debug.Log($"[OnDrag] 드래그 중... 마우스 위치: {eventData.position}, " +
        //          $"UI 위치: {rectTransform.anchoredPosition}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[OnEndDrag] 드래그 종료: {itemInstance.data.itemName}");

        // 다시 RaycastTarget 활성화


        // 1) 마우스가 마지막으로 Hover한 슬롯 기준 좌표 계산
        Vector2Int targetSlotLocation = CaculateTargetSlot();
        bool isTargetMySlot = NewBagInventoryManager.Instance.currentPointedSlotIsMySlot;
        Debug.Log($"[OnEndDrag] targetSlotLocation={targetSlotLocation}, isMySlot={isTargetMySlot}");

        // 2) 배치 가능 여부 검사
        if (CanPlaceItem(targetSlotLocation, isTargetMySlot))
        {
            // 2-1) 배치 성공 로직
            // 실제로 Occupy 시키고, 인벤토리 리스트에 등록하는 과정을 수행
            PlaceItem(targetSlotLocation, isTargetMySlot);

            Debug.Log($"[OnEndDrag] 아이템 '{itemInstance.data.itemName}' 배치 성공!");
        }
        else
        {
            // 2-2) 배치 불가능 → 원래 상태로 복귀
            itemInstance.location = originLocation; // 아이템 데이터 위치 복구
            rectTransform.SetParent(originalParent, true);
            rectTransform.anchoredPosition = originalPosition;
            Debug.Log($"[OnEndDrag] 배치 실패 → 원래 위치({originLocation})로 복귀");
        }

        //
        canvasGroup.blocksRaycasts = true;
        rectTransform.pivot = new Vector2(0f, 1f);
        UpdateUI();
    }

    //============ 새 계산 로직 ============//

    /// <summary>
    /// 마우스가 마지막으로 Hover한 슬롯(currentPointedSlot)에
    /// 아이템의 실제 좌측상단이 들어갈 좌표를 계산
    /// ex) size=(2,2) 이면 currentPointedSlot-(1,1) 
    /// </summary>
    private Vector2Int CaculateTargetSlot()
    {
        var manager = NewBagInventoryManager.Instance;
        Vector2Int hoveredSlot = manager.currentPointedSlot;

        // 아이템 크기가 (3,2)라면, 우측하단이 hoveredSlot 위치에 닿았을 때
        // 좌측상단은 (3-1, 2-1)만큼 왼쪽/위로 보정
        Vector2Int finalPos = hoveredSlot - (itemInstance.data.size - new Vector2Int(1, 1));

        Debug.Log($"[CaculateTargetSlot] hovered={hoveredSlot}, finalPos={finalPos}");
        return finalPos;
    }




    /// <summary>
    /// 실제로 해당 targetPosition에 배치할 수 있는지 검사만 (점유 안함)
    /// </summary>
    private bool CanPlaceItem(Vector2Int targetPosition, bool isTargetMySlot)
    {
        var manager = NewBagInventoryManager.Instance;

        // 1) 어느 인벤토리로 갈지에 따라 딕셔너리와 사이즈 결정
        Dictionary<Vector2Int, Slot> targetSlots;
        if (isTargetMySlot)
        {
            targetSlots = manager.mySlots;
        }
        else
        {targetSlots = manager.opponentSlots;
        }

        Vector2Int inventorySize;
        if (isTargetMySlot)
        {
            inventorySize = manager.myInventoryVector;
        }
        else
        {
            inventorySize = manager.opponentInventoryVector;
        }

        // 2) ValidSlots로 검사
        return manager.ValidSlots(targetPosition, itemInstance.data.size, targetSlots, inventorySize);
    }

    /// <summary>
    /// 배치 가능하다면, 실제로 Occupy + 인벤토리 리스트 이동 처리
    /// (현재 내/상대 인벤토리 중 어느 리스트에 이 아이템이 있었는지 파악 후 이동)
    /// 비효올적이지만 자주하는 연산 아님
    /// </summary>
    private void PlaceItem(Vector2Int targetPosition, bool isTargetMySlot)
    {
        var manager = NewBagInventoryManager.Instance;

        // (A) 기존 인벤토리에서 이 아이템 제거 (if needed)
        if (manager.myItems.Contains(itemInstance))
        {
            manager.myItems.Remove(itemInstance);
            // 필요시 FreeItemSlots(itemInstance) 등으로 해제 가능
            manager.FreeItemSlots(itemInstance);
        }
        else if (manager.opponentItems != null && manager.opponentItems.Contains(itemInstance))
        {
            manager.opponentItems.Remove(itemInstance);
            // manager.FreeItemSlots(itemInstance) etc..
        }

        // (B) PlaceItemInSlot (혹은 AddItemToMyInventory, AddItemToOpponentInventory 등) 활용
        if (isTargetMySlot)
        {
            // 예: manager.PlaceItemInSlot(itemInstance, targetPosition, manager.mySlots, manager.myItems, manager.myInventoryVector)
            //     or manager.AddItemToMyInventoryAtPosition(...)
            manager.PlaceItemInSlot(
                itemInstance,
                targetPosition,
                manager.mySlots,
                manager.myItems,
                manager.myInventoryVector
            );

            // 부모도 myInventoryGrid로
            rectTransform.SetParent(manager.myInventory, true);
        }
        else
        {
            // 상대 인벤토리
            manager.PlaceItemInSlot(
                itemInstance,
                targetPosition,
                manager.opponentSlots,
                manager.opponentItems,
                manager.opponentInventoryVector
            );
            rectTransform.SetParent(manager.opponentInventory, true);
        }

        // (C) UI 갱신
        itemInstance.location = targetPosition;
        UpdatePosition(itemInstance.location);
        UpdateUI();
    }


    /// <summary>
    /// 특정 딕셔너리 내에서 (location ~ location+size-1) 영역을 Occupied 상태로 세팅
    /// </summary>
    private void OccupySlotRange(Dictionary<Vector2Int, Slot> slots,
                                 Vector2Int location,
                                 Vector2Int size,
                                 bool state)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (slots.TryGetValue(pos, out Slot slot))
                {
                    slot.SetOccupied(state);
                }
            }
        }
    }

    private void OnDisable()
    {
        // 드래그 도중 UI가 꺼지면 Raycast 복원
        itemImage.raycastTarget = true;
    }
}
