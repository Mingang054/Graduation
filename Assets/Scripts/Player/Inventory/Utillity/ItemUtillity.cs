using System.Collections.Generic;
using UnityEngine;

public static class ItemUtility
{
    public static void FreeItemSlots(ItemInstance itemInstance, Dictionary<Vector2Int, Slot> slots)
    {
        foreach (Vector2Int position in InventoryUtility.GetSlotPositions(itemInstance.location, itemInstance.data.size, new Vector2Int()))
        {
            if (slots.TryGetValue(position, out var slot))
                slot.SetOccupied(false);
        }
    }

    public static void RemoveItemFromList(ItemInstance itemInstance, List<ItemInstance> itemList)
    {
        itemList.Remove(itemInstance);
    }

    public static void DestroyItemUI(ItemInstance itemInstance)
    {
        //if (itemInstance.UIReference != null)
        //    GameObject.Destroy(itemInstance.UIReference.gameObject);
    }
}
