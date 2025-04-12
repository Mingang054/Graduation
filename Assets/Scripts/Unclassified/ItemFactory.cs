using System.Collections.Generic;
using UnityEngine;

public static class ItemFactory
{
    public static ItemInstance CreateItem(ItemInitData initData)
    {
        // 1. ������ ������ ã��
        ItemData itemData = ItemDB.Instance.GetItemDataByCode(initData.itemCode);
        if (itemData == null)
        {
            Debug.LogWarning($"������ �ڵ� '{initData.itemCode}'�� �ش��ϴ� �����͸� ã�� �� �����ϴ�.");
            return null;
        }

        // 2. ������ �ν��Ͻ� ����
        ItemInstance itemInstance = CreateItemInstance(itemData);
        if (itemInstance == null)
        {
            Debug.LogError($"'{initData.itemCode}' ������ �ν��Ͻ� ���� ����");
            return null;
        }

        // 3. ���� �Ӽ� ����
        itemInstance.SetCount(initData.count);
        itemInstance.location = initData.location;

        // 4. ������ ��� �߰� ����
        if (itemInstance is Weapon weapon)
        {
            weapon.SetDurability(initData.durability ?? 100f);
            weapon.isChamber = initData.loaded ?? false;
            //weapon.loadedIsAP = initData.loadedIsAP ?? false;

            weapon.magCount = 0;
            /*
            weapon.magStack = new Stack<bool>();
            weapon.magCount = initData.magCount ?? 0;
            if (initData.magazineData != null)
            {
                foreach (bool isAP in initData.magazineData)
                {
                    weapon.magStack.Push(isAP);
                }
                weapon.SyncMagAndCount();
            }
            */
        }

        // 5. ���� ��� ������ ����
        else if (itemInstance is Armor armor)
        {
            armor.SetDurability(initData.durability ?? 50f);
        }

        return itemInstance;
    }

    public static ItemInstance CreateItemInstance(ItemData itemData)
    {
        if (itemData is WeaponData weaponData) return new Weapon(weaponData);
        if (itemData is ArmorData armorData) return new Armor(armorData);
        if (itemData is ConsumableData consumableData) return new Consumable(consumableData);

        Debug.LogError($"[ItemFactory] �� �� ���� ������ Ÿ��: {itemData.GetType()}");
        return null;
    }
}

public class ItemInitData
{
    public string itemCode;
    public int count;
    public Vector2Int location;

    // ����,���� ��쿡�� ���
    public float? durability;
    public bool? loaded;
    public bool? loadedIsAP;
    public List<bool> magazineData;
    public int? magCount;
}
