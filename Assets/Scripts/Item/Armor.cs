using UnityEngine;

public class Armor : ItemBase
{
    public float durability { get; private set; }  // 내구도

    public Armor(ArmorData data, int initialCount = 1, float initialDurability = 100)
        : base(data, initialCount)
    {
        this.durability = Mathf.Clamp(initialDurability, 0, 100);
    }

    // 내구도 감소
    public void DecreaseDurability(float amount)
    {
        durability = Mathf.Max(durability - amount, 0);
        if (durability == 0)
        {
            Debug.Log($"{data.itemName}이(가) 파손되었습니다.");
        }
    }

    // 방어구 착용
    public override void Use()
    {
        Debug.Log($"{data.itemName} 착용됨.");
    }
}
