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

        float movement = isX ? eventData.delta.x : -eventData.delta.y; // Y는 아래로 내려갈수록 delta.y가 -임
        currentOffset += movement;

        // 이동 제한
        currentOffset = Mathf.Clamp(currentOffset, 0f, pullMaxOffset);

        // 실제 UI 위치 이동
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

        // 조건 충족 시 실행
        if (currentOffset >= pullOffset)
        {
            VestInventory.Instance.PullReceiverInVest();
            VestInventory.Instance.PullReceiverInVest();
            Debug.Log("리시버 당기기! 작동!");
            // 여기에 원하는 실행 코드 삽입
        }

        // 원래 위치로 복귀
        rectTransform.anchoredPosition = originalPosition;
        currentOffset = 0f;
    }
}
