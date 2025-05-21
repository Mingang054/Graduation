using Mono.Cecil.Cil;
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

    public string GetTitle() {
        string titleText = questId + " " + title ;
        return titleText;
    }
    public string GetObjectiveSummary()
    {
        if (objectives == null || objectives.Count == 0)
            return "※ 목표 없음";

        List<string> lines = new();

        foreach (var obj in objectives)
        {
            string line = obj switch
            {
                KillObjective kill => $"• {kill.requiredCount}명 처치: {kill.targetNpcCode}",
                KillFactionObjective killFac => $"• {killFac.requiredCount}명 처치: {killFac.targetFaction} 팩션",
                DeliverObjective deliver => $"• {deliver.requiredAmount}개 납품: {deliver.itemCode}",
                ConditionObjective cond => $"• 조건 만족: {cond.conditionKey}",
                _ => "• (알 수 없는 목표)"
            };

            lines.Add(line);
        }

        return string.Join("\n", lines);
    }

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

        // 목표 복제 및 다형성 인스턴스화
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
                }
            }
        }
    }

    public void ReportProgress(string key, int amount = 1)
    {
        foreach (var obj in objectives)
            obj.Report(key, amount);
    }

    public bool IsComplete => objectives.All(o => o.IsComplete);
}
