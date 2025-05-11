
//------
// 아이템 분류

public enum ItemType
{
    Weapon,           // 무기 (총)
    Armor,            // 옷 (방어구)
    Consumable,       // 소모품
    SpecialConsumable,
    etc     // 기타 아이템
}



//------ 무기 ------//

// 무기 분류
public enum WeaponCategoryLegacy
{
    Pistol,           // 권총
    SMG,              // 기관단총
    AssaultRifle,     // 돌격소총
    SniperRifle,      // 저격소총
    Shotgun,          // 산탄총
    LMG,              // 기관총
    GrenadeLauncher,  // 유탄
    Melee             // 근접 무기
}

// 장전 방식
public enum ReloadType
{
    Magazine,    // 탄창 (한 번에 장전)
    Single       // 한 발씩 장전
}

// 발사 방식
public enum FireMode
{
    SemiAuto,    // 반자동
    FullAuto     // 자동
}

// 공격 방식
public enum AttackMode
{
    Hitscan,        // 히트스캔
    Projectile,     // 투사체
    MultiProjectile // 다중 투사체 (산탄총)
}

//탄종,
public enum WeaponAType
{
    Pistol,
    Light,    // 소형 탄약 (권총, 기관단총)
    Medium,   // 중형 탄약
    Heavy,    // 대형 탄약
    Anti,     // 대대형
    Shell,    // 산탄총 탄약
    Magnum,     //리볼버 탄약
    Explosive // 유탄
}



//------ 방어구 ------//
public enum ArmorSlot
{
    Head,   // 머리
    Body    // 상의
}


//------ 소모품 타입 ------//
public enum ConsumableType
{
    Medicine,  // 의약품
    Food       // 음식
}

public enum EquipSlotType{

    head,
    body,
    firstWeapon,
    secondWeapon,
    thirdWeapon,
    none
}


//--- 상호 작용 ---//

public enum InteractType
{
    Lootable,
    Trigger,
    Extraction,
    none

}

public enum VestPlaceableType
{
    Mag,
    Medical,
    Docs,
    Radio,
    none
}