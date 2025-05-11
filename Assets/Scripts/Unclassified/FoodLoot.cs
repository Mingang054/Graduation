using System.Collections.Generic;
using UnityEngine;

public class FoodLoot : Loot
{

    private void Awake()
    {
        interactType = InteractType.Lootable;

        int itemCount = Random.Range(3, 7);  // ���� ����: 3~6��
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < itemCount; i++)
        {
            // 1. ���� ������ ����
            List<ItemData> candidates = ItemDB.Instance.foodItems;
            if (candidates.Count == 0) return;

            ItemData chosen = candidates[Random.Range(0, candidates.Count)];

            // 2. ��ġ �ߺ� ���� ����
            Vector2Int pos;
            do
            {
                pos = new Vector2Int(Random.Range(1, 9), Random.Range(1, 13)); // x:1~8, y:1~12
            } while (usedPositions.Contains(pos));

            usedPositions.Add(pos);

            // 3. ItemInitData�� ����
            ItemInitData data = new ItemInitData
            {
                itemCode = chosen.itemCode,
                count = Random.Range(1, 3),
                location = pos,
                durability = 100f
            };

            ItemInstance instance = ItemFactory.CreateItem(data);
            if (instance != null)
            {
                lootItems.Add(instance);
            }
        }
    }
}

