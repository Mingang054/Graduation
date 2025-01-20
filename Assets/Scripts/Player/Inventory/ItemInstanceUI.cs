using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInstanceUI : MonoBehaviour
{
    public Image itemImage; // 아이템 이미지
    public Text itemCountText; // 아이템 수량 텍스트
    private ItemInstance itemInstance; // 현재 UI가 나타내는 아이템 인스턴스

    // 아이템 UI 초기화
    public void Initialize(ItemInstance instance)
    {
        itemInstance = instance;
        Refresh(); // 초기화 시 UI를 업데이트
    }

    // UI 리프레시 메서드
    public void Refresh()
    {
        if (itemInstance != null)
        {
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

    // 드래그 시작 처리
    public void OnBeginDrag()
    {
        Debug.Log($"드래그 시작: {itemInstance.data.itemName}");
        // 드래그 시작 로직 추가
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
        // 드래그 종료 및 드롭 처리
    }

    // 클릭 처리
    public void OnClick()
    {
        Debug.Log($"아이템 클릭: {itemInstance.data.itemName}");
        // 클릭 동작 추가
    }
}
