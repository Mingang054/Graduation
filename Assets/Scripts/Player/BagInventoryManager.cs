using UnityEngine;

public class BagInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;       // ���� ������
    [SerializeField] private Transform inventoryPanel;    // ������ ��ġ�� �κ��丮 �г�
    private const int gridWidth = 8;                      // ���� ���� ����
    private const int gridHeight = 12;                     // ���� ���� ����

    private Inventory inventory;                          // ���� �κ��丮 ����

    private void Start()
    {
        inventory = new Inventory(gridWidth, gridHeight); // �� ���� �ʱ�ȭ
        GenerateSlots();
    }

    // ���� ���� �޼���
    private void GenerateSlots()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject newSlot = Instantiate(slotPrefab, inventoryPanel);
                Slot slotScript = newSlot.GetComponent<Slot>();
                slotScript.Initialize(x, y, inventory); // ���� ��ġ�� �κ��丮 ����
            }
        }
    }

    //Save/Load ��� �ʿ�

}
