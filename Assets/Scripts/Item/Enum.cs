
//------
// ������ �з�

public enum ItemType
{
    Weapon,           // ���� (��)
    Armor,            // �� (��)
    Consumable,       // �Ҹ�ǰ
    SpecialConsumable,
    etc     // ��Ÿ ������
}



//------ ���� ------//

// ���� �з�
public enum WeaponCategory
{
    Pistol,           // ����
    SMG,              // �������
    AssaultRifle,     // ���ݼ���
    SniperRifle,      // ���ݼ���
    Shotgun,          // ��ź��
    LMG,              // �����
    GrenadeLauncher,  // ��ź
    Melee             // ���� ����
}

// ���� ���
public enum ReloadType
{
    Magazine,    // źâ (�� ���� ����)
    Single       // �� �߾� ����
}

// �߻� ���
public enum FireMode
{
    SemiAuto,    // ���ڵ�
    FullAuto     // �ڵ�
}

// ���� ���
public enum AttackMode
{
    Hitscan,        // ��Ʈ��ĵ
    Projectile,     // ����ü
    MultiProjectile // ���� ����ü (��ź��)
}

//ź��
public enum AmmoType
{
    Light,    // ���� ź�� (����, �������)
    Medium,   // ���� ź��
    Heavy,    // ���� ź��
    Anti,     // �����
    Shell,    // ��ź�� ź��
    Explosive // ��ź
}



//------ �� ------//
public enum ArmorSlot
{
    Head,   // �Ӹ�
    Body    // ����
}


//------ �Ҹ�ǰ Ÿ�� ------//
public enum ConsumableType
{
    Medicine,  // �Ǿ�ǰ
    Food       // ����
}

public enum EquipSlotType{

    head,
    body,
    firstWeapon,
    secondWeapon,
    thirdWeapon,
    none
}


//--- ��ȣ �ۿ� ---//

public enum InteractType
{
    Lootable,
    Trigger,
    none

}