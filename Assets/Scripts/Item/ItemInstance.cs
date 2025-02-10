using UnityEngine;

// 공통 아이템 속성을 가진 클래스
[System.Serializable]
public class ItemInstance
{
    public Vector2Int location;
    public ItemData data { get; private set; }  // ScriptableObject 데이터 참조
    public int count { get; private set; }      // 수량
    public float totalWeight => count * data.weight;  // 총 무게 계산

    public ItemInstanceUI UIReference; // UI 참조 추가

    public ItemInstance(ItemData data, int initialCount = 1)
    {
        this.location = new Vector2Int(0, 0);
        this.data = data;
        this.count = Mathf.Clamp(initialCount, 0, data.maxStack);
        this.UIReference = null; // 초기에는 UI가 없으므로 null
    }

    // 수량 설정
    public void SetCount(int count)
    {
        this.count = Mathf.Clamp(count, 0, data.maxStack);
    }
}
