using UnityEngine;
using UnityEngine.EventSystems;

public class HealItemButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int index;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("ffsdfaf");
            HealItemManager.instance.StartHealKit(index);
            
        }else 
        { 
            return; 
        }
    }
}
