using UnityEngine;

public class Consumable : ItemInstance
{
    public Consumable(ConsumableData data, int initialCount = 1)
        : base(data, initialCount)
    {
    }


    // ��� ó��
    public void Use()
    {
        
        Debug.LogWarning($"{data.itemName}��(��) �� �̻� ����� �� �����ϴ�.");
        
    }
}
