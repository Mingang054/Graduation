using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
        // 마우스 커서 숨기기
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        RefreshCursor();
    }

    private void RefreshCursor()
    {
        // 마우스 위치 가져오기 (스크린 좌표)
        Vector2 mouseScreenPosition = Input.mousePosition;

        // 마우스 좌표를 월드 좌표로 변환
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // 오브젝트 위치를 마우스 위치로 갱신
        transform.position = mouseWorldPosition;
    }
}
