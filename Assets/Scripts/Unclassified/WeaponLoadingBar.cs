using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponLoadingBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isX = true;
    public float pullOffset = 5f;
    public float pullMaxOffset = 6f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private float currentOffset = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        currentOffset = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        float movement = isX ? eventData.delta.x : -eventData.delta.y; // Y�� �Ʒ��� ���������� delta.y�� -��
        currentOffset += movement;

        // �̵� ����
        currentOffset = Mathf.Clamp(currentOffset, 0f, pullMaxOffset);

        // ���� UI ��ġ �̵�
        Vector2 newPosition = originalPosition;
        if (isX)
            newPosition.x += currentOffset;
        else
            newPosition.y -= currentOffset;

        rectTransform.anchoredPosition = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // ���� ���� �� ����
        if (currentOffset >= pullOffset)
        {
            VestInventory.Instance.PullReceiverInVest();
            VestInventory.Instance.PullReceiverInVest();
            Debug.Log("���ù� ����! �۵�!");
            // ���⿡ ���ϴ� ���� �ڵ� ����
        }

        // ���� ��ġ�� ����
        rectTransform.anchoredPosition = originalPosition;
        currentOffset = 0f;
    }
}
