using TMPro;
using UnityEngine;

public class MoneyUpdater : MonoBehaviour
{
    public static MoneyUpdater instance;

    private void Awake()
    {
       MoneyUpdater.instance = this;
    }

    public TMP_Text moneyText;
    public void OnEnable()
    {
        UpdateMoney();
    }
    public void UpdateMoney()
    {
        moneyText.text = PlayerStatus.instance.money +" C";
    }
}
