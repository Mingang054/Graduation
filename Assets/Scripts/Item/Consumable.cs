using UnityEngine;

public class Consumable : ItemBase
{
    public Consumable(ConsumableData data, int initialCount = 1)
        : base(data, initialCount) { }

    // 소모품 사용
    public override void Use()
    {
        ConsumableData consumableData = (ConsumableData)data;
        Debug.Log($"{data.itemName} 사용! HP +{consumableData.hp}, SP +{consumableData.sp}");

        SetCount(count - 1);  // 수량 감소
    }
}
