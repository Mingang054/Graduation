using UnityEngine;

public class RadioUI : MonoBehaviour
{
    public GameObject QuestUI;

    public GameObject shopUI;
    public GameObject ammoShopUI;


    public void EnableQuestUI()
    {
        QuestUI.SetActive(true);
        UIManager.Instance.currentSecondaryUI = QuestUI;

    }

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
    public void OnEnable()
    {
        PlayerMovement.Instance.enabled = false;
    }
    public void OnDisable()
    {
        PlayerMovement.Instance.enabled = true;
    }
}
