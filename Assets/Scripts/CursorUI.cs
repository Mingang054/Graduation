using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private RectTransform cursorRT;
    [SerializeField] private Canvas canvas;           // MouseCursorCanvas

    [SerializeField] public Sprite aimCursor;
    [SerializeField] public Sprite uiCursor;

    [SerializeField] private Image image;

    private void Awake()
    {
        UnityEngine.Cursor.visible = false;
    }
    void Update()
    {
        // Overlay 캔버스일 땐 좌표 변환 필요 없이 바로 대입
        cursorRT.position = Input.mousePosition;
    }

    public void SetUIAsAimCursor()
    {
        image.sprite = aimCursor;
    }
    public void SetUIAsUICursor()
    {
        image.sprite = uiCursor;
    }
    public void SetUIAsOnLoad()
    {

    }
}
