using UnityEngine;

public class QuestUI : MonoBehaviour
{

    public void OnQuestUI()
    {
        this.gameObject.SetActive(true);
        UIManager.Instance.currentSecondaryUI =  this.gameObject;
    }
}
