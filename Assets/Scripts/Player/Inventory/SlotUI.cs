using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Vector2Int location;
    //아이템 배치 가능 여부 시각화를 위한 Image
    //해당 부분은 하나의 UI Imgage를 크기를 조절해가며 움직이는 것으로 구현시 Image와 MonoBehaviour 삭제
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
