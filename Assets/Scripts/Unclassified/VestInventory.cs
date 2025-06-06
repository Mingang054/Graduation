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
    //싱글톤
    public static VestInventory Instance;//{ get; private set; }
    [SerializeField]
    public PlayerShooter shooter;

    public CursorUI cursorUI;

    // 관리용
    private InputAction rightClickAction;
    public Transform hand;

    //상호작용 대상
    public PlayerStatus playerStatus;
    public BagInventoryManager BIM;

    public WeaponOnHand weaponOnHand; //current
    public WeaponOnHand weaponOnHand1;
    public WeaponOnHand weaponOnHand2;
    public WeaponOnHand weaponOnHand3;

    public VestPlacable PLeft;
    public VestPlacable PCenter;
    public VestPlacable PRight;
    public VestPlacable SOne;
    public VestPlacable SLeft;
    public VestPlacable SRight;



    [SerializeField]
    public WeaponOnVest firstWeaponOnVest;
    [SerializeField]
    public WeaponOnVest secondWeaponOnVest;
    [SerializeField]
    public WeaponOnVest thirdWeaponOnVest;

    //
    // if 우클릭시 false처리
    public bool isAmmoFill = false;



    //손에 쥔 대상
    public bool isGripAction = false;
    public bool isGrip = false;
    public int isGripCount = 0;
    public int isGripCountMax = 2;
    public VestPlaceableType gripTargetType = VestPlaceableType.none;
    //손에 쥔 대상이 원래 속한 위치
    public VestPlacable originVestPlacable = null;
    public Stack<VestPlacable> originPlaceabkeStack = new Stack<VestPlacable>();
    //public Stack<Vestplacable> originplacable; //2개 잡기용

    //
    [SerializeField] public GameObject healUI;

    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
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
                    //수정, Shell 대신 장전 방식 single인지 체크
                } else if (isGripCount < isGripCountMax && gripTarget.ammoType == WeaponAType.Shell)
                {
                    originPlaceabkeStack.TryPeek(out VestPlacable origin);
                    if (originVestPlacable != null && origin.ammoType == WeaponAType.Shell)
                    {
                        originPlaceabkeStack.Push(gripTarget);
                        gripTarget.count--;

                        isGrip = true;
                        isGripCount++;

                        gripTarget.UpdateUI();
                    }
                }
                else { Debug.Log("Empty"); }
                break;

            case VestPlaceableType.Medical:
                if(UIManager.Instance.currentSecondaryUI== null)
                healUI.SetActive(true);
                UIManager.Instance.currentSecondaryUI = healUI;
                Debug.Log("의료품 슬롯입니다.");
                break;

            case VestPlaceableType.Docs:
                //문서 UI
                Debug.Log("문서 슬롯입니다.");
                break;

            case VestPlaceableType.Radio:
                //무전 UI
                Debug.Log("무전기 슬롯입니다.");
                break;

            case VestPlaceableType.none:
                Debug.Log("아무것도 장착되지 않은 슬롯입니다.");
                break;

            default:
                Debug.LogWarning("정의되지 않은 슬롯 타입입니다.");
                break;
        }
        Debug.Log($"isGrip {isGrip}");
        Debug.Log($"isGripCount {isGripCount}");

        UpdateCursor();
    }

    public void LoadAmmo()
    {
        WeaponData a = weaponOnHand.currentWeapon.data as WeaponData;

        switch (a.ammoType)
        {
            default:        //통상 방식
                if (!weaponOnHand.currentWeapon.isMag)  //탄창이 없으면 바로 탄창 삽입 
                {
                    if (isGrip && originVestPlacable.placeableType ==VestPlaceableType.Mag
                        && originVestPlacable.ammoType == a.ammoType)
                    {
                            //미구현파트 WeaponOnHand 연동
                        
                        weaponOnHand.currentWeapon.isMag = true;
                        weaponOnHand.currentWeapon.magCount = a.magCountMax;
                        AudioManager.Instance.PlaySFX(a.loadClip);  //소리재생
                        UseGrip();
                    }
                }
                else                                    //탄창이 있으면 탄창을 버림
                {
                    weaponOnHand.currentWeapon.isMag = false;
                    weaponOnHand.currentWeapon.magCount = 0;
                    //탄 회수 부분 추가
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
            Debug.Log(vp);
            if (vp.count < vp.magMax)
                vp.count++;
            isGripCount--;
            vp.UpdateUI(); // 여기서도 UI 갱신 필요할 수 있음
        }

        originVestPlacable = null;
        isGrip = false;
        originPlaceabkeStack.Clear();
        UpdateCursor();

    }



    public void UseGrip()   //VestPlacable 좌클릭 시 참조되어 사용
    {
        isGripCount--;
        if (isGripCount <= 0) {CancelGrip();}

        UpdateCursor();
    }

    public void UsePlayerAction() {
        
    }

    

    private void OnEnable()
    {
        // 새로운 액션 생성
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

        // 원하는 동작 실행
    }


    public void EquipWeaponInVest()
    {
        //화면 UI 변경용
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

    //우클릭 처리

    public void initVest()
    {
        CancelGrip();
    }
    private void UpdateCursor()
    {
        cursorUI.UpdateVestCursor(isGripCount,originVestPlacable);
    }





}
