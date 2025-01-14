using UnityEngine;

// 공통 아이템 속성을 가진 추상 클래스
[System.Serializable]
public abstract class ItemBase
{
    public ItemData data { get; private set; }  // ScriptableObject 데이터 참조
    public int count { get; private set; }      // 수량
    public float totalWeight => count * data.weight;  // 총 무게 계산

    public ItemBase(ItemData data, int initialCount = 1)
    {
        this.data = data;
        this.count = Mathf.Clamp(initialCount, 0, data.maxStack);
    }

    // 수량 설정
    public void SetCount(int count)
    {
        this.count = Mathf.Clamp(count, 0, data.maxStack);
    }

    // 아이템 사용 추상 메서드 (파생 클래스에서 구현)
    public abstract void Use();
}
