using UnityEngine;

public class Consumable : ItemInstance
{
    public Consumable(ConsumableData data, int initialCount = 1)
        : base(data, initialCount)
    {
    }


    // 사용 처리
    public void Use()
    {
        
        Debug.LogWarning($"{data.itemName}을(를) 더 이상 사용할 수 없습니다.");
        
    }
}
