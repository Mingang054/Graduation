using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class NewBagInventoryManager : MonoBehaviour
{
    public Vector2Int currentPointedSlot = new Vector2Int(-1, -1);
    public bool currentPointedSlotIsMySlot = false;
    public bool currentPointedSlotIsEquip = false;
    public EquipmentSlotUI currentPointedEquipSlot = null;

    //-- EquipmentSlot 관리 --//
    public ItemInstanceUI firstWeapon;
    public ItemInstanceUI secondWeapon;
    public ItemInstanceUI thirdWeapon; //pistol타입만 허용
    public ItemInstanceUI headArmor;
    public ItemInstanceUI bodyArmor;

    //--- My Inventory 관리 ---//
    public Vector2Int myInventoryVector = new Vector2Int(8, 12);
    public Dictionary<Vector2Int, Slot> mySlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> myItems = new List<ItemInstance>();

    //--- Opponent Inventory 관리 ---//
    public Vector2Int opponentInventoryVector;
    public Dictionary<Vector2Int, Slot> opponentSlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> opponentItems;

    //--- UI 관련 ---//
    public GameObject itemInstanceUIPrefab;
    public Transform myInventory;
    public Transform myInventoryGrid;
    public Transform opponentInventory;
    public Transform opponentInventoryGrid;

    //--SlotUI접근--//
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
            Destroy(gameObject);
        }
        if (slotUIPrefab != null && myInventoryGrid != null && opponentInventoryGrid != null)
        {
            InitSlotUI();
        }
    }

    private void Start()
    {
        InitMySlots();
        InitOpSlots();
        TestCreateItem();
    }

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

    public void InitMySlots()
    {
        mySlots.Clear();
        for (int y = 1; y <= myInventoryVector.y; y++)
        {
            for (int x = 1; x <= myInventoryVector.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                mySlots[position] = new Slot(position);
                Debug.Log($"내 슬롯 추가됨: {position}");
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
                Debug.Log($"상대 슬롯 추가됨: {position}");
            }
        }
    }

    private void TestCreateItem()
    {
        string testItemCode = "W101";
        int testItemCount = 1;

        ItemInstance testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            ItemFactory.SetItemInstance(testItem, testItemCount, new Vector2Int(2, 4), 100, null);
            AddItemToMyInventory(testItem);
        }

        testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            ItemFactory.SetItemInstance(testItem, testItemCount, new Vector2Int(3, 4), 100, null);
            AddItemToMyInventory(testItem);
        }
    }

    public void AddItemToMyInventory(ItemInstance itemInstance)
    {
        myItems.Add(itemInstance);
        CreateItemUI(itemInstance, myInventory);
        Debug.Log($"내 인벤토리에 아이템 추가: {itemInstance.data.itemName}");
    }

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
                    Debug.LogWarning($"'{itemCode}' 아이템을 추가할 공간이 없습니다.");
                    return;
                }

                ItemFactory.SetItemInstance(newItem, 1, availableSlot.Value);
                AddItemToMyInventory(newItem);
            }
        }
    }

    public void AddItemAtPosition(string itemCode, int count, Vector2Int position)
    {
        for (int i = 0; i < count; i++)
        {
            ItemInstance newItem = ItemFactory.CreateItem(itemCode);

            if (newItem != null)
            {
                if (!ValidSlots(position, newItem.data.size, mySlots, myInventoryVector))
                {
                    Debug.LogWarning($"위치 {position}에서 '{itemCode}' 아이템을 추가할 공간이 부족합니다.");
                    return;
                }

                ItemFactory.SetItemInstance(newItem, 1, position);
                AddItemToMyInventory(newItem);
            }
        }
    }

    public void CreateItemUI(ItemInstance itemInstance, Transform gridParent)
    {
        GameObject itemUIObject = Instantiate(itemInstanceUIPrefab, gridParent);
        ItemInstanceUI itemUI = itemUIObject.GetComponent<ItemInstanceUI>();
        itemUI.Initialize(itemInstance);
    }

    public bool PlaceItemInSlot(ItemInstance itemInstance, Vector2Int location, Dictionary<Vector2Int, Slot> slots, List<ItemInstance> items, Vector2Int inventorySize)
    {
        Vector2Int size = itemInstance.data.size;

        if (!ValidSlots(location, size, slots, inventorySize))
        {
            Debug.LogWarning($"위치 {location}에서 아이템 '{itemInstance.data.itemName}'을(를) 배치할 공간이 부족합니다.");
            return false;
        }

        OccupySlots(location, size, slots);
        itemInstance.location = location;
        items.Add(itemInstance);
        Debug.Log($"아이템 '{itemInstance.data.itemName}'이(가) {location} 위치에 배치되었습니다.");
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

        if (myItems.Contains(itemInstance))
        {
            FreeSlots(location, size, mySlots);
        }
        else if (opponentItems.Contains(itemInstance))
        {
            FreeSlots(location, size, opponentSlots);
        }
        else
        {
            Debug.LogWarning("FreeItemSlots: 이 아이템이 어느 인벤토리에도 속해 있지 않습니다.");
        }
    }

    public void OccupySlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (slots.TryGetValue(position, out var slot))
                {
                    slot.SetOccupied(true);
                }
            }
        }
    }

    private void FreeSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> targetSlots)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (targetSlots.TryGetValue(pos, out var slot))
                {
                    slot.SetOccupied(false);
                }
            }
        }
    }

    public bool ValidSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                if (x > inventorySize.x || y > inventorySize.y)
                {
                    continue;
                }

                Vector2Int pos = new Vector2Int(x, y);

                if (!slots.TryGetValue(pos, out var slot) || slot.GetOccupied())
                {
                    return false;
                }
            }
        }
        return true;
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
    public Vector2Int Location { get; }
    public bool IsOccupied { get; private set; }

    public Slot(Vector2Int location)
    {
        Location = location;
        IsOccupied = false;
    }

    public void SetOccupied(bool state)
    {
        IsOccupied = state;
    }

    public bool GetOccupied()
    {
        return IsOccupied;
    }
}
