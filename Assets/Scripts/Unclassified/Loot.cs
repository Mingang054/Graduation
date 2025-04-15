using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


//�ڵ� �Ļ����Ѽ� �پ��� ���ڷ� �����
public class Loot : Interactable        
{
    public List<ItemInstance> lootItems;
    
    private void Awake()
    {
        interactType = InteractType.Lootable;
        List<ItemInitData> itemToCreate = new List<ItemInitData>();
        //������ ������ ���
        itemToCreate.Add(new ItemInitData
        {
            itemCode = "W001",
            count = 1,
            location = new Vector2Int(4, 4),
            durability = 80f,
            loaded = false,
            magCount = 25

});
        itemToCreate.Add(new ItemInitData
        {
            itemCode = "F202",
            count = 1,
            location = new Vector2Int(6, 6),
            durability = 80f,
            loaded = false
        });
        itemToCreate.Add(new ItemInitData
        {
            itemCode = "W002",
            count = 1,
            location = new Vector2Int(1, 11),
            durability = 80f,
            loaded = false,
            magCount = 12
        });

        //������ ����
        foreach (var item in itemToCreate)
        {
            ItemInstance itemInstance = ItemFactory.CreateItem(item);
            if (itemInstance != null)
                lootItems.Add(itemInstance);
        }
        //����Ʈ�� �߰�
        
    }

    public List<ItemInstance> GetListOfLoot()
    {
        return lootItems;
    }
    public string GetLootName()
    {
        return interactionName;
    }


    public void DeleteAll() {
        return;
    }







}
