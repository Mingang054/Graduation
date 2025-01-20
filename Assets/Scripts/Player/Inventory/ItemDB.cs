using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static ItemDB Instance { get; private set; }

    private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();

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

    private void LoadAllItems()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("itemTable");  // Assets\itemTable �������� �ε�
        foreach (ItemData item in allItems)
        {
            if (!itemDictionary.ContainsKey(item.itemCode))
            {
                itemDictionary.Add(item.itemCode, item);
                Debug.Log($"������ ���: {item.itemName}, �ڵ�: {item.itemCode}");
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
}
