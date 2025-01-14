using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Items/Armor")]
public class ArmorData : ItemData
{
    public float defense;            // 방어력
    public float durabilityMax;         // 최대 내구도
    public float wearRate;           // 마모 계수
    public ArmorSlot armorSlot;      // 장착 부위 (머리, 상의 등)
}
