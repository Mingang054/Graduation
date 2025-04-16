using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VestInventory : MonoBehaviour
{
    //�̱���
    public static VestInventory Instance { get; private set; }
    [SerializeField]
    public PlayerShooter shooter;


    // ������
    private InputAction rightClickAction;
    public Transform hand;

    //��ȣ�ۿ� ���
    public PlayerStatus playerStatus;
    public BagInventoryManager BIM;

    public WeaponOnHand weaponOnHand; //current
    public WeaponOnHand weaponOnHand1;
    public WeaponOnHand weaponOnHand2;
    public WeaponOnHand weaponOnHand3;


    [SerializeField]
    public WeaponOnVest firstWeaponOnVest;
    [SerializeField]
    public WeaponOnVest secondWeaponOnVest;
    [SerializeField]
    public WeaponOnVest thirdWeaponOnVest;

    //�տ� �� ���
    public bool isGripAction = false;
    public bool isGrip = false;
    public int isGripCount = 0;
    public int isGripCountMax = 2;
    public VestPlaceableType gripTargetType = VestPlaceableType.none;
    //�տ� �� ����� ���� ���� ��ġ
    public VestPlacable originVestPlacable = null;
    public Stack<VestPlacable> originPlaceabkeStack = new Stack<VestPlacable>();
    //public Stack<Vestplacable> originplacable; //2�� ����

    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }

        /*if (BIM == null)
        {
            BIM = BagInventoryManager.Instance;
        }*/

    }
    public void GetGrip(VestPlacable gripTarget)
    {

        switch (gripTarget.placeableType)
        {
            case VestPlaceableType.Mag:
                if (!isGrip && gripTarget.count > 0)
                {

                    originPlaceabkeStack.Push(gripTarget);
                    originVestPlacable = gripTarget;
                    originVestPlacable.count--;

                    isGrip = true;
                    isGripCount++;

                    gripTarget.UpdateUI();
                    //����, Shell ��� ���� ��� single���� üũ
                } else if (isGripCount < isGripCountMax && gripTarget.ammoType == AmmoType.Shell)
                {
                    originPlaceabkeStack.TryPeek(out VestPlacable origin);
                    if (originVestPlacable != null && origin.ammoType == AmmoType.Shell)
                    {
                        originPlaceabkeStack.Push(gripTarget);
                        originVestPlacable.count--;

                        isGrip = true;
                        isGripCount++;

                        gripTarget.UpdateUI();
                    }
                }
                else { Debug.Log("Empty"); }
                break;

            case VestPlaceableType.Medical:
                Debug.Log("�Ƿ�ǰ �����Դϴ�.");
                break;

            case VestPlaceableType.Docs:
                //���� UI
                Debug.Log("���� �����Դϴ�.");
                break;

            case VestPlaceableType.Radio:
                //���� UI
                Debug.Log("������ �����Դϴ�.");
                break;

            case VestPlaceableType.none:
                Debug.Log("�ƹ��͵� �������� ���� �����Դϴ�.");
                break;

            default:
                Debug.LogWarning("���ǵ��� ���� ���� Ÿ���Դϴ�.");
                break;
        }
        Debug.Log($"isGrip {isGrip}");
        Debug.Log($"isGripCount {isGripCount}");
        Debug.Log($"isGrip {originVestPlacable.placeableType}");

    }

    public void LoadAmmo()
    {
        WeaponData a = weaponOnHand.currentWeapon.data as WeaponData;
        switch (a.ammoType)
        {
            default:        //��� ���
                if (!weaponOnHand.currentWeapon.isMag)  //źâ�� ������ �ٷ� źâ ���� 
                {
                    if (isGrip)
                    {
                            //�̱�����Ʈ WeaponOnHand ����
                        
                        weaponOnHand.currentWeapon.isMag = true;
                        weaponOnHand.currentWeapon.magCount = a.magCountMax;
                        AudioManager.Instance.PlaySFX(a.loadClip);  //�Ҹ����
                        UseGrip();
                    }
                }
                else                                    //źâ�� ������ źâ�� ����
                {
                    weaponOnHand.currentWeapon.isMag = false;
                    weaponOnHand.currentWeapon.magCount = 0;
                    //ź ȸ�� �κ� �߰�
                }
                break;
        }
        weaponOnHand.UpdateUI();
        
        
        
    }

    public void PullReceiverInVest()
    {
        //WeaponOnHand
        if (weaponOnHand != null && weaponOnHand.currentWeapon != null)
        {
            if (weaponOnHand.currentWeapon.data is WeaponData weaponData)
            {
                    AudioManager.Instance.PlaySFX(weaponData.loadbarClip);

            }

            weaponOnHand.currentWeapon.PullReceiver();
        }
    }


    public void CancelGrip()
    {
        while (isGripCount > 0 && originPlaceabkeStack.TryPop(out var vp))
        {
            if (vp.count < vp.magMax)
                vp.count++;
            isGripCount--;
            vp.UpdateUI(); // ���⼭�� UI ���� �ʿ��� �� ����
        }

        originVestPlacable = null;
        isGrip = false;
        originPlaceabkeStack.Clear();
        
    }



    public void UseGrip()   //VestPlacable ��Ŭ�� �� �����Ǿ� ���
    {
        isGripCount--;
        if (isGripCount <= 0) {CancelGrip();}
    }

    public void UsePlayerAction() {
        
    }

    

    private void OnEnable()
    {
        // ���ο� �׼� ����
        rightClickAction = new InputAction(
            name: "RightClick",
            type: InputActionType.Button,
            binding: "<Mouse>/rightButton"
        );

        rightClickAction.performed += OnRightClick;
        rightClickAction.Enable();
    }

    private void OnDisable()
    {
        rightClickAction.performed -= OnRightClick;
        rightClickAction.Disable();
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        if (isGrip)
            CancelGrip();

        // ���ϴ� ���� ����
    }


    public void EquipWeaponInVest()
    {
        
    }

    public void SwapWeapon(WeaponOnVest onVest)
    {
        if (onVest.IsEquiped)
        {
            if (weaponOnHand != null)
            {
                weaponOnHand.gameObject.SetActive(false);
            }

            switch (onVest.equipslot)
            {
                case EquipSlotType.firstWeapon:
                    if (BagInventoryManager.Instance.firstWeapon != null)
                    {
                        weaponOnHand = weaponOnHand1;
                    }
                    break;
                case EquipSlotType.secondWeapon:
                    if (BagInventoryManager.Instance.secondWeapon != null)
                    {
                        weaponOnHand = weaponOnHand2;
                    }
                    break;
                case EquipSlotType.thirdWeapon:
                    if (BagInventoryManager.Instance.thirdWeapon!= null)
                    {
                        weaponOnHand = weaponOnHand3;
                    }
                    break;
                default:
                    break;
            }
            weaponOnHand.gameObject.SetActive(true);
            shooter.SetWeapon(weaponOnHand.currentWeapon);
        
        }
        
    }



    //��Ŭ�� ó��
}
