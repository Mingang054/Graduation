using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

// 공통 아이템 속성을 가진 클래스
[System.Serializable]
public class ItemInstance
{
    public Vector2Int location;
    public EquipmentSlotUI currentEquipSlotUI = null; //장착된 부위 역참조
    public EquipSlotType currentEquipSlotType = EquipSlotType.none;

    public ItemData data { get; private set; }  // ScriptableObject 데이터 참조
    public int count { get; private set; }      // 수량
    public float totalWeight => data != null ? count * data.weight : 0f;    // 총 무게 계산


    public ItemInstance(ItemData data, int initialCount = 1)
    {
        this.location = new Vector2Int(0, 0);
        this.data = data;
        this.count = Mathf.Clamp(initialCount, 0, data.maxStack);
    }

    // 수량 설정
    public ItemInstance SetItemInstance(ItemData data, int count)
    {
        this.data = data;
        this.count = count;
        return this;
    }
    public void ResetData()
    {
        this.data = null;
        this.count = 0;
        this.location = Vector2Int.zero;
        currentEquipSlotUI = null;
        currentEquipSlotType = EquipSlotType.none;
    }

    public void SetCount(int count)
    {
        this.count = Mathf.Clamp(count, 0, data.maxStack);
        
    }
}
