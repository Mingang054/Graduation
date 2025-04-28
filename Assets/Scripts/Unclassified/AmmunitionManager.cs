using JetBrains.Annotations;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class AmmunitionManager : MonoBehaviour
{
    //subUI
    public int lightAmmo;   //pistol, smg(light mag)
    public int mediumAmmo;
    public int heavyAmmo;
    public int antiAmmo;
    public int shellAmmo;
    public int magnumAmmo;
    public int explosiveAmmo;

    public static AmmunitionManager instance;


    public TMP_Text mainTxt;
    public TMP_Text lightTxt;
    public TMP_Text mediumTxt;
    public TMP_Text heavyTxt;
    public TMP_Text antiTxt;
    public TMP_Text shellTxt;
    public TMP_Text magnumTxt;
    public TMP_Text explosiveTxt;


    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        this.gameObject.SetActive(false);
        
    }
    public void UpdateAmmo()
    {
        mainTxt.text =
            $"Light Ammo: {lightAmmo}\n" +
            $"Medium Ammo: {mediumAmmo}\n" +
            $"Heavy Ammo: {heavyAmmo}\n" +
            $"Anti Ammo: {antiAmmo}\n" +
            $"Shell Ammo: {shellAmmo}\n" +
            $"Magnum Ammo: {magnumAmmo}\n" +
            $"Explosive Ammo: {explosiveAmmo}";
    }

    public void OnAmmoUI()
    {
        if (!this.gameObject.active)
        {
            this.gameObject.SetActive(true);
            UIManager.Instance.currentSecondaryUI = this.gameObject;


        }
    }

    public void UpdateUI()
    {

    }

    public void OnEnable()
    {
        if(VestInventory.Instance != null)
        VestInventory.Instance.isAmmoFill = true;
        UpdateAmmo();
    }

    public void OnDisable()
    {
        VestInventory.Instance.isAmmoFill = false;
    }
}
