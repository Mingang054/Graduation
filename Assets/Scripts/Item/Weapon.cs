using UnityEngine;

public class Weapon : ItemBase
{
    public float durability { get; private set; }  // 내구도

    public Weapon(WeaponData data, int initialCount = 1, float initialDurability = 100)
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

    // 무기 사용
    public override void Use()
    {
        Debug.Log($"{data.itemName} 사용됨 (무기 발사)");
    }
}
