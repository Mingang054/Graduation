using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text infoText;        // 🔥 하나로 통합된 Text
    public Button buyButton;

    private ItemData itemData;

    public void Setup(ItemData data)
    {
        itemData = data;

        // 🔥 텍스트 하나로 이름과 가격 모두 표시

        itemImage.sprite = itemData.itemSprite;
        infoText.text = $"Name: {itemData.itemName}\nPrice: {itemData.price}";

        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    private void OnBuyButtonClicked()
    {
        if (PlayerStatus.instance.money < itemData.price)
        {
            Debug.Log("❗ 돈이 부족합니다.");
            return;
        }

        var newItem = ItemFactory.CreateItem(new ItemInitData
        {
            itemCode = itemData.itemCode,
            count = 1,
            durability = 100f,
            loaded = false,
            magCount = 0
        });

        if (newItem == null)
        {
            Debug.LogError("❗ 아이템 생성 실패");
            return;
        }

        // 🔥 1. 가능한 슬롯 위치만 찾기
        var inventoryManager = BagInventoryManager.Instance;
        var size = newItem.data.size;
        var location = inventoryManager.FindFirstAvailableSlot(size, inventoryManager.mySlots, inventoryManager.myInventoryVector);

        if (location == null)
        {
            Debug.Log("❗ 인벤토리에 공간이 없습니다.");
            return;
        }

        // 🔥 2. 아이템 인벤토리 데이터에 추가
        //inventoryManager.myItems.Add(newItem);

        // 🔥 3. 슬롯 점유 처리
        //inventoryManager.FillSlots(newItem, location.Value, inventoryManager.mySlots, inventoryManager.myInventoryVector);

        // 🔥 4. ItemInstance에 위치 저장
        newItem.location = location.Value;

        // 🔥 5. ItemInstanceUI 생성 및 배치
        var itemUIObj = ItemUIPoolManager.Instance.GetItemUI(newItem);
        var itemUI = itemUIObj.GetComponent<ItemInstanceUI>();

        if (itemUI != null)
        {
            itemUI.itemInstance = newItem;
            itemUI.PlaceItem(location.Value, true);  // ✅ 여기서 안정적으로 배치
        }
        else
        {
            Debug.LogWarning("⚠️ 생성한 ItemUI가 null입니다.");
        }

        // 🔥 6. 돈 차감

        AudioManager.Instance.PlayCoin();
        PlayerStatus.instance.money -= itemData.price;
        Debug.Log($"✅ {itemData.itemName} 구매 완료!");
    }


}
