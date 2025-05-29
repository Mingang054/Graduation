using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.FilePathAttribute;
#endif

using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField]
    private ItemType validItemType;
    [SerializeField]
    private EquipSlotType equipSlotType; //head, body, firstWeapon, ~ thirdWeapon
    public ItemInstanceUI equipedItem;
    bool isMySlot;

    public void OnPointerEnter(PointerEventData eventData)
    {

        // SlotUI 위를 마우스로 포인팅 시 현재 마우스가 위치한 Slot 갱신
        BagInventoryManager.Instance.currentPointedSlot = new Vector2Int(-2,-2);
        BagInventoryManager.Instance.currentPointedSlotIsMySlot = true;
        BagInventoryManager.Instance.currentPointedSlotIsEquip = true;
        //NewBagInventoryManager.Instance.currentPointedSlotType = equipSlotType;


        Debug.Log($"[SlotUI] OnPointerEnter. location: {BagInventoryManager.Instance.currentPointedSlot}");
        Debug.Log($"[SlotUI] OnPointerEnter. IsMySlot: {BagInventoryManager.Instance.currentPointedSlotIsMySlot}");

    }   

    public void OnPointerExit(PointerEventData eventData) {

        BagInventoryManager.Instance.currentPointedSlotIsEquip = false;
        //NewBagInventoryManager.Instance.currentPointedEquipSlot = GameObject.;

    }
    
    public bool GetIsMySlot()
    {
        // 
        return isMySlot;
    }
    public EquipSlotType GetEquipSlotType() {
        return equipSlotType;
    }

    public ItemType GetValidItemType()
    {
        return validItemType;
    }

}
