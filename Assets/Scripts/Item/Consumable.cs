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
        //PlayerStatus.instace
        
    }
}
