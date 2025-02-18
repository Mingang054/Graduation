using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
        // ���콺 Ŀ�� �����
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        RefreshCursor();
    }

    private void RefreshCursor()
    {
        // ���콺 ��ġ �������� (��ũ�� ��ǥ)
        Vector2 mouseScreenPosition = Input.mousePosition;

        // ���콺 ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // ������Ʈ ��ġ�� ���콺 ��ġ�� ����
        transform.position = mouseWorldPosition;
    }
}
