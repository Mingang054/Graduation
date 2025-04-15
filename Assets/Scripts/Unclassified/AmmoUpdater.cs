using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AmmoUpdater : MonoBehaviour
{
    public TMP_Text tMP_Text;

    private void Awake()
    {
       tMP_Text = GetComponent<TMP_Text>();
    }
    public void UpdateAmmoUI(int? ammoCount)
    {
        if (ammoCount == null)
        {
            tMP_Text.text = "N/A"; // �Ǵ� "N/A", "����" �� ǥ��
        }
        else
        {
            Debug.Log(ammoCount);
            tMP_Text.text = ammoCount.ToString();   
        }
    }
}
