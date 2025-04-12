using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WeaponOnVest : MonoBehaviour ,IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    public EquipSlotType equipslot;
    public bool IsEquiped ;

    public float triggerOffset = 40f;
    public float maxOffset = 50f;
    public UnityEvent onPulled;

    private RectTransform rt;
    private CanvasGroup cg;        // ← Raycast 차단 복원용
    private Vector2 originPos;
    private float offset;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        originPos = rt.anchoredPosition;   // ★ 여기서 원위치 캡처
        offset = 0f;
        if (cg) cg.blocksRaycasts = false; // EndDrag 보장
    }

    public void OnDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        offset = Mathf.Clamp(offset + e.delta.y, 0, maxOffset);
        rt.anchoredPosition = originPos + Vector2.up * offset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        if (offset >= triggerOffset)
            onPulled?.Invoke();

        rt.anchoredPosition = originPos;   // 즉시 복귀
        offset = 0f;
        if (cg) cg.blocksRaycasts = true;  // Raycast 복원
    }   


    public void UpdateUI()
    {
        if (IsEquiped)
        {
                    
        }
        else
        {

        }
        //
    }
}
