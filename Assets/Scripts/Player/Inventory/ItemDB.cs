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
        ItemData[] allItems = Resources.LoadAll<ItemData>("itemTable");  // Assets/itemTable ����

        foreach (ItemData item in allItems)
        {
            if (!itemDictionary.ContainsKey(item.itemCode))
            {
                itemDictionary.Add(item.itemCode, item);
                Debug.Log($"������ ���: {item.itemName}, �ڵ�: {item.itemCode}");

                // �ڵ� ù ���ڷ� �з�
                if (item.itemCode.StartsWith("W"))
                    weaponItems.Add(item);
                else if (item.itemCode.StartsWith("F"))
                    foodItems.Add(item);
                else if (item.itemCode.StartsWith("H"))
                    healItems.Add(item);
            }
            else
            {
                Debug.LogWarning($"�ߺ��� ������ �ڵ� �߰�: {item.itemCode}");
            }
        }
    }

    public ItemData GetItemDataByCode(string itemCode)
    {
        if (itemDictionary.TryGetValue(itemCode, out ItemData item))
        {
            return item;
        }
        Debug.LogWarning($"������ �ڵ� {itemCode}�� �ش��ϴ� �������� ã�� �� �����ϴ�.");
        return null;
    }

    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDictionary.Values);
    }
}
