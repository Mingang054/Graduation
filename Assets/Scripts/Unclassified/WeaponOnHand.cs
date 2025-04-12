using UnityEngine;
using UnityEngine.UIElements;

public class WeaponOnHand : MonoBehaviour
{
    public Image weaponBody;
    public Image loadBar;
    //public GameObject receiverStopper;
    public GameObject MagPlace;

    public Weapon currentWeapon;


    private void UpdateUI()
    {
        if (currentWeapon.data is WeaponData weaponData)
        {
            if (weaponData.ammoType != AmmoType.Shell &&weaponData.ammoType != AmmoType.Magnum) {
                
            }

            switch (weaponData.reloadMode)
            {
                case ReloadType.Single:
                    break;
                case ReloadType.Magazine:
                    if (currentWeapon.isMag)
                    {
                        if(weaponBody.sprite != weaponData.weaponBody) 
                        { weaponBody.sprite = weaponData.weaponBody; }
                        
                    }else {
                        if(weaponBody.sprite != weaponData.weaponUnloadBody)
                        {
                            weaponBody.sprite = weaponData.weaponUnloadBody;
                        }
                    }
                    break;
                default: 
                    break;
            }
        }
    }

}

