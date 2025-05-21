using System.Collections.Generic;
using System.Linq;

public class Quest
{
    public string questId;
    public string title;
    public string description;
    public List<QuestObjective> objectives;
    public List<ItemRewardInfo> itemRewards;
    public int moneyReward;
    public List<string> requiredCompletedQuestIds;
    public bool isRepeatable;

    public Quest(QuestDataSO data)
    {
        questId = data.questId;
        title = data.title;
        description = data.description;
        itemRewards = data.itemRewards;
        moneyReward = data.moneyReward;
        isRepeatable = data.isRepeatable;
        requiredCompletedQuestIds = data.requiredCompletedQuests != null ?
            data.requiredCompletedQuests.Select(q => q.questId).ToList() : new List<string>();

        objectives = new List<QuestObjective>();
        if (data.objectives != null)
        {
            foreach (var obj in data.objectives)
            {
                switch (obj.type)
                {
                    case ObjectiveType.KillNPC:
                        objectives.Add(new KillObjective(obj.key, obj.targetCount));
                        break;
                    case ObjectiveType.DeliverItem:
                        objectives.Add(new DeliverObjective(obj.key, obj.targetCount));
                        break;
                    case ObjectiveType.ConditionFlag:
                        objectives.Add(new ConditionObjective(obj.key));
                        break;
                    case ObjectiveType.KillFaction:
                        objectives.Add(new KillFactionObjective(obj.key, obj.targetCount));
                        break;
                }
            }
        }
    }

    public void ReportProgress(string key, int amount = 1)
    {
        foreach (var obj in objectives)
        {
            if (obj is KillObjective ko && ko.targetNpcCode == key && !ko.IsComplete)
                ko.currentCount += amount;
            else if (obj is KillFactionObjective fo && fo.targetFaction == key && !fo.IsComplete)
                fo.currentCount += amount;
            else if (obj is DeliverObjective dobj && dobj.itemCode == key && !dobj.IsComplete)
                dobj.deliveredAmount += amount;
            else if (obj is ConditionObjective co && co.conditionKey == key)
                co.isSatisfied = true;
        }
    }

    public bool IsComplete => objectives.All(o => o.IsComplete);

    public string GetTitle() => title;
    public string GetObjectiveSummary()
    {
        if (objectives == null || objectives.Count == 0) return "※ 목표 없음";
        List<string> lines = new();
        foreach (var obj in objectives)
        {
            string line = obj switch
            {
                KillObjective ko => $"• {ko.requiredCount}명 처치: {ko.targetNpcCode} ({ko.currentCount}/{ko.requiredCount})",
                KillFactionObjective fo => $"• {fo.requiredCount}명 처치: {fo.targetFaction} 팩션 ({fo.currentCount}/{fo.requiredCount})",
                DeliverObjective dobj => $"• {dobj.requiredAmount}개 납품: {dobj.itemCode} ({dobj.deliveredAmount}/{dobj.requiredAmount})",
                ConditionObjective co => $"• 조건 만족: {co.conditionKey}" + (co.isSatisfied ? " (완료)" : ""),
                _ => "• (알 수 없는 목표)"
            };
            lines.Add(line);
        }
        return string.Join("\n", lines);
    }
}

