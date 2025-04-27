using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SaveLoader : 모든 세이브 데이터를 한 파일로 묶어 저장/로드
/// </summary>
public static class SaveLoader
{
    /*──────────────── 경로 헬퍼 ────────────────*/
    private static readonly string DIR = Path.Combine(Application.persistentDataPath, "SaveData");
    private static string PathFor(string name) => Path.Combine(DIR, $"{name}.json");
    private static void EnsureDir() { if (!Directory.Exists(DIR)) Directory.CreateDirectory(DIR); }

    /*──────────────── 공개 API ────────────────*/
    /// <summary>2025-04-27_2005.json 처럼 시간 기반 파일로 저장</summary>
    public static void SaveWithTimestamp()
    {
        EnsureDir();
        GameSaveData gs = GatherCurrentState();
        string file = PathFor(DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
        File.WriteAllText(file, JsonUtility.ToJson(gs, true));
        Debug.Log($"✅ Save (timed) → {file}");
    }

    /// <summary>checkpoint.json 파일로 덮어쓰기 저장 (씬 이동용 임시 저장)</summary>
    public static void SaveCheckpoint()
    {
        EnsureDir();
        GameSaveData gs = GatherCurrentState();
        string file = PathFor("checkpoint");
        File.WriteAllText(file, JsonUtility.ToJson(gs, true));
        Debug.Log($"✅ Save (checkpoint) → {file}");
    }

    /// <summary>checkpoint.json 을 읽어서 복구</summary>
    public static void LoadCheckpoint()
    {
        string file = PathFor("checkpoint");
        if (!File.Exists(file)) { Debug.LogWarning("❗ checkpoint.json 없음"); return; }

        var gs = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(file));
        if (gs == null) { Debug.LogWarning("❗ 파싱 실패"); return; }

        ApplyToGame(gs);                              // 실제 게임 오브젝트에 반영(구현 필요)
        Debug.Log($"✅ Load (checkpoint) :  {gs.saveTime}");
    }

    /*──────────────── 현재 상태 → GameSaveData ────────────────*/
    private static GameSaveData GatherCurrentState()
    {
        var bag = BagInventoryManager.Instance;
        var vest = VestInventory.Instance;
        var ammo = AmmunitionManager.instance;
        var heal = HealItemManager.instance;
        var player = PlayerStatus.instance;

        var gs = new GameSaveData
        {
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            inventory = new InventorySaveData { myItems = new List<ItemSaveData>() },
            equipSlot = new EquipSaveData { equipSlots = new List<EquipSlotSaveData>() },
            healSlot = new HealSaveData { healItems = new ItemSaveData[heal.consumables.Length] },
            vestPlaceables = new VestPlaceableSaveList { placeables = new List<VestPlaceableSaveData>() },
            ammunition = new AmmunitionSaveData(),
            currentWeapon = new CurrentWeaponSaveData(),

            money = player != null ? player.money : 0f  // 🔥 돈 저장
        };

        /* ① 인벤토리 */
        foreach (var it in bag.myItems) gs.inventory.myItems.Add(ToItem(it));

        /* ② 장비 슬롯 */
        foreach (var ui in new[] {
                 bag.firstWeaponUI?.GetComponent<EquipmentSlotUI>(),
                 bag.secondWeaponUI?.GetComponent<EquipmentSlotUI>(),
                 bag.thirdWeaponUI?.GetComponent<EquipmentSlotUI>(),
                 bag.headArmorUI?.GetComponent<EquipmentSlotUI>(),
                 bag.bodyArmorUI?.GetComponent<EquipmentSlotUI>() })
        {
            if (ui == null) continue;
            gs.equipSlot.equipSlots.Add(new EquipSlotSaveData
            {
                slotType = ui.GetEquipSlotType(),
                item = ui.equipedItem ? ToItem(ui.equipedItem.itemInstance) : null
            });
        }

        /* ③ 힐 슬롯 */
        for (int i = 0; i < heal.consumables.Length; i++)
            if (heal.consumables[i] != null)
                gs.healSlot.healItems[i] = ToItem(heal.consumables[i]);

        /* ④ 베스트 슬롯 */
        foreach (var vp in new[] { vest.PLeft, vest.PCenter, vest.PRight,
                                   vest.SOne, vest.SLeft,  vest.SRight })
            if (vp)
                gs.vestPlaceables.placeables.Add(new VestPlaceableSaveData
                {
                    placeableType = vp.placeableType,
                    ammoType = vp.ammoType,
                    count = vp.count,
                    magAmmoCount = vp.magAmmoCount
                });

        /* ⑤ 탄약 */
        gs.ammunition = new AmmunitionSaveData
        {
            lightAmmo = ammo.lightAmmo,
            mediumAmmo = ammo.mediumAmmo,
            heavyAmmo = ammo.heavyAmmo,
            antiAmmo = ammo.antiAmmo,
            shellAmmo = ammo.shellAmmo,
            magnumAmmo = ammo.magnumAmmo,
            explosiveAmmo = ammo.explosiveAmmo
        };

        /* ⑥ 현재 들고 있던 무기 */
        var cw = vest.weaponOnHand?.currentWeapon;
        if (cw == bag.firstWeapon) gs.currentWeapon.currentUsingWeapon = EquipSlotType.firstWeapon;
        else if (cw == bag.secondWeapon) gs.currentWeapon.currentUsingWeapon = EquipSlotType.secondWeapon;
        else if (cw == bag.thirdWeapon) gs.currentWeapon.currentUsingWeapon = EquipSlotType.thirdWeapon;
        else gs.currentWeapon.currentUsingWeapon = EquipSlotType.none;

        return gs;
    }

    /*──────────────── TODO : 로드시 적용 ────────────────*/
    private static void ApplyToGame(GameSaveData data)
    {
        var bag = BagInventoryManager.Instance;
        var vest = VestInventory.Instance;
        var heal = HealItemManager.instance;
        var ammo = AmmunitionManager.instance;
        var player = PlayerStatus.instance;


        /* ① 인벤토리 복구 */
        var newItems = new List<ItemInstance>();
        foreach (var saveItem in data.inventory.myItems)
        {
            var item = ItemFactory.CreateItem(new ItemInitData
            {
                itemCode = saveItem.itemCode,
                count = saveItem.count,
                location = saveItem.location,
                durability = saveItem.durability,
                loaded = saveItem.loaded,
                magCount = saveItem.magAmmoCount
            });
            newItems.Add(item);
        }
        bag.SetMyItems(newItems);

        /* ② 장비 슬롯 복구 */
        foreach (var equip in data.equipSlot.equipSlots)
        {
            if (equip.item == null || string.IsNullOrEmpty(equip.item.itemCode))
                continue; // 🔥 아이템이 없으면 넘어감
            Debug.Log(equip.item.itemCode);
            var inst = ItemFactory.CreateItem(new ItemInitData
            {
                itemCode = equip.item.itemCode,
                count = equip.item.count,
                durability = equip.item.durability,
                loaded = equip.item.loaded,
                magCount = equip.item.magAmmoCount
            });

            var slotUI = BagInventoryManager.Instance.GetEquipSlotUI(equip.slotType);
            if (slotUI != null)
            {
                var itemUI = ItemUIPoolManager.Instance.GetItemUI(inst);
                itemUI.GetComponent<ItemInstanceUI>().EquipItem(slotUI);
                itemUI.GetComponent<ItemInstanceUI>().UpdateUI();
            }
            
        }


        /* ③ 힐 슬롯 복구 */
        /* ③ 힐 슬롯 복구 */
        for (int i = 0; i < heal.consumables.Length; i++)
        {
            var healItem = data.healSlot.healItems[i];

            if (healItem != null && !string.IsNullOrEmpty(healItem.itemCode))
            {
                var inst = ItemFactory.CreateItem(new ItemInitData
                {
                    itemCode = healItem.itemCode,
                    count = healItem.count
                }) as Consumable;

                if (inst != null)
                {
                    heal.consumables[i] = inst;

                    // 🔥 여기서 ItemUI를 생성하고
                    var itemUIObj = ItemUIPoolManager.Instance.GetItemUI(inst);
                    var itemUI = itemUIObj.GetComponent<ItemInstanceUI>();

                    // 🔥 Heal 슬롯에 장착
                    var slotUI = heal.healSlot[i];  // 이거 필요함: heal 슬롯 i번째 가져오기
                    if (slotUI != null)
                    {
                        itemUI.EquipHealItem(slotUI);
                    }
                }
            }
            else
            {
                heal.consumables[i] = null;
            }
            if (player != null)
            {
                player.money = data.money;
            }
        }

        /* ④ 베스트 슬롯 복구 */
        var slots = new VestPlacable[] { vest.PLeft, vest.PCenter, vest.PRight, vest.SOne, vest.SLeft, vest.SRight };
        for (int i = 0; i < slots.Length && i < data.vestPlaceables.placeables.Count; i++)
        {
            var save = data.vestPlaceables.placeables[i];
            var slot = slots[i];

            if (slot != null)
            {
                slot.placeableType = save.placeableType;
                slot.ammoType = save.ammoType;
                slot.count = save.count;
                slot.magAmmoCount = save.magAmmoCount;
                slot.UpdateUI();
            }
        }

        /* ⑤ 탄약 복구 */
        ammo.lightAmmo = data.ammunition.lightAmmo;
        ammo.mediumAmmo = data.ammunition.mediumAmmo;
        ammo.heavyAmmo = data.ammunition.heavyAmmo;
        ammo.antiAmmo = data.ammunition.antiAmmo;
        ammo.shellAmmo = data.ammunition.shellAmmo;
        ammo.magnumAmmo = data.ammunition.magnumAmmo;
        ammo.explosiveAmmo = data.ammunition.explosiveAmmo;
        ammo.UpdateAmmo();

        /* ⑥ 현재 무기 스왑 복구 */
        switch (data.currentWeapon.currentUsingWeapon)
        {
            case EquipSlotType.firstWeapon: vest.weaponOnHand = vest.weaponOnHand1; break;
            case EquipSlotType.secondWeapon: vest.weaponOnHand = vest.weaponOnHand2; break;
            case EquipSlotType.thirdWeapon: vest.weaponOnHand = vest.weaponOnHand3; break;
            default: vest.weaponOnHand = null; break;
        }

        if (vest.weaponOnHand != null)
            vest.shooter.SetWeapon(vest.weaponOnHand.currentWeapon);
        else
            vest.shooter.SetNoWeapon();
    }


    /*──────────────── ItemInstance ↔ SaveData 변환 ────────────────*/
    private static ItemSaveData ToItem(ItemInstance it)
    {
        var d = new ItemSaveData
        {
            itemCode = it.data.itemCode,
            count = it.count,
            location = it.location
        };

        if (it is Weapon w) { d.durability = w.durability; d.loaded = w.isChamber; d.magAmmoCount = w.magCount; }
        else if (it is Armor a) d.durability = a.durability;
        return d;
    }




}

/*──────────────────────── Save Data 구조체 (통합) ────────────────────────*/
[Serializable]
public class GameSaveData
{
    public string saveTime;

    public InventorySaveData inventory;
    public EquipSaveData equipSlot;
    public HealSaveData healSlot;
    public VestPlaceableSaveList vestPlaceables;
    public AmmunitionSaveData ammunition;
    public CurrentWeaponSaveData currentWeapon;

    public float money;  // 🔥 추가: 플레이어 돈
}


/*― 기존 구조체 재사용 ―*/
[Serializable] public class InventorySaveData { public List<ItemSaveData> myItems; }
[Serializable] public class EquipSaveData { public List<EquipSlotSaveData> equipSlots; }
[Serializable] public class HealSaveData { public ItemSaveData[] healItems; }
[Serializable] public class VestPlaceableSaveList { public List<VestPlaceableSaveData> placeables; }

[Serializable]
public class AmmunitionSaveData
{
    public int lightAmmo, mediumAmmo, heavyAmmo, antiAmmo, shellAmmo, magnumAmmo, explosiveAmmo;
}
[Serializable] public class CurrentWeaponSaveData { public EquipSlotType currentUsingWeapon; }

[Serializable]
public class ItemSaveData
{
    public string itemCode; public int count; public Vector2Int location;
    public float durability; public bool loaded; public int magAmmoCount;
}
[Serializable]
public class EquipSlotSaveData
{
    public EquipSlotType slotType; public ItemSaveData item;
}
[Serializable]
public class VestPlaceableSaveData
{
    public VestPlaceableType placeableType; public WeaponAType ammoType;
    public int count; public int magAmmoCount;
}
