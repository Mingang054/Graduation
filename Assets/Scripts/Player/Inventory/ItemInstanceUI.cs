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
        if (itemInstance.currentEquipSlotType == EquipSlotType.none)
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
        else
        {
            if (transform.parent == null) return;

            RectTransform parentRect = transform.parent as RectTransform;
            RectTransform selfRect = transform as RectTransform;
            if (parentRect == null || selfRect == null) return;

            selfRect.anchorMin = new Vector2(0.5f, 0.5f);
            selfRect.anchorMax = new Vector2(0.5f, 0.5f);
            selfRect.pivot = new Vector2(0.5f, 0.5f);
            selfRect.anchoredPosition = Vector2.zero;
        }
    }

    public void UpdatePosition(Vector2Int location)
    {
        if (rectTransform == null) return;
        rectTransform.anchoredPosition = new Vector2(location.x * 96 - 96, -location.y * 96 + 96);
    }

    public void UpdateSize()
    {
        if (rectTransform == null || itemInstance == null) return;
        rectTransform.sizeDelta = new Vector2(itemInstance.data.size.x * 96, itemInstance.data.size.y * 96);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originLocation = itemInstance.location;
        originalPosition = rectTransform.anchoredPosition;

        rectTransform.pivot = new Vector2(1f, 0f);
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out Vector3 globalMousePos))
        {
            Vector3 offset = new Vector3(48f, -48f, 0f);
            rectTransform.position = globalMousePos + offset;
        }

        rectTransform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject foundObject = eventData.pointerCurrentRaycast.gameObject;
        canvasGroup.blocksRaycasts = true;
        rectTransform.pivot = new Vector2(0f, 1f);

        if (foundObject != null)
        {
            SlotUI foundSlotUI = foundObject.GetComponent<SlotUI>();
            if (foundSlotUI != null)
            {
                NewBagInventoryManager.Instance.currentPointedSlot = foundSlotUI.location;
                NewBagInventoryManager.Instance.currentPointedSlotIsMySlot = foundSlotUI.isMySlot;

                Vector2Int targetSlotLocation = CaculateTargetSlot();
                bool isTargetMySlot = NewBagInventoryManager.Instance.currentPointedSlotIsMySlot;

                if (itemInstance.currentEquipSlotUI == null)
                    NewBagInventoryManager.Instance.FreeItemSlots(itemInstance);

                if (CanPlaceItem(targetSlotLocation, isTargetMySlot))
                {
                    PlaceItem(targetSlotLocation, isTargetMySlot);
                    itemInstance.currentEquipSlotType = EquipSlotType.none;
                }
                else
                {
                    if (itemInstance.currentEquipSlotUI == null)
                    {
                        var manager = NewBagInventoryManager.Instance;
                        if (manager.myItems.Contains(itemInstance))
                            manager.OccupySlots(originLocation, itemInstance.data.size, manager.mySlots);
                        else if (manager.opponentItems.Contains(itemInstance))
                            manager.OccupySlots(originLocation, itemInstance.data.size, manager.opponentSlots);
                    }
                    itemInstance.location = originLocation;
                    rectTransform.SetParent(originalParent, true);
                    rectTransform.anchoredPosition = originalPosition;
                }

                UpdateUI();
                return;
            }

            EquipmentSlotUI foundEquipUI = foundObject.GetComponent<EquipmentSlotUI>();
            if (foundEquipUI != null)
            {
                NewBagInventoryManager.Instance.currentPointedEquipSlot = foundEquipUI;
                NewBagInventoryManager.Instance.currentPointedSlotIsEquip = true;
                NewBagInventoryManager.Instance.currentPointedSlotIsMySlot = true;

                if (CanEquipItem(foundEquipUI))
                {
                    EquipItem(foundEquipUI);
                }
                else
                {
                    itemInstance.location = originLocation;
                    rectTransform.SetParent(originalParent, true);
                    rectTransform.anchoredPosition = originalPosition;
                }

                NewBagInventoryManager.Instance.currentPointedEquipSlot = null;
                NewBagInventoryManager.Instance.currentPointedSlotIsEquip = false;
                UpdateUI();
                return;
            }
        }

        rectTransform.SetParent(originalParent, true);
        UpdateUI();
    }

    private Vector2Int CaculateTargetSlot()
    {
        Vector2Int hoveredSlot = NewBagInventoryManager.Instance.currentPointedSlot;
        return hoveredSlot - (itemInstance.data.size - new Vector2Int(1, 1));
    }

    private bool CanEquipItem(EquipmentSlotUI slot)
    {
        if (slot.equipedItem != null) return false;
        if (slot.GetValidItemType() != itemInstance.data.itemType) return false;

        var type = slot.GetEquipSlotType();

        if (itemInstance.data is WeaponData weapon)
        {
            if (type == EquipSlotType.thirdWeapon && weapon.category != WeaponCategory.Pistol)
                return false;
        }
        else if (itemInstance.data is ArmorData armor)
        {
            if ((type == EquipSlotType.head && armor.armorSlot != ArmorSlot.Head) ||
                (type == EquipSlotType.body && armor.armorSlot != ArmorSlot.Body))
                return false;
        }

        return true;
    }

    private void EquipItem(EquipmentSlotUI slot)
    {
        var manager = NewBagInventoryManager.Instance;

        if (manager.myItems.Contains(itemInstance))
        {
            manager.myItems.Remove(itemInstance);
            manager.FreeItemSlots(itemInstance);
        }
        else if (manager.opponentItems.Contains(itemInstance))
        {
            manager.opponentItems.Remove(itemInstance);
            manager.FreeItemSlots(itemInstance);
        }
        else if (itemInstance.currentEquipSlotType != EquipSlotType.none)
        {
            UnEquip();
        }

        rectTransform.SetParent(slot.transform, true);
        slot.equipedItem = this;
        itemInstance.currentEquipSlotUI = slot;
        itemInstance.currentEquipSlotType = slot.GetEquipSlotType();
    }

    private void UnEquip()
    {
        if (itemInstance.currentEquipSlotUI != null)
        {
            itemInstance.currentEquipSlotUI.equipedItem = null;
            itemInstance.currentEquipSlotUI = null;
            itemInstance.currentEquipSlotType = EquipSlotType.none;
        }
    }

    private bool CanPlaceItem(Vector2Int pos, bool isMySlot)
    {
        var manager = NewBagInventoryManager.Instance;
        var slots = isMySlot ? manager.mySlots : manager.opponentSlots;
        var size = isMySlot ? manager.myInventoryVector : manager.opponentInventoryVector;
        return manager.ValidSlots(pos, itemInstance.data.size, slots, size);
    }

    private void PlaceItem(Vector2Int pos, bool isMySlot)
    {
        var manager = NewBagInventoryManager.Instance;

        if (itemInstance.currentEquipSlotUI == null)
        {
            if (manager.myItems.Contains(itemInstance))
                manager.myItems.Remove(itemInstance);
            else if (manager.opponentItems.Contains(itemInstance))
                manager.opponentItems.Remove(itemInstance);
        }
        else
        {
            UnEquip();
        }

        if (isMySlot)
        {
            manager.PlaceItemInSlot(itemInstance, pos, manager.mySlots, manager.myItems, manager.myInventoryVector);
            rectTransform.SetParent(manager.myInventory, true);
        }
        else
        {
            manager.PlaceItemInSlot(itemInstance, pos, manager.opponentSlots, manager.opponentItems, manager.opponentInventoryVector);
            rectTransform.SetParent(manager.opponentInventory, true);
        }

        itemInstance.location = pos;
        UpdatePosition(pos);
        UpdateUI();
    }

    private void OnDisable()
    {
        itemImage.raycastTarget = true;
    }
}