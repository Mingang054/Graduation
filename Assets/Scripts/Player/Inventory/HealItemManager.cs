using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class HealItemManager : MonoBehaviour
{
    public static HealItemManager instance;
    public HealSlotUI[] healSlot;             // �ν����� ����
    public Consumable[] consumables;          // ���� 6
    
    public HealItemOnVest healOnVest;

    public GameObject HealBase;
    public GameObject injectorSet;
    public GameObject tourniquetSet;
    public GameObject tempSet;

    public Consumable currentConsumable;
    public ItemInstanceUI currentConsumableUI;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        // �迭�� ��� ������ ���� 6���� Ȯ��
        if (consumables == null || consumables.Length == 0)
            consumables = new Consumable[6];

        // �� HealSlotUI�� index �ڵ� �Ҵ� (0~5)
        for (int i = 0; i < healSlot.Length; i++)
            healSlot[i].index = i;
    }

    public void StartHealKit(int index)
    {
        currentConsumableUI = healSlot[index].equipedItem;
        currentConsumable = consumables[index];
        ConsumableData medData = consumables[index].data as ConsumableData;
            
        HealBase.SetActive(true);
        if (medData.isInjector)
        {
            UIManager.Instance.current3rdUI = injectorSet;
            injectorSet.SetActive(true);
        }else if (medData.isTourniquet)
        {
            UIManager.Instance.current3rdUI = tourniquetSet;
            tourniquetSet.SetActive(true);
        }
        else
        {

        }

    }

    public void UseHealKit()
    {
        currentConsumableUI.TryUseItem();
        
    }
    
    public void EndHealKit()
    {
        if (healOnVest!= null) { healOnVest.UpdateButtonUI(); }


        currentConsumableUI = null;
        currentConsumable = null;
        if (UIManager.Instance.current3rdUI != null)
        {
            UIManager.Instance.current3rdUI.SetActive(false);
            UIManager.Instance.current3rdUI = null;
        }

        HealBase.SetActive(false);

    }
    
    
}