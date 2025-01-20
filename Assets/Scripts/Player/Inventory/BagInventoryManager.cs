using System.Collections.Generic;
using UnityEngine;

public class BagInventoryManager : MonoBehaviour
{
    //--- My Inventory 관리 ---//
    public Vector2Int myInventoryVector = new Vector2Int(8, 12);
    public Dictionary<Vector2Int, Slot> mySlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> myItems = new List<ItemInstance>();

    //--- opponent Inventory 관리 ---//
    public Vector2Int opponentInventoryVector; // 타겟 인벤토리 크기
    public Dictionary<Vector2Int, Slot> opponentSlots = new Dictionary<Vector2Int, Slot>();
    public List<ItemInstance> opponentItems;

    //--- UI 관련 ---//
    public GameObject itemInstanceUIPrefab; // ItemInstanceUI 프리팹
    public Transform myInventoryGrid; // MyInventory의 Grid Layout Parent
    public Transform opponentInventoryGrid; // opponentInventory의 Grid Layout Parent

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
        string testItemCode = "F201"; // 테스트 아이템 코드
        int testItemCount = 2;        // 테스트 아이템 수량

        // 팩토리에서 아이템 생성
        ItemInstance testItem = ItemFactory.CreateItem(testItemCode);

        if (testItem != null)
        {
            // 아이템 세부 사항 설정
            ItemFactory.SetItemInstance(
                testItem,
                count: testItemCount,
                location: new Vector2Int(1, 1), // 첫 번째 슬롯에 배치
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

    //--- ItemInstance UI 생성 ---//
    public void CreateItemUI(ItemInstance itemInstance, Transform gridParent)
    {
        GameObject itemUIObject = Instantiate(itemInstanceUIPrefab, gridParent);
        ItemInstanceUI itemUI = itemUIObject.GetComponent<ItemInstanceUI>();
        itemUI.Initialize(itemInstance); // UI와 데이터 연동
    }

    //--- Slot 관리 ---//

    // 아이템을 놓을때 사용
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


    //넘겨주기, 인벤토리에서 Item을 alt 클릭 시 사용
    private void HandOver(ItemInstance itemInstance)
    {
        // 1. 내 인벤토리에 있는 아이템인지 확인
        if (myItems.Contains(itemInstance))
        {
            // 상대 인벤토리가 null인 경우 아무 작업도 하지 않음
            if (opponentItems == null)
            {
                Debug.LogWarning("상대 인벤토리가 존재하지 않습니다.");
                return;
            }

            // 상대 인벤토리의 처음 가능한 장소로 아이템 이동 시도
            bool moved = PlaceFirstAvailableSlot(itemInstance, opponentItems);
            if (!moved)
            {
                Debug.LogWarning("상대 인벤토리가 꽉 차 있어 아이템을 이동할 수 없습니다.");
            }
            return;
        }

        // 2. 상대 인벤토리에 있는 아이템인지 확인
        if (opponentItems != null && opponentItems.Contains(itemInstance))
        {
            // 내 인벤토리의 처음 가능한 장소로 아이템 이동 시도
            bool moved = PlaceFirstAvailableSlot(itemInstance, myItems);
            if (!moved)
            {
                Debug.LogWarning("내 인벤토리가 꽉 차 있어 아이템을 이동할 수 없습니다.");
            }
            return;
        }

        // 3. 아이템이 어느 리스트에도 포함되지 않은 경우
        Debug.LogWarning($"아이템 '{itemInstance.data.itemName}'이(가) 어느 인벤토리에도 포함되어 있지 않습니다.");
    }
    
    private bool PlaceFirstAvailableSlot(ItemInstance itemInstance, List<ItemInstance> targetItems)
    {
        Vector2Int size = itemInstance.data.size;
        Vector2Int sizeOfInventory;
        Dictionary<Vector2Int, Slot> targetSlot;

        // 타겟 리스트에 따라 슬롯과 인벤토리 크기 결정
        if (targetItems == myItems)
        {
            targetSlot = mySlots;
            sizeOfInventory = myInventoryVector;
        }
        else if (targetItems == opponentItems)
        {
            targetSlot = opponentSlots;
            sizeOfInventory = opponentInventoryVector;
        }
        else
        {
            return false; // 타겟 리스트가 유효하지 않음
        }

        // 첫 번째로 사용 가능한 슬롯 찾기
        Vector2Int? location = FindFirstAvailableSlot(size, targetSlot, sizeOfInventory);
        if (location == null)
        {
            return false; // 사용 가능한 슬롯이 없음
        }

        // 기존 슬롯 해제 및 아이템 위치 초기화
        PrepareItemForRelocation(itemInstance);

        // 새로운 슬롯에 아이템 배치
        return PlaceItemInSlot(itemInstance, location.Value, targetSlot, targetItems, sizeOfInventory);
    }



    // 아이템 인스턴스를 포함된 리스트에서 제거하고 기존 슬롯을 해제하며 위치를 초기화
    private void PrepareItemForRelocation(ItemInstance itemInstance)
    {
        List<ItemInstance> sourceList = null;
        Dictionary<Vector2Int, Slot> sourceSlots = null;

        // 아이템이 어느 리스트에 포함되어 있는지 확인
        if (myItems.Contains(itemInstance))
        {
            sourceList = myItems;
            sourceSlots = mySlots;
        }
        else if (opponentItems!=null && opponentItems.Contains(itemInstance))
        {
            sourceList = opponentItems;
            sourceSlots = opponentSlots;
        }

        if (sourceList == null)
        {
            Debug.LogWarning($"아이템 '{itemInstance.data.itemName}'이(가) 어느 리스트에도 포함되어 있지 않습니다.");
            return;
        }

        // 리스트에서 아이템 제거
        sourceList.Remove(itemInstance);
        Debug.Log($"아이템 '{itemInstance.data.itemName}'이(가) 리스트에서 제거되었습니다.");

        // 기존 슬롯 해제
        Vector2Int size = itemInstance.data.size;
        Vector2Int location = itemInstance.location;

        if (location != Vector2Int.zero)
        {
            foreach (Vector2Int position in GetSlotPositions(location, size, new Vector2Int()))
            {
                if (sourceSlots.TryGetValue(position, out var slot))
                {
                    slot.SetOccupied(false);
                    Debug.Log($"슬롯 {position} 해제 완료.");
                }
            }
        }

        // 아이템 위치 초기화
        itemInstance.location = Vector2Int.zero;
        Debug.Log($"아이템 '{itemInstance.data.itemName}' 위치가 초기화되었습니다.");
    }


    private bool ValidSlots(Vector2Int location, Vector2Int size,
                            Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
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

    private IEnumerable<Vector2Int> GetSlotPositions(Vector2Int location, Vector2Int size, Vector2Int inventorySize)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                if (x > inventorySize.x || y > inventorySize.y)
                {
                    continue;
                }
                yield return new Vector2Int(x, y);
            }
        }
    }


    // 입력받은 크기(size)에 맞는 첫 번째로 점유 가능한 슬롯의 위치를 반환
    // myinventory, target inventory 모두 적용가능
    private Vector2Int? FindFirstAvailableSlot(Vector2Int size, Dictionary<Vector2Int, Slot> slots, Vector2Int inventorySize)
    {
        for (int y = 1; y <= inventorySize.y; y++)
        {
            for (int x = 1; x <= inventorySize.x; x++)
            {
                Vector2Int location = new Vector2Int(x, y);

                // 현재 위치를 시작점으로 크기(size)에 맞게 점유 가능한지 확인
                if (ValidSlots(location, size, slots, inventorySize))
                {
                    return location; // 첫 번째 가능한 위치 반환
                }
            }
        }

        Debug.LogWarning("점유 가능한 슬롯을 찾을 수 없습니다.");
        return null; // 빈 공간 없음
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
