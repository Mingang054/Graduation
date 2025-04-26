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


    public int magAmmoCount;// �̻��

    public int count; //���ǰ���
    // Weaponv
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
    // consumableData�� �̻��
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
            // Vest�� isGrip�κ� Ȯ���ϰ� ������ �۵�x
        }
        
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
    
}
