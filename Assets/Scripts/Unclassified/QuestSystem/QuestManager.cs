using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    int questIndex = 0;

    public static QuestManager instance;
    private List<Quest> activeQuests = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddQuest(QuestData data)
    {
        activeQuests.Add(new Quest { data = data });
    }

    public void ReportEvent(string key, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            quest.ReportProgress(key, amount);
        }
    }
}
