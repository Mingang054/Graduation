using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerEnterHandler
{
    public Vector2Int location;
    public bool isMySlot = new bool();
    //아이템 배치 가능 여부 시각화를 위한 Image
    //해당 부분은 하나의 UI Imgage를 크기를 조절해가며 움직이는 것으로 구현시 Image와 MonoBehaviour 삭제
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetLocation(Vector2Int newlocation)
    {
        location = newlocation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // SlotUI 위를 마우스로 포인팅 시 현재 마우스가 위치한 Slot 갱신
        BagInventoryManager.Instance.currentPointedSlot = location;
        BagInventoryManager.Instance.currentPointedSlotIsMySlot = isMySlot;
        BagInventoryManager.Instance.currentPointedSlotIsEquip = false;

    }
    
    public bool GetIsMySlot() {
        // 
        return isMySlot;
    }
    
}
