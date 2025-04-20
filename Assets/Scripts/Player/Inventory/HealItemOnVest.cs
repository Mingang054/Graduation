using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealItemOnVest : MonoBehaviour
{
    
    public GameObject[] healButton = new GameObject[6];
    public TMP_Text[] text = new TMP_Text[6];
    public HealItemButton[] healItemButtonsa = new HealItemButton[6];

    public void UpdateButtonImages()
    {
        var HIM = HealItemManager.instance;
        for (int i = 0; i<6; i++)
        {
           if ( HIM.consumables[i] != null)
            {
                healButton[i].SetActive(true);
                healButton[i].GetComponent<Image>().sprite = HIM.consumables[i].data.itemSprite;
                text[i].text = HIM.consumables[i].count.ToString();
            }
            else
            {
                healButton[i].GetComponent<Image>().sprite = null;
                text[i].text = "";
                healButton[i].SetActive(false);
            }

        }
    }

    private void OnEnable()
    {
        UpdateButtonImages();   
    }
}
