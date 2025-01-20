using UnityEngine;

public static class ItemFactory
{
    // ������ �ν��Ͻ� ����
    public static ItemInstance CreateItem(string itemCode)
    {
        ItemData itemData = ItemDB.Instance.GetItemDataByCode(itemCode);

        if (itemData == null)
        {
            Debug.LogWarning($"������ �ڵ� '{itemCode}'�� �ش��ϴ� �����͸� ã�� �� �����ϴ�.");
            return null;
        }

        try
        {
            ItemInstance itemInstance = CreateItemInstance(itemData);
            Debug.Log($"������ ���� �Ϸ�: {itemData.itemName}");
            return itemInstance;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"������ ���� ����: {ex.Message}");
            return null;
        }
    }

    // ������ �ν��Ͻ� ���� ���� ����
    public static void SetItemInstance(ItemInstance itemInstance, int count, Vector2Int location, int? durability = null, int? charges = null)
    {
        itemInstance.SetCount(count); // ���� ����
        itemInstance.location = location; // ��ġ ����

        // ������ ������ ���� ���� �Ӽ� ����
        if (itemInstance is Weapon weapon)
        {
            weapon.SetDurability(durability ?? 100); // ������ ����
        }
        else if (itemInstance is Armor armor)
        {
            armor.SetDurability(durability  ?? 50); // ������ ����
        }
    }

    // �⺻ ������ �ν��Ͻ� ����
    public static ItemInstance CreateItemInstance(ItemData itemData)
    {
        if (itemData is WeaponData weaponData)
        {
            return new Weapon(weaponData);
        }
        else if (itemData is ArmorData armorData)
        {
            return new Armor(armorData);
        }
        else if (itemData is ConsumableData consumableData)
        {
            return new Consumable(consumableData);
        }
        else
        {
            Debug.LogError($"�� �� ���� ������ ����: {itemData.GetType()}");
            return null;
        }
    }

}