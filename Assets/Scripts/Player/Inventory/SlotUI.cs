using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerEnterHandler
{
    public Vector2Int location;
    public bool isMySlot = new bool();
    //������ ��ġ ���� ���� �ð�ȭ�� ���� Image
    //�ش� �κ��� �ϳ��� UI Imgage�� ũ�⸦ �����ذ��� �����̴� ������ ������ Image�� MonoBehaviour ����
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
        // SlotUI ���� ���콺�� ������ �� ���� ���콺�� ��ġ�� Slot ����
        BagInventoryManager.Instance.currentPointedSlot = location;
        BagInventoryManager.Instance.currentPointedSlotIsMySlot = isMySlot;
        BagInventoryManager.Instance.currentPointedSlotIsEquip = false;

    }
    
    public bool GetIsMySlot() {
        // 
        return isMySlot;
    }
    
}
