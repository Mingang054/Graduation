using UnityEngine;

public class InventoryInteractionManager : MonoBehaviour
{
    public RectTransform currentGridParentRect; // 현재 조작 중인 Grid RectTransform
    public Vector2 slotSize = new Vector2(96, 96); // 슬롯 크기

    private GameObject dragUI; // 드래그 중 표시할 임시 UI
    private Vector2Int originalSlotPosition; // 드래그 시작 위치 저장

    // 드래그 시작 처리
    public void OnBeginDrag(Vector3 mousePosition, RectTransform gridParentRect)
    {
        currentGridParentRect = gridParentRect; // 드래그 대상 Grid 설정
        originalSlotPosition = GetSlotPositionFromMouse(mousePosition); // 드래그 시작 위치 저장
        CreateDragUI(); // 드래그 UI 생성
    }

    // 드래그 중 처리
    public void OnDrag(Vector3 mousePosition)
    {
        if (dragUI != null)
        {
            dragUI.transform.position = mousePosition; // 드래그 UI를 마우스 위치로 이동
        }
    }

    // 드래그 종료 처리
    public void OnEndDrag(Vector3 mousePosition)
    {
        Vector2Int dropSlotPosition = GetSlotPositionFromMouse(mousePosition); // 드롭 위치 계산

        if (IsSlotValid(dropSlotPosition)) // 드롭 위치 유효성 검사
        {
            Debug.Log($"아이템이 {dropSlotPosition} 위치에 드롭되었습니다.");
        }
        else
        {
            Debug.LogWarning("유효하지 않은 슬롯입니다. 원래 위치로 복귀합니다.");
        }

        DestroyDragUI(); // 드래그 UI 제거
    }

    // 마우스 위치로부터 슬롯 좌표(Vector2Int) 계산
    private Vector2Int GetSlotPositionFromMouse(Vector3 mousePosition)
    {
        if (currentGridParentRect == null)
        {
            Debug.LogError("currentGridParentRect가 설정되지 않았습니다.");
            return Vector2Int.zero;
        }

        // Vector3 -> Vector2 변환
        Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);
        Vector2 localPoint;

        // 마우스 좌표를 Grid 내부 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(currentGridParentRect, mousePos2D, null, out localPoint);

        // 슬롯 좌표 계산
        int x = Mathf.FloorToInt(localPoint.x / slotSize.x);
        int y = Mathf.FloorToInt(localPoint.y / slotSize.y);

        return new Vector2Int(x, y);
    }

    // 드래그 UI 생성
    private void CreateDragUI()
    {
        Debug.Log("드래그 UI 생성");
        // 드래그 UI 생성 로직 추가
    }

    // 드래그 UI 제거
    private void DestroyDragUI()
    {
        if (dragUI != null)
        {
            Destroy(dragUI);
            dragUI = null;
        }
    }

    // 슬롯 유효성 검사 (임시로 항상 true 반환)
    private bool IsSlotValid(Vector2Int slotPosition)
    {
        return true; // 유효성 검사 로직 추가 필요
    }
}
