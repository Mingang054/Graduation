using UnityEngine;
using UnityEngine.UI;

public class WeaponOnHand : MonoBehaviour
{
    //public GameObject receiverStopper;
    public Image weaponBody;
    public Image loadBar;
    public GameObject MagPlace;

    public Weapon currentWeapon;


    public void UpdateUI()
    {
        if (currentWeapon.data is WeaponData weaponData)
        {

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

