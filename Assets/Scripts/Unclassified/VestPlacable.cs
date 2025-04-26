using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VestPlacable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public VestPlaceableType placeableType;
    [SerializeField]
    public VestSpriteSet spriteSet;


    public int magAmmoCount;// 미사용

    public int count; //물건개수
    // Weaponv
    public int magMax;      //2개
    //public int magCount;    //실제 개수
    public AmmoType ammoType;
    /*
        Light,    // 소형 탄약 (권총, 기관단총)
        Medium,   // 중형 탄약
        Heavy,    // 대형 탄약
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
                case AmmoType.Shell:
                    image.sprite = spriteSet.shellSprite[count];
                    break;
                case AmmoType.Light:
                    image.sprite = spriteSet.lightSprite[count];
                    break;
                case AmmoType.Medium:
                    image.sprite = spriteSet.mediumSprite[count];
                    break;
                case AmmoType.Heavy:
                    image.sprite = spriteSet.heavySprite[count];
                    break;
                case AmmoType.Anti:
                    image.sprite = spriteSet.shellSprite[count];
                    break;
                case AmmoType.Explosive:
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
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            VestInventory.Instance.GetGrip(this);
        }else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Vest의 isGrip부분 확인하고 맞으면 작동x
        }
        
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
    
}
