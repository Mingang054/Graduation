using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


//코드 파생시켜서 다양한 상자로 만들기
public class Loot : Interactable        
{
    public List<ItemInstance> lootItems;
    
    private void Awake()
    {
        interactType = InteractType.Lootable;
        List<ItemInitData> itemToCreate = new List<ItemInitData>();
        //생성될 아이템 명시
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

        //아이템 생성
        foreach (var item in itemToCreate)
        {
            ItemInstance itemInstance = ItemFactory.CreateItem(item);
            if (itemInstance != null)
                lootItems.Add(itemInstance);
        }
        //리스트에 추가
        
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
