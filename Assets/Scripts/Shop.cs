using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Transform contentParent;        // ScrollView의 Content 부모 (UI 부모)
    public GameObject itemSlotPrefab;       // 아이템 한 칸 프리팹 (버튼, 텍스트 포함)

    private void Start()
    {
        GenerateShopItems();
    }

    private void GenerateShopItems()
    {
        var allItems = ItemDB.Instance.GetAllItems();   // 🔥 이 함수 추가해야 함

        foreach (var itemData in allItems)
        {
            if (itemData.forSale)
            {
                var itemSlotObj = Instantiate(itemSlotPrefab, contentParent);
                var itemSlotUI = itemSlotObj.GetComponent<ShopItemUI>();

                if (itemSlotUI != null)
                {
                    itemSlotUI.Setup(itemData);
                }
            }
        }
    }
}
