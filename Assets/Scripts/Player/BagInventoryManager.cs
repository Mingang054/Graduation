using UnityEngine;

public class BagInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;       // 슬롯 프리팹
    [SerializeField] private Transform inventoryPanel;    // 슬롯이 배치될 인벤토리 패널
    private const int gridWidth = 8;                      // 가로 슬롯 개수
    private const int gridHeight = 12;                     // 세로 슬롯 개수

    private Inventory inventory;                          // 논리적 인벤토리 구조

    private void Start()
    {
        inventory = new Inventory(gridWidth, gridHeight); // 논리 구조 초기화
        GenerateSlots();
    }

    // 슬롯 생성 메서드
    private void GenerateSlots()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject newSlot = Instantiate(slotPrefab, inventoryPanel);
                Slot slotScript = newSlot.GetComponent<Slot>();
                slotScript.Initialize(x, y, inventory); // 논리적 위치와 인벤토리 연결
            }
        }
    }

    //Save/Load 고려 필요

}
