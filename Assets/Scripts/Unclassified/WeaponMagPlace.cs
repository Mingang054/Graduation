using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponMagPlace : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {   
            
            VestInventory.Instance.LoadAmmo();
        }
    }

}
