using UnityEngine;

public class Armor : ItemInstance
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

    // 내구도 설정
    public void SetDurability(float newDurability)
    {
        durability = Mathf.Clamp(newDurability, 0, 100);
        Debug.Log($"{data.itemName} 내구도 설정: {durability}");
    }
}
