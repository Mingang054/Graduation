using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInstanceUI : MonoBehaviour
{
    public Image itemImage; // ������ �̹���
    public Text itemCountText; // ������ ���� �ؽ�Ʈ
    private ItemInstance itemInstance; // ���� UI�� ��Ÿ���� ������ �ν��Ͻ�

    // ������ UI �ʱ�ȭ
    public void Initialize(ItemInstance instance)
    {
        itemInstance = instance;
        Refresh(); // �ʱ�ȭ �� UI�� ������Ʈ
    }

    // UI �������� �޼���
    public void Refresh()
    {
        if (itemInstance != null)
        {
            // ������ ��������Ʈ ����
            itemImage.sprite = itemInstance.data.itemSprite;
            itemImage.enabled = true;

            // ���� �ؽ�Ʈ ������Ʈ
            itemCountText.text = itemInstance.count > 1 ? itemInstance.count.ToString() : ""; // 1���� ���� ǥ�� �� ��
        }
        else
        {
            // �������� ������ UI �ʱ�ȭ
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemCountText.text = "";
        }
    }

    // �巡�� ���� ó��
    public void OnBeginDrag()
    {
        Debug.Log($"�巡�� ����: {itemInstance.data.itemName}");
        // �巡�� ���� ���� �߰�
    }

    // �巡�� �� ó��
    public void OnDrag(Vector3 mousePosition)
    {
        transform.position = mousePosition; // �巡�� UI �̵�
    }

    // �巡�� ���� ó��
    public void OnEndDrag(Vector3 mousePosition)
    {
        Debug.Log($"�巡�� ����: {itemInstance.data.itemName}");
        // �巡�� ���� �� ��� ó��
    }

    // Ŭ�� ó��
    public void OnClick()
    {
        Debug.Log($"������ Ŭ��: {itemInstance.data.itemName}");
        // Ŭ�� ���� �߰�
    }
}
