using UnityEngine;


//실제로 인벤토리 등에서 소지하게 될 아이템 인스턴스입니다
public class ItemInstance
{
    public ItemBase item { get; private set; }  // ItemBase 참조
    
    public ItemInstance(ItemBase item)

    {
        this.item = item;
    }

    // 수량 반환
    public int GetCount()
    {
        return item.count;
    }

    // 아이템 사용
    public void UseItem()
    {
        item.Use();
    }
}