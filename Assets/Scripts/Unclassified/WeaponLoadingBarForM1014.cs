using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponLoadingBarForM1014 : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("축 선택")]
    public bool isX = true;

    [Header("짝꿍으로 함께 움직일 UI")]
    public RectTransform linkedRect;           // ← 추가
    private Vector2 linkedOriginalPos;         // ← 추가

    public bool isStopped = false;

    [Header("오프셋")]
    public float pullOffset = 5f;
    public float pullMaxOffset = 6f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private float currentOffset = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Start()                  // ← 순서 보장용: 다른 오브젝트가 모두 Awake 된 뒤
    {
        if (linkedRect != null)
            linkedOriginalPos = linkedRect.anchoredPosition;
    }

    /*──────────────────────── OnEnable ────────────────────────*/
    private void OnEnable()
    {
        currentOffset = isStopped ? pullMaxOffset : 0f;
        ApplyOffset(currentOffset);
    }

    /*──────────────────────── 드래그 입력 ─────────────────────*/
    public void OnBeginDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;
        currentOffset = 0f;
    }

    public void OnDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        float movement = isX ? e.delta.x : -e.delta.y;
        currentOffset = Mathf.Clamp(currentOffset + movement, 0f, pullMaxOffset);

        ApplyOffset(currentOffset);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        if (VestInventory.Instance.weaponOnHand.currentWeapon.magCount == 0)
        {
            isStopped = true;
            return;
        }

        if (currentOffset >= pullOffset)
            VestInventory.Instance.PullReceiverInVest();   // 트리거

        // 원위치 복귀
        currentOffset = 0f;
        ApplyOffset(0f);
    }

    /*──────────────────────── 버튼 복귀 ───────────────────────*/
    public void returnDrag()
    {
        if (!isStopped) return;

        VestInventory.Instance.PullReceiverInVest();
        currentOffset = 0f;
        ApplyOffset(0f);
    }

    /*────────────────────────  공통 위치 적용  ────────────────*/
    private void ApplyOffset(float offset)
    {
        // 주 UI
        Vector2 newPos = originalPosition;
        if (isX) newPos.x += offset;
        else newPos.y -= offset;
        rectTransform.anchoredPosition = newPos;

        // 짝꿍 UI
        if (linkedRect != null)
        {
            Vector2 lPos = linkedOriginalPos;
            if (isX) lPos.x += offset;
            else lPos.y -= offset;
            linkedRect.anchoredPosition = lPos;
        }
    }

    public void InsertWithIsStopped()
    {
        if (isStopped == true)
        {

            VestInventory.Instance.LoadAmmo(); 
            returnDrag();
        }
    }
}
