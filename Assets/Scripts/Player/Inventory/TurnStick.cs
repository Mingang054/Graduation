using UnityEngine;
using UnityEngine.EventSystems;

public class TurnStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public AudioClip roopClip;

    public tourniquet tourniquetMain;

    public float requiredRotation = 990f; // 360 + 360 + 270
    public RectTransform targetUI;        // 회전시킬 UI
    public System.Action onTurnCompleted; // 회전 완료 시 호출할 외부 함수

    private bool isDragging = false;
    private float accumulatedRotation = 0f;
    private float prevMouseAngle = 0f;
    private bool isCompleted = false;

    public void OnEnable()
    {
        // 회전 초기화
        transform.rotation = Quaternion.identity;

        // 내부 상태 초기화
        isDragging = false;
        prevMouseAngle = 0f;
        accumulatedRotation = 0f;
        isCompleted = false;

        // 디버그 로그 (선택)
        Debug.Log("🌀 TurnStick 초기화 완료");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        prevMouseAngle = GetMouseAngle();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || isCompleted) return;

        float currentAngle = GetMouseAngle();
        float deltaAngle = Mathf.DeltaAngle(prevMouseAngle, currentAngle);

        targetUI.Rotate(0, 0, deltaAngle); // UI 자체 회전
        accumulatedRotation += Mathf.Abs(deltaAngle); // 절대값 누적

        prevMouseAngle = currentAngle;

        if (accumulatedRotation >= requiredRotation)
        {
            isCompleted = true;
            OnTurnCompleted(); // ✅ 회전 완료 처리
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private float GetMouseAngle()
    {
        // 1. UI 오브젝트 중심의 스크린 좌표
        Vector2 centerScreenPos = (Vector2)RectTransformUtility.WorldToScreenPoint(null, targetUI.position);

        // 2. 현재 마우스 위치
        Vector2 mousePos = (Vector2)Input.mousePosition;

        // 3. 중심 → 마우스로 향하는 방향 벡터
        Vector2 dir = mousePos - centerScreenPos;

        // 4. 각도 계산
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }



    private void OnTurnCompleted()
    {
        tourniquetMain.CompleteStick();
    }
}
