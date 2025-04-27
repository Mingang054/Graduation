using UnityEngine;

public class AmmunitionManager : MonoBehaviour
{
    //subUI
    public int lightAmmo;
    public int mediumAmmo;
    public int heavyAmmo;
    public int antiAmmo;
    public int shellAmmo;
    public int magnumAmmo;
    public int explosiveAmmo;

    public static AmmunitionManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void UpdateAmmo()
    {
       
    }

    public void OnAmmoUI()
    {
        if (!this.gameObject.active)
        {
            this.gameObject.SetActive(true);
            UIManager.Instance.currentSecondaryUI = this.gameObject;


        }
    }

    public void OnEnable()
    {
        VestInventory.Instance.isAmmoFill = true;
    }

    public void OnDisable()
    {
        VestInventory.Instance.isAmmoFill = false;
    }
}
