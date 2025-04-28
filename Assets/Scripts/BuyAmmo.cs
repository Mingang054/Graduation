using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyAmmo : MonoBehaviour
{
    public WeaponAType type;
    public int count;   // 구매할 탄약 수량
    public int price;   // 가격
    private Button button;

    public TMP_Text text;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnBuyButtonClicked);
        }
    }

    private void OnBuyButtonClicked()
    {
        if (PlayerStatus.instance.money < price)
        {
            Debug.Log("❗ 돈이 부족합니다.");
            return;
        }

        switch (type)
        {
            case WeaponAType.Light:
                AmmunitionManager.instance.lightAmmo += count;
                break;
            case WeaponAType.Medium:
                AmmunitionManager.instance.mediumAmmo += count;
                break;
            case WeaponAType.Heavy:
                AmmunitionManager.instance.heavyAmmo += count;
                break;
            case WeaponAType.Anti:
                AmmunitionManager.instance.antiAmmo += count;
                break;
            case WeaponAType.Shell:
                AmmunitionManager.instance.shellAmmo += count;
                break;
            case WeaponAType.Magnum:
                AmmunitionManager.instance.magnumAmmo += count;
                break;
            case WeaponAType.Explosive:
                AmmunitionManager.instance.explosiveAmmo += count;
                break;
            default:
                Debug.LogWarning("⚠️ 알 수 없는 탄약 타입입니다.");
                return;
        }

        PlayerStatus.instance.money -= price;

        // 탄약 수치 업데이트
        AudioManager.Instance.PlayCoin();
        AmmunitionManager.instance.UpdateUI();
        UpdateUI();
        Debug.Log($"✅ {type} 탄약 {count}개 구매 완료! 남은 돈: {PlayerStatus.instance.money}");
    }

    public void UpdateUI()
    {
        int currentAmmo = 0;

        switch (type)
        {
            case WeaponAType.Light:
                currentAmmo = AmmunitionManager.instance.lightAmmo;
                break;
            case WeaponAType.Medium:
                currentAmmo = AmmunitionManager.instance.mediumAmmo;
                break;
            case WeaponAType.Heavy:
                currentAmmo = AmmunitionManager.instance.heavyAmmo;
                break;
            case WeaponAType.Anti:
                currentAmmo = AmmunitionManager.instance.antiAmmo;
                break;
            case WeaponAType.Shell:
                currentAmmo = AmmunitionManager.instance.shellAmmo;
                break;
            case WeaponAType.Magnum:
                currentAmmo = AmmunitionManager.instance.magnumAmmo;
                break;
            case WeaponAType.Explosive:
                currentAmmo = AmmunitionManager.instance.explosiveAmmo;
                break;
            default:
                Debug.LogWarning("⚠️ 알 수 없는 탄약 타입입니다.");
                break;
        }

        // 🔥 현재 탄약 수량을 표시
        text.text = $"({currentAmmo})";
    }
    private void OnEnable()
    {
        UpdateUI();
    }
}
