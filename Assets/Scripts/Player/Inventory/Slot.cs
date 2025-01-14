using UnityEngine;

public class Slot : MonoBehaviour
{
    public int x; // 슬롯의 논리적 위치 (가로 인덱스)
    public int y; // 슬롯의 논리적 위치 (세로 인덱스)
    private Inventory inventory; // 연결된 인벤토리 논리 구조

    public void Initialize(int x, int y, Inventory inventory)
    {
        this.x = x;
        this.y = y;
        this.inventory = inventory;

        UpdateUI();
    }

    public void UpdateUI()
    {
        ItemInstance itemInstance = inventory.GetItem(x, y);
        if (itemInstance != null)
        {
            // 아이콘과 수량 표시
            Debug.Log($"슬롯 ({x}, {y})에 아이템: {itemInstance.item.data.itemName}, 수량: {itemInstance.item.count}");
        }
        else
        {
            // 슬롯 비우기
            Debug.Log($"슬롯 ({x}, {y}) 비어 있음");
        }
    }

    public void OnSlotClick()
    {
        Debug.Log($"슬롯 클릭: ({x}, {y})");
        ItemInstance itemInstance = inventory.GetItem(x, y);
        if (itemInstance != null)
        {
            Debug.Log($"아이템 사용: {itemInstance.item.data.itemName}");
            itemInstance.UseItem();
        }
    }
}
