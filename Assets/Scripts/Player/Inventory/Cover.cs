using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("연동 대상")]
    [SerializeField] private Slider progressSlider; // 시각적 게이지용
    [SerializeField] private tourniquet tourniquetRef;


    [Header("드래그 설정")]
    [SerializeField] private float dragThreshold = 200f; // 실행 조건이 되는 드래그 거리(px)

    private Vector2 dragStartPos;
    private bool triggered = false;

    private void OnEnable()
    {
        progressSlider.value = 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
        triggered = false;

        if (progressSlider != null)
            progressSlider.value = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (triggered) return;

        float deltaX = eventData.position.x - dragStartPos.x;

        // 0 이하 드래그는 무시 (왼쪽으로 드래그한 경우)
        float clampedX = Mathf.Clamp(deltaX, 0f, dragThreshold);

        // ✅ 슬라이더는 진행률로 표시 (0 ~ 1)
        if (progressSlider != null)
            progressSlider.value = clampedX / dragThreshold;

        if (deltaX >= dragThreshold)
        {
            triggered = true;

            if (tourniquetRef != null)
                tourniquetRef.CompleteCover();

            Debug.Log("✅ 커버 드래그 완료!");
        }
        else
        {
            progressSlider.value = 0f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 선택: 실패 시 슬라이더 초기화 가능
        if (!triggered && progressSlider != null)
            progressSlider.value = 0f;
    }
}
