using UnityEngine;

public class Slot : MonoBehaviour
{
    public int x; // ������ ���� ��ġ (���� �ε���)
    public int y; // ������ ���� ��ġ (���� �ε���)
    private Inventory inventory; // ����� �κ��丮 �� ����

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
            // �����ܰ� ���� ǥ��
            Debug.Log($"���� ({x}, {y})�� ������: {itemInstance.item.data.itemName}, ����: {itemInstance.item.count}");
        }
        else
        {
            // ���� ����
            Debug.Log($"���� ({x}, {y}) ��� ����");
        }
    }

    public void OnSlotClick()
    {
        Debug.Log($"���� Ŭ��: ({x}, {y})");
        ItemInstance itemInstance = inventory.GetItem(x, y);
        if (itemInstance != null)
        {
            Debug.Log($"������ ���: {itemInstance.item.data.itemName}");
            itemInstance.UseItem();
        }
    }
}
