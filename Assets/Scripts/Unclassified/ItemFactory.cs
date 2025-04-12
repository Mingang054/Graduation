using System.Collections.Generic;
using UnityEngine;

public static class ItemFactory
{
    public static ItemInstance CreateItem(ItemInitData initData)
    {
        // 1. 아이템 데이터 찾기
        ItemData itemData = ItemDB.Instance.GetItemDataByCode(initData.itemCode);
        if (itemData == null)
        {
            Debug.LogWarning($"아이템 코드 '{initData.itemCode}'에 해당하는 데이터를 찾을 수 없습니다.");
            return null;
        }

        // 2. 아이템 인스턴스 생성
        ItemInstance itemInstance = CreateItemInstance(itemData);
        if (itemInstance == null)
        {
            Debug.LogError($"'{initData.itemCode}' 아이템 인스턴스 생성 실패");
            return null;
        }

        // 3. 공통 속성 설정
        itemInstance.SetCount(initData.count);
        itemInstance.location = initData.location;

        // 4. 무기일 경우 추가 설정
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

        // 5. 방어구일 경우 내구도 설정
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

        Debug.LogError($"[ItemFactory] 알 수 없는 아이템 타입: {itemData.GetType()}");
        return null;
    }
}

public class ItemInitData
{
    public string itemCode;
    public int count;
    public Vector2Int location;

    // 무기,방어구일 경우에만 사용
    public float? durability;
    public bool? loaded;
    public bool? loadedIsAP;
    public List<bool> magazineData;
    public int? magCount;
}
