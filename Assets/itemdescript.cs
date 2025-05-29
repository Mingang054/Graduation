using TMPro;
using UnityEngine;

public class itemdescript : MonoBehaviour
{
    public static itemdescript itemDescription;

    public void Awake()
    {

        itemDescription = GetComponent<itemdescript>();
        gameObject.SetActive(false);
    }

    public TMP_Text name_value;
    public TMP_Text description;


    public void SetText(string itemName, int price, string itemDesc)
    {
        if (name_value != null)
            name_value.text = $"{itemName}  <size=70%><color=#bbbbbb>{price:N0}</color></size>";

        if (description != null)
            description.text = itemDesc;
    }



}
