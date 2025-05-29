using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VestPlacable : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public VestPlaceableType placeableType;
    [SerializeField]
    public VestSpriteSet spriteSet;
    //꾹 누르기
    public HoldTimer holdTimer;


    public int magAmmoCount;

    public int count; //물건개수
    // Weaponv
    public int magMax;      //2개
    //public int magCount;    //실제 개수
    public WeaponAType ammoType;
    /*
        Light,    // 소형 mag light ammo
        Medium,   // 중형 mag 
        Heavy,    // 대형 mag heavy ammo
        Anti,     // 대대형
        Shell,    // 산탄총 탄약
        Explosive // 유탄
    */

    //Consumable - Medicine
    //public int medCount;
    // consumableData도 미사용
    //public ConsumableData consumableData;
    //consumableData.


    public Image image;
    public Sprite sprite;

    public void Awake()
    {
        image = GetComponent<Image>();
    }
    public void Start()
    {
        UpdateUI();
    }

    public VestPlaceableType GetPlaceableType()
    {

        return placeableType;
    }
    public void SetPlaceableType(VestPlaceableType newType)
    {

        placeableType = newType;
    }

    public void UpdateUI()
    {
        if (placeableType == VestPlaceableType.Mag)
        {
            switch (ammoType)
            {
                case WeaponAType.Shell:
                    image.sprite = spriteSet.shellSprite[count];
                    break;
                case WeaponAType.Light:
                    image.sprite = spriteSet.pistolSprite[count];
                    break;
                case WeaponAType.Medium:
                    if (count >= 0 && count < spriteSet.mediumSprite.Length)
                        image.sprite = spriteSet.mediumSprite[count];
                    else
                        Debug.LogWarning("[UpdateUI] mediumSprite 인덱스 초과");
                    break;
                case WeaponAType.Heavy:
                    image.sprite = spriteSet.heavySprite[count];
                    break;
                case WeaponAType.Anti:
                    image.sprite = spriteSet.shellSprite[count];
                    break;
                case WeaponAType.Explosive:
                    image.sprite = spriteSet.shellSprite[count];
                    break;

                default: break;
            }
        }else if (placeableType == VestPlaceableType.Medical)
        {
            return;
        }
        //UP 갱신
        
    }


    public void SetMag(bool isAp) {//X

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if( VestInventory.Instance.isAmmoFill) { return; }
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            VestInventory.Instance.GetGrip(this);
        }else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Vest의 isGrip부분 확인하고 맞으면 작동x
        }
        
        
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (placeableType != VestPlaceableType.Mag) { return; }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            holdTimer.StartHold(this); // 🔥 내 자신을 넘겨줘
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (placeableType != VestPlaceableType.Mag) { return; }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            holdTimer.StopHold();
        }
    }

    public bool OnHoldTick()
    {
        Debug.Log("tick");

        var am = AmmunitionManager.instance;

            int need = 0;
        switch (ammoType)
        {
            case WeaponAType.Pistol:
                            need = 30 - magAmmoCount;
                if (need > 10) need = 15;
                if (am.lightAmmo < need) need = am.lightAmmo;

                if (need > 0)
                {
                    am.lightAmmo -= need;
                    magAmmoCount += need;

                    if (magAmmoCount > 30)
                        count = 2;
                    else if (magAmmoCount > 0)
                        count = 1;
                    else
                        count = 0;
                    am.UpdateAmmo();
                    UpdateUI();
                }

                // 여기서 조건 판단
                if (magAmmoCount >= 30 || am.mediumAmmo <= 0)
                    return true; // 🔥 멈춰야 한다
                else
                    return false; // 계속 진행

            case WeaponAType.Light:
                need = 60 - magAmmoCount;
                if (need > 10) need = 15;
                if (am.lightAmmo < need) need = am.lightAmmo;

                if (need > 0)
                {
                    am.lightAmmo -= need;
                    magAmmoCount += need;

                    if (magAmmoCount > 30)
                        count = 2;
                    else if (magAmmoCount > 0)
                        count = 1;
                    else
                        count = 0;
                    am.UpdateAmmo();
                    UpdateUI();
                }

                // 여기서 조건 판단
                if (magAmmoCount >= 60 || am.mediumAmmo <= 0)
                    return true; // 🔥 멈춰야 한다
                else
                    return false; // 계속 진행

            case WeaponAType.Medium:
                int medneed = 60 - magAmmoCount;
                if (medneed > 10) medneed = 10;
                if (am.mediumAmmo < medneed) medneed = am.mediumAmmo;

                if (medneed > 0)
                {
                    am.mediumAmmo -= medneed;
                    magAmmoCount += medneed;

                    if (magAmmoCount > 30)
                        count = 2;
                    else if (magAmmoCount > 0)
                        count = 1;
                    else
                        count = 0;

                    am.UpdateAmmo();
                    UpdateUI();
                }

                // 여기서 조건 판단
                if (magAmmoCount >= 60 || am.mediumAmmo <= 0)
                    return true; // 🔥 멈춰야 한다
                else
                    return false; // 계속 진행

            case WeaponAType.Shell:
                if (am.shellAmmo > 0 && am.shellAmmo < 8)
                {
                    am.shellAmmo--;
                    count++;
                    am.UpdateAmmo();
                    UpdateUI();
                }
                if (count>=8 || am.shellAmmo <=0)
                { return true; }
                else
                // shell은 아직 멈출 조건 따로 없음
                return false;

            default:
                return false;
        }
    }


}
