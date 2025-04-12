using UnityEngine;

public class WeaponOnHand : MonoBehaviour
{
    public GameObject loadBar;
    //public GameObject receiverStopper;
    public GameObject MagPlace;

    public Weapon currentWeapon;


    private void UpdateUI()
    {
        if (currentWeapon.data is WeaponData weaponData)
        {
            if (weaponData.ammoType != AmmoType.Shell &&weaponData.ammoType != AmmoType.Magnum) {
                
            }
        }
    }

}

