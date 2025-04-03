using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class NewBagInventoryManager : MonoBehaviour
{
    public Vector2Int currentPointedSlot = new Vector2Int(-1, -1);
    public bool currentPointedSlotIsMySlot = false;
    public bool currentPointedSlotIsEquip = false;
    public EquipmentSlotUI currentPointedEquipSlot = null;

<<<<<<< HEAD
    //-- EquipmentSlot ∞¸∏Æ --// Serialize
=======
    //-- EquipmentSlot ∞¸∏Æ --//
>>>>>>> 8ba03cc5 ([UPDATE] Ï¢ÄÎπÑ ÏÇ¨Îßù Ïï†ÎãàÎ©îÏù¥ÏÖò Î∞è Ïù¥ÌéôÌä∏ Íµ¨ÌòÑ)
    public ItemInstanceUI firstWeapon;
    public ItemInstanceUI secondWeapon;
    public ItemInstanceUI thirdWeapon; //pistol≈∏¿‘∏∏ «„øÎ
    public ItemInstanceUI headArmor;
    public ItemInstanceUI bodyArmor;


    //--- My Inventory ∞¸∏Æ ---//
    public Vector2Int myInventoryVector = new Vector2Int(8, 12);
    public Dictionary<Vector2Int, Slot> mySlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> myItems = new List<ItemInstance>();

    //--- Opponent Inventory ∞¸∏Æ ---//
    public Vector2Int opponentInventoryVector;
    public Dictionary<Vector2Int, Slot> opponentSlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> opponentItems;

    //--- UI ∞¸∑√ ---//
    public GameObject itemInstanceUIPrefab;
    public Transform myInventory;
    public Transform myInventoryGrid;
    public Transform opponentInventory;
    public Transform opponentInventoryGrid;

    //--SlotUI¡¢±Ÿ--//
    public Dictionary<Vector2Int, SlotUI> mySlotsUI = new Dictionary<Vector2Int, SlotUI>();
    public Dictionary<Vector2Int, SlotUI> opponentSlotsUI = new Dictionary<Vector2Int, SlotUI>();

    [SerializeField]
    private GameObject slotUIPrefab;


    public static NewBagInventoryManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // ¡ﬂ∫π ¿ŒΩ∫≈œΩ∫ πÊ¡ˆ
        }
        if (slotUIPrefab != null && myInventoryGrid!=null && opponentInventoryGrid!=null)
        {
            InitSlotUI();
        }
    }
    private void Start()
    {
        InitMySlots();
        InitOpSlots();
        // ≈◊Ω∫∆ÆøÎ æ∆¿Ã≈€ ª˝º∫
        TestCreateItem();
    }


    //2025-03-17 12:31PM
    //µÂ∑°±◊ µÂ∑” Ω√ æ∆¿Ã≈€¿Ã πËƒ°µ… ¿ßƒ° ¡§∫∏∏¶ √Êµπ∑Œ ∞ËªÍ«œ±‚ ¿ß«— UIΩΩ∑‘ √ ±‚»≠
    //
    public void InitSlotUI()
    {
        for (int y = 1; y <= 12; y++)
        {
            for (int x = 1; x <= 8; x++)
            {
                GameObject slotUIObject = Instantiate(slotUIPrefab, myInventoryGrid);
                SlotUI slotUI = slotUIObject.GetComponent<SlotUI>();
                slotUI.SetLocation(new Vector2Int(x, y));
                slotUI.isMySlot = true;
                mySlotsUI[slotUI.location] = slotUI;
            }
        }

        for (int y = 1; y <= 12; y++)
        {
            for (int x = 1; x <= 8; x++)
            {
                GameObject slotUIObject = Instantiate(slotUIPrefab, opponentInventoryGrid);
                SlotUI slotUI = slotUIObject.GetComponent<SlotUI>();
                slotUI.SetLocation(new Vector2Int(x, y));
                slotUI.isMySlot = false;
                opponentSlotsUI[slotUI.location] = slotUI;
            }
        }


    }











    //--- MyInventory √ ±‚»≠ ---//
    public void InitMySlots()
    {
        mySlots.Clear();
        for (int y = 1; y <= myInventoryVector.y; y++)
        {
            for (int x = 1; x <= myInventoryVector.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                mySlots[position] = new Slot(position);
                Debug.Log($"≥ª ΩΩ∑‘ √ﬂ∞°µ : {position}");
            }
        }
    }
    public void InitOpSlots()
    {
        opponentSlots.Clear();
        for (int y = 1; y <= opponentInventoryVector.y; y++)
        {
            for (int x = 1; x <= opponentInventoryVector.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                opponentSlots[position] = new Slot(position);
                Debug.Log($"≥ª ΩΩ∑‘ √ﬂ∞°µ : {position}");
            }
        }
    }
    //--- æ∆¿Ã≈€ ª˝º∫ ≈◊Ω∫∆Æ ---//
    private void TestCreateItem()
    {
        string testItemCode = "W101"; // ≈◊Ω∫∆Æ æ∆¿Ã≈€ ƒ⁄µÂ
        int testItemCount = 1;        // ≈◊Ω∫∆Æ æ∆¿Ã≈€ ºˆ∑Æ

        // ∆—≈‰∏Æø°º≠ æ∆¿Ã≈€ ª˝º∫
        ItemInstance testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            // æ∆¿Ã≈€ ºº∫Œ ªÁ«◊ º≥¡§
            ItemFactory.SetItemInstance(
                testItem,
                count: testItemCount,
                location: new Vector2Int(2, 4), // √π π¯¬∞ ΩΩ∑‘ø° πËƒ°
                durability: 100,               // ≥ª±∏µµ º≥¡§ (ø…º«)
                charges: null                  // ªÁøÎ ∞°¥… »Ωºˆ æ¯¿Ω
            );

            // MyInventoryø° √ﬂ∞°
            AddItemToMyInventory(testItem);
        }
        else
        {
            Debug.LogError($"æ∆¿Ã≈€ ª˝º∫ Ω«∆–: ƒ⁄µÂ '{testItemCode}'");
        }
        testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            // æ∆¿Ã≈€ ºº∫Œ ªÁ«◊ º≥¡§
            ItemFactory.SetItemInstance(
                testItem,
                count: testItemCount,
                location: new Vector2Int(3, 4), // √π π¯¬∞ ΩΩ∑‘ø° πËƒ°
                durability: 100,               // ≥ª±∏µµ º≥¡§ (ø…º«)
                charges: null                  // ªÁøÎ ∞°¥… »Ωºˆ æ¯¿Ω
            );

            // MyInventoryø° √ﬂ∞°
            AddItemToMyInventory(testItem);
        }
        else
        {
            Debug.LogError($"æ∆¿Ã≈€ ª˝º∫ Ω«∆–: ƒ⁄µÂ '{testItemCode}'");
        }

    }

    //--- ItemInstance √ﬂ∞° ---//
    public void AddItemToMyInventory(ItemInstance itemInstance)
    {
        myItems.Add(itemInstance);
        CreateItemUI(itemInstance, myInventory);
        Debug.Log($"≥ª ¿Œ∫•≈‰∏Æø° æ∆¿Ã≈€ √ﬂ∞°: {itemInstance.data.itemName}");
    }

    //--- ƒ⁄µÂ ±‚π›¿« æ∆¿Ã≈€ ª˝º∫ ---//
    public void AddItemByCode(string itemCode, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ItemInstance newItem = ItemFactory.CreateItem(itemCode);

            if (newItem != null)
            {
                Vector2Int? availableSlot = FindFirstAvailableSlot(newItem.data.size, mySlots, myInventoryVector);

                if (availableSlot == null)
                {
                    Debug.LogWarning($"'{itemCode}' æ∆¿Ã≈€¿ª √ﬂ∞°«“ ∞¯∞£¿Ã æ¯Ω¿¥œ¥Ÿ.");
                    return;
                }

                ItemFactory.SetItemInstance(newItem, 1, availableSlot.Value);
                AddItemToMyInventory(newItem);
            }
        }
    }

    //--- ∆Ø¡§ ¿ßƒ°ø° æ∆¿Ã≈€ πËƒ° (∑ŒµÂ Ω√ ªÁøÎ) ---//
    public void AddItemAtPosition(string itemCode, int count, Vector2Int position)
    {
        for (int i = 0; i < count; i++)
        {
            ItemInstance newItem = ItemFactory.CreateItem(itemCode);

            if (newItem != null)
            {
                if (!ValidSlots(position, newItem.data.size, mySlots, myInventoryVector))
                {
                    Debug.LogWarning($"¿ßƒ° {position}ø°º≠ '{itemCode}' æ∆¿Ã≈€¿ª √ﬂ∞°«“ ∞¯∞£¿Ã ∫Œ¡∑«’¥œ¥Ÿ.");
                    return;
                }

                ItemFactory.SetItemInstance(newItem, 1, position);
                AddItemToMyInventory(newItem);
            }
        }
    }

    //--- ItemInstance UI ª˝º∫ ---//
    public void CreateItemUI(ItemInstance itemInstance, Transform gridParent)
    {
        GameObject itemUIObject = Instantiate(itemInstanceUIPrefab, gridParent);
        ItemInstanceUI itemUI = itemUIObject.GetComponent<ItemInstanceUI>();
        itemUI.Initialize(itemInstance);
    }

    //--- Slot ∞¸∏Æ ---//
    public bool PlaceItemInSlot(ItemInstance itemInstance, Vector2Int location,
                                 Dictionary<Vector2Int, Slot> slots, List<ItemInstance> items, Vector2Int inventorySize)
    {
        Vector2Int size = itemInstance.data.size;

        if (!ValidSlots(location, size, slots, inventorySize))
        {
            Debug.LogWarning($"¿ßƒ° {location}ø°º≠ æ∆¿Ã≈€ '{itemInstance.data.itemName}'¿ª(∏¶) πËƒ°«“ ∞¯∞£¿Ã ∫Œ¡∑«’¥œ¥Ÿ.");
            return false;
        }

        OccupySlots(location, size, slots);
        itemInstance.location = location;
        items.Add(itemInstance);
        Debug.Log($"æ∆¿Ã≈€ '{itemInstance.data.itemName}'¿Ã(∞°) {location} ¿ßƒ°ø° πËƒ°µ«æ˙Ω¿¥œ¥Ÿ.");
        return true;
    }

    private bool PlaceFirstAvailableSlot(ItemInstance itemInstance, List<ItemInstance> targetItems)
    {
        Vector2Int size = itemInstance.data.size;
        Vector2Int sizeOfInventory = (targetItems == myItems) ? myInventoryVector : opponentInventoryVector;
        Dictionary<Vector2Int, Slot> targetSlot = (targetItems == myItems) ? mySlots : opponentSlots;

        Vector2Int? location = FindFirstAvailableSlot(size, targetSlot, sizeOfInventory);
        if (location == null) return false;

        FreeItemSlots(itemInstance);
        return PlaceItemInSlot(itemInstance, location.Value, targetSlot, targetItems, sizeOfInventory);
    }

    public void FreeItemSlots(ItemInstance itemInstance)
    {
        Vector2Int location = itemInstance.location;
        Vector2Int size = itemInstance.data.size;

        // 1) æ∆¿Ã≈€¿Ã ≥ª ¿Œ∫•≈‰∏Æø° º”«ÿ ¿÷æ˙¥Ÿ∏È mySlots «ÿ¡¶
        if (myItems.Contains(itemInstance))
        {
            FreeSlots(location, size, mySlots);
        }
        // 2) ªÛ¥Î ¿Œ∫•≈‰∏Æø° º”«ÿ ¿÷æ˙¥Ÿ∏È opponentSlots «ÿ¡¶
        else if (opponentItems.Contains(itemInstance))
        {
            FreeSlots(location, size, opponentSlots);
        }
        else
        {
            Debug.LogWarning("FreeItemSlots: ¿Ã æ∆¿Ã≈€¿Ã æÓ¥¿ ¿Œ∫•≈‰∏Æø°µµ º”«ÿ ¿÷¡ˆ æ Ω¿¥œ¥Ÿ.");
        }
    }


    public void OccupySlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots)
    {
        Debug.Log($"[OccupySlots] ¡°¿Ø Ω√¿€: Ω√¿€ ¿ßƒ° = {location}, ≈©±‚ = {size}");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (slots.TryGetValue(position, out var slot))
                {
                    slot.SetOccupied(true);
                    Debug.Log($"[OccupySlots] ¡°¿Ø ΩΩ∑‘: {position}");
                }
                else
                {
                    Debug.LogWarning($"[OccupySlots] ΩΩ∑‘ æ¯¿Ω: {position}");
                }
            }
        }
    }

    private void FreeSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> targetSlots)
    {
        Debug.Log($"[FreeSlots] ¡°¿Ø «ÿ¡¶ Ω√¿€: ¿ßƒ° = {location}, ≈©±‚ = {size}");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (targetSlots.TryGetValue(pos, out var slot))
                {
                    slot.SetOccupied(false);
                    Debug.Log($"[FreeSlots] «ÿ¡¶µ» ΩΩ∑‘: {pos}");
                }
                else
                {
                    Debug.LogWarning($"[FreeSlots] ΩΩ∑‘ æ¯¿Ω: {pos}");
                }
            }
        }
    }



    public bool ValidSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
    {
        Debug.Log("ValidSlots called (for-loop)");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                // ¿Œ∫•≈‰∏Æ π¸¿ß∏¶ ≥—¿∏∏È π´Ω√
                if (x > inventorySize.x || y > inventorySize.y)
                {
                    Debug.LogWarning($"[ValidSlots] ∞ÀªÁ ¡¶ø‹: π¸¿ß √ ∞˙ ¿ßƒ° ({x},{y})");
                    continue;
                }

                Vector2Int pos = new Vector2Int(x, y);
                Debug.Log($"[ValidSlots] ∞ÀªÁ ¡ﬂ¿Œ ΩΩ∑‘ ¿ßƒ°: {pos}");

                if (!slots.TryGetValue(pos, out var slot))
                {
                    Debug.LogWarning($"[ValidSlots] ΩΩ∑‘ ¡§∫∏ æ¯¿Ω: {pos}");
                    return false;
                }

                if (slot.GetOccupied())
                {
                    Debug.LogWarning($"[ValidSlots] ΩΩ∑‘ ¡°¿Ø ¡ﬂ: {pos}");
                    return false;
                }
            }
        }

        return true;
    }


    private IEnumerable<Vector2Int> GetSlotPositions(Vector2Int location, Vector2Int size, Vector2Int inventorySize)
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

    private Vector2Int? FindFirstAvailableSlot(Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
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
}


public class Slot
{
    public Vector2Int Location { get; } // ΩΩ∑‘ ¿ßƒ° ¡§∫∏ (¿–±‚ ¿¸øÎ)
    public bool IsOccupied { get; private set; } // ΩΩ∑‘ ¡°¿Ø ø©∫Œ (¿–±‚ ¿¸øÎ)

    public Slot(Vector2Int location)
    {
        Location = location;
        IsOccupied = false; // √ ±‚»≠ Ω√ ¡°¿Øµ«¡ˆ æ ¿∫ ªÛ≈¬
    }

    // ΩΩ∑‘ ¡°¿Ø ªÛ≈¬ ∫Ø∞Ê ∏ﬁº≠µÂ
    public void SetOccupied(bool state)
    {
        IsOccupied = state;
    }

    // ¡°¿Ø ªÛ≈¬∏¶ π›»Ø«œ¥¬ ∏ﬁº≠µÂ
    public bool GetOccupied()
    {
        return IsOccupied;
    }
}