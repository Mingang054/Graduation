using UnityEngine;
using UnityEngine.UI;

public class ItemInstanceUI : MonoBehaviour
{
    public Image itemImage; // 아이템 이미지
    public Text itemCountText; // 아이템 수량 텍스트
    private ItemInstance itemInstance; // 현재 UI가 나타내는 아이템 인스턴스
    private RectTransform rectTransform; // UI 위치를 조정할 RectTransform

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 아이템이 존재할 경우, UI 위치 및 크기를 초기화
        if (itemInstance != null)
        {
            UpdateUI();
            UpdateSize(); // UI 크기 설정
        }
    }

    // 아이템 UI 초기화
    public void Initialize(ItemInstance instance)
    {
        itemInstance = instance;
        UpdateUI(); // UI 업데이트
        UpdatePosition(itemInstance.location); // UI 위치 초기화
        UpdateSize(); // UI 크기 설정
    }

    // UI 리프레시 메서드
    public void UpdateUI()
    {
        if (itemInstance != null)
        {
            UpdatePosition(itemInstance.location);
            UpdateSize(); // 크기 조정 추가

            // 아이템 스프라이트 설정
            itemImage.sprite = itemInstance.data.itemSprite;
            itemImage.enabled = true;

            // 수량 텍스트 업데이트
            itemCountText.text = itemInstance.count > 1 ? itemInstance.count.ToString() : ""; // 1개일 때는 표시 안 함
        }
        else
        {
            // 아이템이 없으면 UI 초기화
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemCountText.text = "";
        }
    }

    // UI 위치 업데이트 (로컬 좌표 기준)
    public void UpdatePosition(Vector2Int location)
    {
        if (rectTransform == null)
        {
            Debug.LogWarning("RectTransform이 존재하지 않습니다.");
            return;
        }

        // 새로운 UI 위치 설정 (Pivot이 맞춰져 있으므로 위치값만 변경)
        rectTransform.anchoredPosition = new Vector2(location.x * 96-96, -location.y * 96+96);
    }

    // UI 크기 업데이트
    public void UpdateSize()
    {
        if (rectTransform == null)
        {
            Debug.LogWarning("RectTransform이 존재하지 않습니다.");
            return;
        }

        if (itemInstance != null)
        {
            // 아이템 크기 = size * 96
            rectTransform.sizeDelta = new Vector2(itemInstance.data.size.x * 96, itemInstance.data.size.y * 96);
            Debug.Log($"UI 크기 업데이트: {rectTransform.sizeDelta}");
        }
    }






    // 드래그 시작 처리
    public void OnBeginDrag()
    {
        Debug.Log($"드래그 시작: {itemInstance.data.itemName}");
    }

    // 드래그 중 처리
    public void OnDrag(Vector3 mousePosition)
    {
        transform.position = mousePosition; // 드래그 UI 이동
    }

    // 드래그 종료 처리
    public void OnEndDrag(Vector3 mousePosition)
    {
        Debug.Log($"드래그 종료: {itemInstance.data.itemName}");
    }

    // 클릭 처리
    public void OnClick()
    {
        Debug.Log($"아이템 클릭: {itemInstance.data.itemName}");
    }
}
