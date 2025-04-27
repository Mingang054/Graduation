using UnityEngine;

[CreateAssetMenu(fileName = "NewMagazine", menuName = "Items/Magazine")]
public class MagazineData : ItemData
{
    public WeaponAType ammoType;         // 탄약 종류 (무기와 호환성 검사)
    public int ammoCapacity;          // 최대 탄약 수
}   
