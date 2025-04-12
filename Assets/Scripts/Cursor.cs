using UnityEngine;

public class UICursorFollower : MonoBehaviour
{
    [SerializeField] private RectTransform cursorRT;
    [SerializeField] private Canvas canvas;           // MouseCursorCanvas

    void Update()
    {
        Vector2 pos = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            pos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPos);

        cursorRT.anchoredPosition = localPos;
    }
}
