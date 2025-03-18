using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Vector2Int location;
    //������ ��ġ ���� ���� �ð�ȭ�� ���� Image
    //�ش� �κ��� �ϳ��� UI Imgage�� ũ�⸦ �����ذ��� �����̴� ������ ������ Image�� MonoBehaviour ����
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetLocation(Vector2Int newlocation)
    {
        location = newlocation;
    }
}
