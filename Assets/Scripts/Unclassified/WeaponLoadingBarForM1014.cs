using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponLoadingBarForM1014 : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("�� ����")]
    public bool isX = true;

    [Header("¦������ �Բ� ������ UI")]
    public RectTransform linkedRect;           // �� �߰�
    private Vector2 linkedOriginalPos;         // �� �߰�

    public bool isStopped = false;

    [Header("������")]
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

    private void Start()                  // �� ���� �����: �ٸ� ������Ʈ�� ��� Awake �� ��
    {
        if (linkedRect != null)
            linkedOriginalPos = linkedRect.anchoredPosition;
    }

    /*������������������������������������������������ OnEnable ������������������������������������������������*/
    private void OnEnable()
    {
        currentOffset = isStopped ? pullMaxOffset : 0f;
        ApplyOffset(currentOffset);
    }

    /*������������������������������������������������ �巡�� �Է� ������������������������������������������*/
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
            VestInventory.Instance.PullReceiverInVest();   // Ʈ����

        // ����ġ ����
        currentOffset = 0f;
        ApplyOffset(0f);
    }

    /*������������������������������������������������ ��ư ���� ����������������������������������������������*/
    public void returnDrag()
    {
        if (!isStopped) return;

        VestInventory.Instance.PullReceiverInVest();
        currentOffset = 0f;
        ApplyOffset(0f);
    }

    /*������������������������������������������������  ���� ��ġ ����  ��������������������������������*/
    private void ApplyOffset(float offset)
    {
        // �� UI
        Vector2 newPos = originalPosition;
        if (isX) newPos.x += offset;
        else newPos.y -= offset;
        rectTransform.anchoredPosition = newPos;

        // ¦�� UI
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
