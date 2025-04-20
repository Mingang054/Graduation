using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class injector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private Vector2 originalPosition;
    private float currentOffset = 0f;

    [SerializeField]
    public AudioClip injectorClip;
    [SerializeField]
    public AudioClip capClip;

    public float minY = -70f;
    public float maxY = 0f;
    [SerializeField] private float dragThreshold = 60f;

    public bool isCap = true;
    public GameObject cap;

    private void OnEnable()
    {
        isCap = true;
        cap.SetActive(true);
        if (rectTransform != null)
            rectTransform.anchoredPosition = originalPosition;
    }

    private void OnDisable()
    {
        rectTransform.anchoredPosition = originalPosition;
        isCap = true;
        cap.SetActive(true);
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        currentOffset = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        float movement = -eventData.delta.y;
        currentOffset += movement;
        currentOffset = Mathf.Clamp(currentOffset, 0f, dragThreshold);

        Vector2 newPosition = originalPosition;
        newPosition.y = Mathf.Clamp(originalPosition.y - currentOffset, minY, maxY);
        rectTransform.anchoredPosition = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (currentOffset >= dragThreshold && !isCap)
        {
            Debug.Log("💉 인젝터 작동!");
            UseInjector();
            // 트리거 실행
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
            Debug.Log("🔄 원래 위치로 복귀");
        }

        currentOffset = 0f;
    }

    public void UseInjector()
    {
        rectTransform.anchoredPosition = originalPosition;
        AudioManager.Instance.PlaySFX(injectorClip);
        isCap = true;
        cap.SetActive(true);
        HealItemManager.instance.UseHealKit();
        HealItemManager.instance.EndHealKit();
        this.gameObject.SetActive(false);
    }

    public void RemoveCap()
    {
        AudioManager.Instance.PlaySFX(capClip);
        isCap = false;
        cap.SetActive(false);
    }
}
