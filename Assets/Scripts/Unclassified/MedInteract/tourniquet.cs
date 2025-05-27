using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class tourniquet : MonoBehaviour
{

    public AudioClip endClip;
    public Sprite first;
    public Sprite second;
    public Sprite last;
    public GameObject stick;
    public GameObject cover;



    public bool stickFlag = false;
    //스틱이 돌려졌는지 아닌지를 기록하는 플래그

    public bool coverFlag = false;


    private void OnEnable()
    {
        stickFlag = false;
        GetComponent<Image>().sprite = first;
        stick.SetActive(true);
        cover.SetActive(false);
    }
    private void OnDisable()
    {

        HealItemManager.instance.HealBase.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void CompleteStick()
    {
        if (!stickFlag)
        {
            stickFlag = true;
            stick.SetActive(false);
            cover.SetActive(true);
            GetComponent<Image>().sprite = second;
        }

    }
    public void CompleteCover()
    {
        if (stickFlag)
        {
            GetComponent<Image>().sprite = last;
            AudioManager.Instance.PlaySFX(endClip);
        }

        HealItemManager.instance.UseHealKit();
        HealItemManager.instance.EndHealKit();

    }

    public void UseTourniquet()
    {

    }


}
