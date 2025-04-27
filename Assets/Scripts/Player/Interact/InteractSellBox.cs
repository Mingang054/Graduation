using UnityEngine;

public class InteractSellBox : Interactable
{
    public Loot boxForSell;  // 연결된 판매할 Loot 상자

    public void SellBox()
    {
        if (boxForSell == null || boxForSell.lootItems == null || boxForSell.lootItems.Count == 0)
        {
            Debug.Log("💬 판매할 아이템이 없습니다.");
            return;
        }

        float totalPrice = 0f;

        foreach (var item in boxForSell.lootItems)
        {
            if (item == null || item.data == null)
                continue;

            float itemPrice = item.data.price * item.count;   // 🔥 가격 × 수량
            totalPrice += itemPrice;
        }

        // 💵 최종 합산한 금액을 플레이어 돈에 추가
        PlayerStatus.instance.money += totalPrice;

        Debug.Log($"💰 {boxForSell.lootItems.Count}개 아이템 판매 완료! 총 수익: {totalPrice} 돈");

        // lootItems 비우기
        boxForSell.lootItems.Clear();
    }
}
