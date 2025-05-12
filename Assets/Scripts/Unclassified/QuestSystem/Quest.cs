using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class QuestObjective
{
    public ObjectiveType type;
    public bool IsComplete => CheckComplete();

    public abstract void Report(string key, int amount = 1);
    protected abstract bool CheckComplete();
}

[System.Serializable]
public class KillObjective : QuestObjective
{
    public string targetNpcCode;
    public int requiredCount;
    private int currentCount = 0;

    public KillObjective()
    {
        type = ObjectiveType.KillNPC;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == targetNpcCode)
            currentCount += amount;
    }

    protected override bool CheckComplete()
    {
        return currentCount >= requiredCount;
    }
}

[System.Serializable]
public class DeliverObjective : QuestObjective
{
    public string itemCode;
    public int requiredAmount;
    private int deliveredAmount = 0;

    public DeliverObjective()
    {
        type = ObjectiveType.DeliverItem;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == itemCode)
            deliveredAmount += amount;
    }

    protected override bool CheckComplete()
    {
        return deliveredAmount >= requiredAmount;
    }
}

[System.Serializable]
public class ConditionObjective : QuestObjective
{
    public string conditionKey;
    private bool isSatisfied = false;

    public ConditionObjective()
    {
        type = ObjectiveType.ConditionFlag;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == conditionKey)
            isSatisfied = true;
    }

    protected override bool CheckComplete()
    {
        return isSatisfied;
    }
}


[System.Serializable]
public class QuestData
{
    public string questID;
    public string title;
    public List<QuestObjective> objectives;
}


public class Quest
{
    public QuestData data;

    public bool IsComplete => data.objectives.All(o => o.IsComplete);

    public void ReportProgress(string key, int amount = 1)
    {
        foreach (var obj in data.objectives)
        {
            obj.Report(key, amount);
        }
    }
}


public enum ObjectiveType
{
    KillNPC,
    DeliverItem,
    ConditionFlag
}
