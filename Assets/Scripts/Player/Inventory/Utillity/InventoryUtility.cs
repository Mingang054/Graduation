using System.Collections.Generic;
using UnityEngine;

public static class InventoryUtility
{
    public static Vector2Int? FindFirstAvailableSlot(Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
    {
        for (int y = 1; y <= inventorySize.y; y++)
        {
            for (int x = 1; x <= inventorySize.x; x++)
            {
                Vector2Int location = new Vector2Int(x, y);
                if (ValidSlots(location, size, slots, inventorySize))
                    return location;
            }
        }
        return null;
    }

    public static bool ValidSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
    {
        foreach (Vector2Int position in GetSlotPositions(location, size, inventorySize))
        {
            if (!slots.TryGetValue(position, out var slot) || slot.GetOccupied())
                return false;
        }
        return true;
    }

    public static IEnumerable<Vector2Int> GetSlotPositions(Vector2Int location, Vector2Int size, Vector2Int inventorySize)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                if (x > inventorySize.x || y > inventorySize.y) continue;
                yield return new Vector2Int(x, y);
            }
        }
    }
}
