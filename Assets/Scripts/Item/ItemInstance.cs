using UnityEngine;


//������ �κ��丮 ��� �����ϰ� �� ������ �ν��Ͻ��Դϴ�
public class ItemInstance
{
    public ItemBase item { get; private set; }  // ItemBase ����
    
    public ItemInstance(ItemBase item)

    {
        this.item = item;
    }

    // ���� ��ȯ
    public int GetCount()
    {
        return item.count;
    }

    // ������ ���
    public void UseItem()
    {
        item.Use();
    }
}