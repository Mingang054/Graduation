using UnityEngine;

public class RadioUI : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject test;

    public void EnableShopUI()
    {
        shopUI.SetActive(true);
        UIManager.Instance.currentSecondaryUI = shopUI;

    }
}
