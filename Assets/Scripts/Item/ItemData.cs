using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public Vector2Int size;
    public string itemName;                   // 아이템 이름
    public string itemCode;                   // 아이템 코드  4자리 16진수 테스트용 첫자리 F
    public Sprite itemSprite;                 // 아이템 아이콘
    public int price;                         // 가격
    public float weight;                      // 기본 무게
    public int maxStack;                      // 최대 겹침 수량
    public ItemType itemType;                 // 아이템 타입 (열거형)
    public string description;         // 아이템 설명
}
