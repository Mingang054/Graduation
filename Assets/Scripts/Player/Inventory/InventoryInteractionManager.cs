using UnityEngine;

public class InventoryInteractionManager : MonoBehaviour
{
    public RectTransform currentGridParentRect; // ���� ���� ���� Grid RectTransform
    public Vector2 slotSize = new Vector2(96, 96); // ���� ũ��

    private GameObject dragUI; // �巡�� �� ǥ���� �ӽ� UI
    private Vector2Int originalSlotPosition; // �巡�� ���� ��ġ ����

    // �巡�� ���� ó��
    public void OnBeginDrag(Vector3 mousePosition, RectTransform gridParentRect)
    {
        currentGridParentRect = gridParentRect; // �巡�� ��� Grid ����
        originalSlotPosition = GetSlotPositionFromMouse(mousePosition); // �巡�� ���� ��ġ ����
        CreateDragUI(); // �巡�� UI ����
    }

    // �巡�� �� ó��
    public void OnDrag(Vector3 mousePosition)
    {
        if (dragUI != null)
        {
            dragUI.transform.position = mousePosition; // �巡�� UI�� ���콺 ��ġ�� �̵�
        }
    }

    // �巡�� ���� ó��
    public void OnEndDrag(Vector3 mousePosition)
    {
        Vector2Int dropSlotPosition = GetSlotPositionFromMouse(mousePosition); // ��� ��ġ ���

        if (IsSlotValid(dropSlotPosition)) // ��� ��ġ ��ȿ�� �˻�
        {
            Debug.Log($"�������� {dropSlotPosition} ��ġ�� ��ӵǾ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� �����Դϴ�. ���� ��ġ�� �����մϴ�.");
        }

        DestroyDragUI(); // �巡�� UI ����
    }

    // ���콺 ��ġ�κ��� ���� ��ǥ(Vector2Int) ���
    private Vector2Int GetSlotPositionFromMouse(Vector3 mousePosition)
    {
        if (currentGridParentRect == null)
        {
            Debug.LogError("currentGridParentRect�� �������� �ʾҽ��ϴ�.");
            return Vector2Int.zero;
        }

        // Vector3 -> Vector2 ��ȯ
        Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);
        Vector2 localPoint;

        // ���콺 ��ǥ�� Grid ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(currentGridParentRect, mousePos2D, null, out localPoint);

        // ���� ��ǥ ���
        int x = Mathf.FloorToInt(localPoint.x / slotSize.x);
        int y = Mathf.FloorToInt(localPoint.y / slotSize.y);

        return new Vector2Int(x, y);
    }

    // �巡�� UI ����
    private void CreateDragUI()
    {
        Debug.Log("�巡�� UI ����");
        // �巡�� UI ���� ���� �߰�
    }

    // �巡�� UI ����
    private void DestroyDragUI()
    {
        if (dragUI != null)
        {
            Destroy(dragUI);
            dragUI = null;
        }
    }

    // ���� ��ȿ�� �˻� (�ӽ÷� �׻� true ��ȯ)
    private bool IsSlotValid(Vector2Int slotPosition)
    {
        return true; // ��ȿ�� �˻� ���� �߰� �ʿ�
    }
}
