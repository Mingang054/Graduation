using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VestPlacable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public VestPlaceableType placeableType;

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
    public ConsumableData consumableData;
    //consumableData.


    public Image image;
    public Sprite sprite;

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
                    break;
                case AmmoType.Light:
                    break;
                case AmmoType.Medium:
                    break;
                case AmmoType.Heavy:
                    break;
                case AmmoType.Anti:
                    break;
                case AmmoType.Explosive:
                    break;

                default: break;
            }
        }else if (placeableType == VestPlaceableType.Medical)
        {

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
}
