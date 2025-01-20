using UnityEngine;

public static class ItemFactory
{
    // 아이템 인스턴스 생성
    public static ItemInstance CreateItem(string itemCode)
    {
        ItemData itemData = ItemDB.Instance.GetItemDataByCode(itemCode);

        if (itemData == null)
        {
            Debug.LogWarning($"아이템 코드 '{itemCode}'에 해당하는 데이터를 찾을 수 없습니다.");
            return null;
        }

        try
        {
            ItemInstance itemInstance = CreateItemInstance(itemData);
            Debug.Log($"아이템 생성 완료: {itemData.itemName}");
            return itemInstance;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"아이템 생성 실패: {ex.Message}");
            return null;
        }
    }

    // 아이템 인스턴스 세부 사항 설정
    public static void SetItemInstance(ItemInstance itemInstance, int count, Vector2Int location, int? durability = null, int? charges = null)
    {
        itemInstance.SetCount(count); // 수량 설정
        itemInstance.location = location; // 위치 설정

        // 아이템 유형에 따라 세부 속성 설정
        if (itemInstance is Weapon weapon)
        {
            weapon.SetDurability(durability ?? 100); // 내구도 설정
        }
        else if (itemInstance is Armor armor)
        {
            armor.SetDurability(durability  ?? 50); // 내구도 설정
        }
    }

    // 기본 아이템 인스턴스 생성
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
            Debug.LogError($"알 수 없는 아이템 유형: {itemData.GetType()}");
            return null;
        }
    }

}