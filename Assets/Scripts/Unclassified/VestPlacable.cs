using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VestPlacable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public VestPlaceableType placeableType { get; private set; }

    public int count; //���ǰ���
    // Weapon
    public int magMax;      //2��
    //public int magCount;    //���� ����
    public AmmoType ammoType;
    /*
        Light,    // ���� ź�� (����, �������)
        Medium,   // ���� ź��
        Heavy,    // ���� ź��
        Anti,     // �����
        Shell,    // ��ź�� ź��
        Explosive // ��ź
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
        //UP ����
        
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
            //
        }
        
        
    }
}
