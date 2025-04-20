using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private RectTransform cursorRT;
    [SerializeField] private Canvas canvas;           // MouseCursorCanvas

    [SerializeField] public Sprite aimCursor;
    [SerializeField] public Sprite uiCursor;

    [SerializeField] public Sprite uiCursor_mag;
    [SerializeField] public Sprite[] uiCursor_shell;
    [SerializeField] public Sprite uiCursor_;   

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

    public void UpdateVestCursor( int isGripCount, VestPlacable? origin )
    {
        if (origin == null || isGripCount <=0 ) { image.sprite = uiCursor; return; }

        if (origin.placeableType == VestPlaceableType.Mag)
        {
            switch (origin.ammoType)
            {
                case AmmoType.Light:
                    break;
                case AmmoType.Medium:
                    image.sprite = uiCursor_mag;
                    break;
                case AmmoType.Heavy:
                    break;
                case AmmoType.Anti:
                    break;
                case AmmoType.Shell:
                    if (uiCursor_shell.Length <= isGripCount)
                    {
                        image.sprite = uiCursor_shell[isGripCount - 1];
                    }    
                    break;
                default:
                    image.sprite = uiCursor;
                    break;
            }
        }
    }
}
