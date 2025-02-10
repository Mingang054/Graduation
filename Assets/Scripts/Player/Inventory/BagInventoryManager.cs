using System.Collections.Generic;
using UnityEngine;

public class BagInventoryManager : MonoBehaviour
{
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
    public Transform myInventoryGrid;
    public Transform opponentInventoryGrid;

    private void Start()
    {
        InitMySlots();

        // 테스트용 아이템 생성
        TestCreateItem();
    }

    //--- MyInventory 초기화 ---//
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

    //--- 아이템 생성 테스트 ---//
    private void TestCreateItem()
    {
        string testItemCode = "F202"; // 테스트 아이템 코드
        int testItemCount = 2;        // 테스트 아이템 수량

        // 팩토리에서 아이템 생성
        ItemInstance testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            // 아이템 세부 사항 설정
            ItemFactory.SetItemInstance(
                testItem,
                count: testItemCount,
                location: new Vector2Int(2, 4), // 첫 번째 슬롯에 배치
                durability: 100,               // 내구도 설정 (옵션)
                charges: null                  // 사용 가능 횟수 없음
            );

            // MyInventory에 추가
            AddItemToMyInventory(testItem);
        }
        else
        {
            Debug.LogError($"아이템 생성 실패: 코드 '{testItemCode}'");
        }
    }

    //--- ItemInstance 추가 ---//
    public void AddItemToMyInventory(ItemInstance itemInstance)
    {
        myItems.Add(itemInstance);
        CreateItemUI(itemInstance, myInventoryGrid);
        Debug.Log($"내 인벤토리에 아이템 추가: {itemInstance.data.itemName}");
    }

    //--- 코드 기반의 아이템 생성 ---//
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

    //--- 특정 위치에 아이템 배치 (로드 시 사용) ---//
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

    //--- ItemInstance UI 생성 ---//
    public void CreateItemUI(ItemInstance itemInstance, Transform gridParent)
    {
        GameObject itemUIObject = Instantiate(itemInstanceUIPrefab, gridParent);
        ItemInstanceUI itemUI = itemUIObject.GetComponent<ItemInstanceUI>();
        itemUI.Initialize(itemInstance);
    }

    //--- Slot 관리 ---//
    private bool PlaceItemInSlot(ItemInstance itemInstance, Vector2Int location,
                                 Dictionary<Vector2Int, Slot> slots, List<ItemInstance> items, Vector2Int inventorySize)
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

    private void FreeItemSlots(ItemInstance itemInstance)
    {
        FreeSlots(itemInstance.location, itemInstance.data.size);
    }

    private void OccupySlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots)
    {
        foreach (Vector2Int position in GetSlotPositions(location, size, new Vector2Int()))
        {
            if (slots.TryGetValue(position, out var slot))
            {
                slot.SetOccupied(true);
            }
        }
    }

    private void FreeSlots(Vector2Int location, Vector2Int size)
    {
        foreach (Vector2Int position in GetSlotPositions(location, size, new Vector2Int()))
        {
            if (mySlots.TryGetValue(position, out var slot))
            {
                slot.SetOccupied(false);
            }
        }
    }

    private bool ValidSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
    {
        foreach (Vector2Int position in GetSlotPositions(location, size, inventorySize))
        {
            if (!slots.TryGetValue(position, out var slot) || slot.GetOccupied())
            {
                return false;
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
    public Vector2Int Location { get; } // 슬롯 위치 정보 (읽기 전용)
    public bool IsOccupied { get; private set; } // 슬롯 점유 여부 (읽기 전용)

    public Slot(Vector2Int location)
    {
        Location = location;
        IsOccupied = false; // 초기화 시 점유되지 않은 상태
    }

    // 슬롯 점유 상태 변경 메서드
    public void SetOccupied(bool state)
    {
        IsOccupied = state;
    }

    // 점유 상태를 반환하는 메서드
    public bool GetOccupied()
    {
        return IsOccupied;
    }
}
