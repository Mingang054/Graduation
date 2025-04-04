using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


//코드 파생시켜서 다양한 상자로 만들기
public class Loot : MonoBehaviour
{
    public List<ItemInstance> lootItems;
    
    private void Start()
    {

        List<ItemInitData> itemToCreate = new List<ItemInitData>();
        //생성될 아이템 명시
        itemToCreate.Add(new ItemInitData
        {
            itemCode = "W101",
            count = 1,
            location = new Vector2Int(4, 4),
            durability = 80f,
            loaded = false,
            loadedIsAP = false,
            magazineData = new List<bool> { true, false, false, true }
        });
        itemToCreate.Add(new ItemInitData
        {
            itemCode = "F202",
            count = 1,
            location = new Vector2Int(6, 6),
            durability = 80f,
            loaded = false,
            loadedIsAP = false,
            magazineData = new List<bool> { true, false, false, true }
        });
        itemToCreate.Add(new ItemInitData
        {
            itemCode = "W102",
            count = 1,
            location = new Vector2Int(1, 11),
            durability = 80f,
            loaded = false,
            loadedIsAP = false,
            magazineData = new List<bool> { true, false, false, true }
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



    public void DeleteAll() {
        return;
    }







}
