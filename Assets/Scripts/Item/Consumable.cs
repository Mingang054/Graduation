using UnityEngine;

public class Consumable : ItemInstance
{
    public Consumable(ConsumableData data, int initialCount = 1)
        : base(data, initialCount)
    {
    }


    // 사용 처리
    public bool Use()
    {
        if (PlayerStatus.instance.UseConsumable(data as ConsumableData))
        {
            SetCount(count-1);
            if (count <= 0) { return true; } else
            {
                return false;
            }
        }
        //PlayerStatus.instace
        return false;
    }
}
