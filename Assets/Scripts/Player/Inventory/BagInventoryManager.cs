using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.FilePathAttribute;

public class BagInventoryManager : MonoBehaviour
{
    
    [SerializeField] private CanvasScaler canvasScaler;

    public Vector2Int currentPointedSlot = new Vector2Int(-1, -1);
    public bool currentPointedSlotIsMySlot = false;
    public bool currentPointedSlotIsEquip = false;
    public EquipmentSlotUI currentPointedEquipSlot = null;
    
    
    //-- 현재 사용중인 무기
    public EquipSlotType currentUsingWeapon;
    public Weapon currentWeapon;

    //-- EquipmentSlot 관리 --// Serialize
    public Transform firstWeaponUI;
    public Transform secondWeaponUI;
    public Transform thirdWeaponUI; //pistol타입만 허용
    public Transform headArmorUI;
    public Transform bodyArmorUI;
    //-- Equipweapon
    public Weapon firstWeapon;
    public Weapon secondWeapon;
    public Weapon thirdWeapon; //pistol타입만 허용


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

    //--SlotUI 접근 (location 확인용)--//
    public Dictionary<Vector2Int, SlotUI> mySlotsUI = new Dictionary<Vector2Int, SlotUI>();
    public Dictionary<Vector2Int, SlotUI> opponentSlotsUI = new Dictionary<Vector2Int, SlotUI>();

    [SerializeField]
    private GameObject slotUIPrefab;

    //싱글톤 패턴 적용
    public static BagInventoryManager Instance { get; private set; }
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }


        if (canvasScaler == null)
            canvasScaler = GetComponentInParent<CanvasScaler>();

        if (slotUIPrefab != null && myInventoryGrid!=null && opponentInventoryGrid!=null)
        {
            InitSlotUI();
        }
        InitMySlots();
        InitOpSlots();


    }


    private void Start()
    {
        ResetOpponentItems();
    }


    //---SlotUI 초기화---///
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
    //--- Inventory 초기화 ---//
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
                Debug.Log($"내 슬롯 추가됨: {position}");
            }
        }
    }


    //--- MyInventory 읽어오기 ---//(미구현)

    public void LoadMyInventory()       //인자 json
    {
        ResetMyItems();
        myItems = new List<ItemInstance>();
        //json파일 파싱
        SetMyItems(myItems);
    }

    public void ResetMyItems()  //Load 단계 외 구동 금지
    {

        // 1) 슬롯 점유 해제
        for (int y = 1; y <= myInventoryVector.y; y++)
        {
            for (int x = 1; x <= myInventoryVector.x; x++)
            {
                mySlots[new Vector2Int(x, y)].SetOccupied(false);
            }
        }

        // 2) opponentInventory 하위 자식 오브젝트 반환 (한 단계만)
        foreach (Transform child in myInventory)
        {
            // 오브젝트가 ItemInstanceUI 컴포넌트를 가지고 있는지 확인
            ItemInstanceUI ui = child.GetComponent<ItemInstanceUI>();
            if (ui != null)
            {
                ItemUIPoolManager.Instance.ReturnItemUI(child.gameObject);
            }
        }

        // 3) opponentItems 참조 제거
        myItems = null;
    }
    
    public void SetMyItems(List<ItemInstance> setItems)
    {

        ResetMyItems();
        //기존 등록된 Item 해제
        for (int y = 1; y <= myInventoryVector.y; y++)
        {
            for (int x = 1; x <= myInventoryVector.x; x++)
            {
                mySlots[new Vector2Int(x, y)].SetOccupied(false);
            }
        }
        myItems = setItems;
        //ItemPoolManager 연동 파트
        LoadMyItemsUI();
        //미구현
    }
    public void LoadMyItemsUI()
    {
        if (myItems == null) return;

        foreach (var itemInstance in myItems)
        {
            // 이미 리스트에 존재하는 아이템이므로, 위치만 복구
            OccupySlots(itemInstance.location, itemInstance.data.size, mySlots);

            // UI 연결
            GameObject obj = ItemUIPoolManager.Instance.GetItemUI(itemInstance);
            obj.transform.SetParent(myInventory);
            ItemInstanceUI itemInstanceUI = obj.GetComponent<ItemInstanceUI>();
            itemInstanceUI.UpdateUI();
            obj.SetActive(true);
        }

    }





    //--- opponentInventory 읽어오기 ---//
    public void ResetOpponentItems()
    {
        // 1) 슬롯 점유 해제
        for (int y = 1; y <= opponentInventoryVector.y; y++)
        {
            for (int x = 1; x <= opponentInventoryVector.x; x++)
            {
                opponentSlots[new Vector2Int(x, y)].SetOccupied(false);
            }
        }

        // 2) opponentInventory 하위 자식 오브젝트 반환 (한 단계만)
        foreach (Transform child in opponentInventory)
        {
            // 오브젝트가 ItemInstanceUI 컴포넌트를 가지고 있는지 확인
            ItemInstanceUI ui = child.GetComponent<ItemInstanceUI>();
            if (ui != null)
            {
                ItemUIPoolManager.Instance.ReturnItemUI(child.gameObject);
            }
        }

        // 3) opponentItems 참조 제거
        opponentItems = null;
    }

    public void SetOpponentItems(List<ItemInstance> setItems)
    {
        ResetOpponentItems();
        //기존 등록된 Item 해제
        for (int y = 1; y<= opponentInventoryVector.y; y++)
        {
            for(int x = 1; x<= opponentInventoryVector.x; x++)
            {
                opponentSlots[new Vector2Int(x, y)].SetOccupied(false);
            }
        }
        opponentItems = setItems;
        //ItemPoolManager 연동 파트
        LoadOpponentItemsUI();
        //미구현
    }

    //-- opponentInvneoty에 있는 아이템 UI로 로딩 --//
    public void LoadOpponentItemsUI()
    {
        if (opponentItems == null) return;

        foreach (var itemInstance in opponentItems)
        {
            // 이미 리스트에 존재하는 아이템이므로, 위치만 복구
            OccupySlots(itemInstance.location, itemInstance.data.size, opponentSlots);

            // UI 연결
            GameObject obj = ItemUIPoolManager.Instance.GetItemUI(itemInstance);
            obj.transform.SetParent(opponentInventory);
            ItemInstanceUI itemInstanceUI = obj.GetComponent<ItemInstanceUI>();
            itemInstanceUI.UpdateUI();
            obj.SetActive(true);
        }
    }




    //--- Slot 관리 ---//

    //item이동시 사용 (초기화X)
    public bool PlaceItemInSlot(ItemInstance itemInstance, Vector2Int location,
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

    public void FreeItemSlots(ItemInstance itemInstance)
    {
        Vector2Int location = itemInstance.location;
        Vector2Int size = itemInstance.data.size;

        // 1) 아이템이 내 인벤토리에 속해 있었다면 mySlots 해제
        if (myItems.Contains(itemInstance))
        {
            FreeSlots(location, size, mySlots);
        }
        // 2) 상대 인벤토리에 속해 있었다면 opponentSlots 해제
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
        Debug.Log($"[OccupySlots] 점유 시작: 시작 위치 = {location}, 크기 = {size}");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (slots.TryGetValue(position, out var slot))
                {
                    slot.SetOccupied(true);
                    Debug.Log($"[OccupySlots] 점유 슬롯: {position}");
                }
                else
                {
                    Debug.LogWarning($"[OccupySlots] 슬롯 없음: {position}");
                }
            }
        }
    }

    private void FreeSlots(Vector2Int location, Vector2Int size, Dictionary<Vector2Int, Slot> targetSlots)
    {
        Debug.Log($"[FreeSlots] 점유 해제 시작: 위치 = {location}, 크기 = {size}");

        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (targetSlots.TryGetValue(pos, out var slot))
                {
                    slot.SetOccupied(false);
                    Debug.Log($"[FreeSlots] 해제된 슬롯: {pos}");
                }
                else
                {
                    Debug.LogWarning($"[FreeSlots] 슬롯 없음: {pos}");
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
                // 인벤토리 범위를 넘으면 무시
                if (x > inventorySize.x || y > inventorySize.y)
                {
                    Debug.LogWarning($"[ValidSlots] 검사 제외: 범위 초과 위치 ({x},{y})");
                    continue;
                }

                Vector2Int pos = new Vector2Int(x, y);
                Debug.Log($"[ValidSlots] 검사 중인 슬롯 위치: {pos}");

                if (!slots.TryGetValue(pos, out var slot))
                {
                    Debug.LogWarning($"[ValidSlots] 슬롯 정보 없음: {pos}");
                    return false;
                }

                if (slot.GetOccupied())
                {
                    Debug.LogWarning($"[ValidSlots] 슬롯 점유 중: {pos}");
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

    public void SetOpponentList(List<ItemInstance> lootList) {
        opponentItems = new List<ItemInstance>();
        foreach(ItemInstance item in opponentItems)
        {
            //itemInstanceUI 생성 및 Pool에서 디큐
        }
    }



    //-- 스왑 구현(미구현)

    public void SetPlayerWeapon()
    {
        //currentUsingWeapon = ;
        //currentWeapon = ;
    }
    public void SetPlayerArmor()
    {
        //단순 합연산
    }



    public float CellSize
    {
        get
        {
            // CanvasScaler가 Scale With Screen Size 일 때,
            // scaleFactor 가 현재 해상도 대비 배율(1.0 = 기준 해상도)
            float scale = canvasScaler != null
                          ? canvasScaler.scaleFactor   // 권장
                          : CanvasScaleFallback();

            return 70f * scale; 
        }
    }

    private float CanvasScaleFallback()
    {
        // 최상단 Canvas의 scaleFactor 가져오기 (예비용)
        var rootCanvas = GetComponentInParent<Canvas>();
        return rootCanvas != null ? rootCanvas.scaleFactor : 1f;
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