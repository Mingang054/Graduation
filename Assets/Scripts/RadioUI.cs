using UnityEngine;

public class RadioUI : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject ammoShopUI;

    public void EnableShopUI()
    {
        shopUI.SetActive(true);
        UIManager.Instance.currentSecondaryUI = shopUI;

    }

    public void EnableAmmoShopUI()
    {
        ammoShopUI.SetActive(true);
        UIManager.Instance.currentSecondaryUI = ammoShopUI;

    }
}
