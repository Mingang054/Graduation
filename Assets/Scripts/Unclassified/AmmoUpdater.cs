using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AmmoUpdater : MonoBehaviour
{
    public RectTransform ammoUIRect;
    private Vector2 size = new Vector2(64, 14);
    private int x = 64;
    private int y = 14;
    private void Awake()
    { 
        ammoUIRect = GetComponent<RectTransform>();
    }
    public void UpdateAmmoUI(int? ammoCount)
    {

        
        if (ammoCount == null) 
        {
            Vector2 size = ammoUIRect.sizeDelta;
            size.y = 0;
            ammoUIRect.sizeDelta = size;            
        }
        else
        {
            Vector2 size = ammoUIRect.sizeDelta;
            size.y = y * ammoCount.Value;
            ammoUIRect.sizeDelta = size;

        }


    }
}
