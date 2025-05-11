using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static ItemDB Instance { get; private set; }

    private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();

    public List<ItemData> weaponItems = new List<ItemData>();
    public List<ItemData> foodItems = new List<ItemData>();
    public List<ItemData> healItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllItems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadAllItems()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("itemTable");  // Assets/itemTable 폴더

        foreach (ItemData item in allItems)
        {
            if (!itemDictionary.ContainsKey(item.itemCode))
            {
                itemDictionary.Add(item.itemCode, item);
                Debug.Log($"아이템 등록: {item.itemName}, 코드: {item.itemCode}");

                // 코드 첫 글자로 분류
                if (item.itemCode.StartsWith("W"))
                    weaponItems.Add(item);
                else if (item.itemCode.StartsWith("F"))
                    foodItems.Add(item);
                else if (item.itemCode.StartsWith("H"))
                    healItems.Add(item);
            }
            else
            {
                Debug.LogWarning($"중복된 아이템 코드 발견: {item.itemCode}");
            }
        }
    }

    public ItemData GetItemDataByCode(string itemCode)
    {
        if (itemDictionary.TryGetValue(itemCode, out ItemData item))
        {
            return item;
        }
        Debug.LogWarning($"아이템 코드 {itemCode}에 해당하는 아이템을 찾을 수 없습니다.");
        return null;
    }

    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDictionary.Values);
    }
}
