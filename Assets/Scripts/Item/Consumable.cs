using UnityEngine;

public class Consumable : ItemBase
{
    public Consumable(ConsumableData data, int initialCount = 1)
        : base(data, initialCount) { }

    // �Ҹ�ǰ ���
    public override void Use()
    {
        ConsumableData consumableData = (ConsumableData)data;
        Debug.Log($"{data.itemName} ���! HP +{consumableData.hp}, SP +{consumableData.sp}");

        SetCount(count - 1);  // ���� ����
    }
}
