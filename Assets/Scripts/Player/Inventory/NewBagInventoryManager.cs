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
    //-- EquipmentSlot ���� --// Serialize
=======
    //-- EquipmentSlot ���� --//
>>>>>>> 8ba03cc5 ([UPDATE] 좀비 사망 애니메이션 및 이펙트 구현)
    public ItemInstanceUI firstWeapon;
    public ItemInstanceUI secondWeapon;
    public ItemInstanceUI thirdWeapon; //pistolŸ�Ը� ���
    public ItemInstanceUI headArmor;
    public ItemInstanceUI bodyArmor;


    //--- My Inventory ���� ---//
    public Vector2Int myInventoryVector = new Vector2Int(8, 12);
    public Dictionary<Vector2Int, Slot> mySlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> myItems = new List<ItemInstance>();

    //--- Opponent Inventory ���� ---//
    public Vector2Int opponentInventoryVector;
    public Dictionary<Vector2Int, Slot> opponentSlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> opponentItems;

    //--- UI ���� ---//
    public GameObject itemInstanceUIPrefab;
    public Transform myInventory;
    public Transform myInventoryGrid;
    public Transform opponentInventory;
    public Transform opponentInventoryGrid;

    //--SlotUI����--//
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
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
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
        // �׽�Ʈ�� ������ ����
        TestCreateItem();
    }


    //2025-03-17 12:31PM
    //�巡�� ��� �� �������� ��ġ�� ��ġ ������ �浹�� ����ϱ� ���� UI���� �ʱ�ȭ
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











    //--- MyInventory �ʱ�ȭ ---//
    public void InitMySlots()
    {
        mySlots.Clear();
        for (int y = 1; y <= myInventoryVector.y; y++)
        {
            for (int x = 1; x <= myInventoryVector.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                mySlots[position] = new Slot(position);
                Debug.Log($"�� ���� �߰���: {position}");
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
                Debug.Log($"�� ���� �߰���: {position}");
            }
        }
    }
    //--- ������ ���� �׽�Ʈ ---//
    private void TestCreateItem()
    {
        string testItemCode = "W101"; // �׽�Ʈ ������ �ڵ�
        int testItemCount = 1;        // �׽�Ʈ ������ ����

        // ���丮���� ������ ����
        ItemInstance testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            // ������ ���� ���� ����
            ItemFactory.SetItemInstance(
                testItem,
                count: testItemCount,
                location: new Vector2Int(2, 4), // ù ��° ���Կ� ��ġ
                durability: 100,               // ������ ���� (�ɼ�)
                charges: null                  // ��� ���� Ƚ�� ����
            );

            // MyInventory�� �߰�
            AddItemToMyInventory(testItem);
        }
        else
        {
            Debug.LogError($"������ ���� ����: �ڵ� '{testItemCode}'");
        }
        testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            // ������ ���� ���� ����
            ItemFactory.SetItemInstance(
                testItem,
                count: testItemCount,
                location: new Vector2Int(3, 4), // ù ��° ���Կ� ��ġ
                durability: 100,               // ������ ���� (�ɼ�)
                charges: null                  // ��� ���� Ƚ�� ����
            );

            // MyInventory�� �߰�
            AddItemToMyInventory(testItem);
        }
        else
        {
            Debug.LogError($"������ ���� ����: �ڵ� '{testItemCode}'");
        }

    }

    //--- ItemInstance �߰� ---//
    public void AddItemToMyInventory(ItemInstance itemInstance)
    {
        myItems.Add(itemInstance);
        CreateItemUI(itemInstance, myInventory);
        Debug.Log($"�� �κ��丮�� ������ �߰�: {itemInstance.data.itemName}");
    }

    //--- �ڵ� ����� ������ ���� ---//
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
                    Debug.LogWarning($"'{itemCode}' �������� �߰��� ������ �����ϴ�.");
                    return;
                }

                ItemFactory.SetItemInstance(newItem, 1, availableSlot.Value);
                AddItemToMyInventory(newItem);
            }
        }
    }

    //--- Ư�� ��ġ�� ������ ��ġ (�ε� �� ���) ---//
    public void AddItemAtPosition(string itemCode, int count, Vector2Int position)
    {
        for (int i = 0; i < count; i++)
        {
            ItemInstance newItem = ItemFactory.CreateItem(itemCode);

            if (newItem != null)
            {
                if (!ValidSlots(position, newItem.data.size, mySlots, myInventoryVector))
                {
                    Debug.LogWarning($"��ġ {position}���� '{itemCode}' �������� �߰��� ������ �����մϴ�.");
                    return;
                }

                ItemFactory.SetItemInstance(newItem, 1, position);
                AddItemToMyInventory(newItem);
            }
        }
    }

    //--- ItemInstance UI ���� ---//
    public void CreateItemUI(ItemInstance itemInstance, Transform gridParent)
    {
        GameObject itemUIObject = Instantiate(itemInstanceUIPrefab, gridParent);
        ItemInstanceUI itemUI = itemUIObject.GetComponent<ItemInstanceUI>();
        itemUI.Initialize(itemInstance);
    }

    //--- Slot ���� ---//
    public bool PlaceItemInSlot(ItemInstance itemInstance, Vector2Int location,
                                 Dictionary<Vector2Int, Slot> slots, List<ItemInstance> items, Vector2Int inventorySize)
    {
        Vector2Int size = itemInstance.data.size;

        if (!ValidSlots(location, size, slots, inventorySize))
        {
            Debug.LogWarning($"��ġ {location}���� ������ '{itemInstance.data.itemName}'��(��) ��ġ�� ������ �����մϴ�.");
            return false;
        }

        OccupySlots(location, size, slots);
        itemInstance.location = location;
        items.Add(itemInstance);
        Debug.Log($"������ '{itemInstance.data.itemName}'��(��) {location} ��ġ�� ��ġ�Ǿ����ϴ�.");
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

        // 1) �������� �� �κ��丮�� ���� �־��ٸ� mySlots ����
        if (myItems.Contains(itemInstance))
        {
            FreeSlots(location, size, mySlots);
        }
        // 2) ��� �κ��丮�� ���� �־��ٸ� opponentSlots ����
        else if (opponentItems.Contains(itemInstance))
        {
            FreeSlots(location, size, opponentSlots);
        }
        else
        {
            Debug.LogWarning("FreeItemSlots: �� �������� ��� �κ��丮���� ���� ���� �ʽ��ϴ�.");
        }
    }


    public void OccupySlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots)
    {
        Debug.Log($"[OccupySlots] ���� ����: ���� ��ġ = {location}, ũ�� = {size}");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (slots.TryGetValue(position, out var slot))
                {
                    slot.SetOccupied(true);
                    Debug.Log($"[OccupySlots] ���� ����: {position}");
                }
                else
                {
                    Debug.LogWarning($"[OccupySlots] ���� ����: {position}");
                }
            }
        }
    }

    private void FreeSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> targetSlots)
    {
        Debug.Log($"[FreeSlots] ���� ���� ����: ��ġ = {location}, ũ�� = {size}");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (targetSlots.TryGetValue(pos, out var slot))
                {
                    slot.SetOccupied(false);
                    Debug.Log($"[FreeSlots] ������ ����: {pos}");
                }
                else
                {
                    Debug.LogWarning($"[FreeSlots] ���� ����: {pos}");
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
                // �κ��丮 ������ ������ ����
                if (x > inventorySize.x || y > inventorySize.y)
                {
                    Debug.LogWarning($"[ValidSlots] �˻� ����: ���� �ʰ� ��ġ ({x},{y})");
                    continue;
                }

                Vector2Int pos = new Vector2Int(x, y);
                Debug.Log($"[ValidSlots] �˻� ���� ���� ��ġ: {pos}");

                if (!slots.TryGetValue(pos, out var slot))
                {
                    Debug.LogWarning($"[ValidSlots] ���� ���� ����: {pos}");
                    return false;
                }

                if (slot.GetOccupied())
                {
                    Debug.LogWarning($"[ValidSlots] ���� ���� ��: {pos}");
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
    public Vector2Int Location { get; } // ���� ��ġ ���� (�б� ����)
    public bool IsOccupied { get; private set; } // ���� ���� ���� (�б� ����)

    public Slot(Vector2Int location)
    {
        Location = location;
        IsOccupied = false; // �ʱ�ȭ �� �������� ���� ����
    }

    // ���� ���� ���� ���� �޼���
    public void SetOccupied(bool state)
    {
        IsOccupied = state;
    }

    // ���� ���¸� ��ȯ�ϴ� �޼���
    public bool GetOccupied()
    {
        return IsOccupied;
    }
}