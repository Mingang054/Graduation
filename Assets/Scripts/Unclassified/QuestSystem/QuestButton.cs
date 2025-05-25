using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Button button;
    public Quest quest;
    public TMP_Text mytitle;
    public TMP_Text myobject;
    public TMP_Text statusText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetQuestDetail);
    }

    public void SetQuest()
    {
        mytitle.text = quest.GetTitle();
        myobject.text = quest.GetObjectiveSummary();
    }
    public void SetStatus(string status)
    {
        if (statusText != null)
            statusText.text = status;
    }
    public void SetQuestDetail()
    {
        QuestManager.instance.currentQuest = quest;
        var temp = QuestManager.instance.questDetail;
        temp.GetComponent<QuestDetailUI>().setTextOnQUI(
            quest.GetTitle(),
            quest.description,
            quest.GetObjectiveSummary());
        temp.SetActive(true);
        UIManager.Instance.current3rdUI = temp; // « ø‰«‘
    }
}
