using System;


[Serializable]
public abstract class QuestObjective
{
    public ObjectiveType type;
    public bool IsComplete => CheckComplete();

    public abstract void Report(string key, int amount = 1);
    protected abstract bool CheckComplete();
}

[Serializable]
public class KillObjective : QuestObjective
{
    public string targetNpcCode;
    public int requiredCount;
    private int currentCount = 0;

    public KillObjective(string npcCode, int count)
    {
        type = ObjectiveType.KillNPC;
        targetNpcCode = npcCode;
        requiredCount = count;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == targetNpcCode && !IsComplete)
            currentCount += amount;
    }

    protected override bool CheckComplete() => currentCount >= requiredCount;
}

[Serializable]
public class DeliverObjective : QuestObjective
{
    public string itemCode;
    public int requiredAmount;
    private int deliveredAmount = 0;

    public DeliverObjective(string code, int count)
    {
        type = ObjectiveType.DeliverItem;
        itemCode = code;
        requiredAmount = count;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == itemCode && !IsComplete)
            deliveredAmount += amount;
    }

    protected override bool CheckComplete() => deliveredAmount >= requiredAmount;
}

[Serializable]
public class ConditionObjective : QuestObjective
{
    public string conditionKey;
    private bool isSatisfied = false;

    public ConditionObjective(string key)
    {
        type = ObjectiveType.ConditionFlag;
        conditionKey = key;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == conditionKey)
            isSatisfied = true;
    }

    protected override bool CheckComplete() => isSatisfied;
}

[Serializable]
public class KillFactionObjective : QuestObjective
{
    public string targetFaction;
    public int requiredCount;
    private int currentCount = 0;

    public KillFactionObjective(string faction, int count)
    {
        type = ObjectiveType.KillFaction;
        targetFaction = faction;
        requiredCount = count;
    }

    public override void Report(string key, int amount = 1)
    {
        if (key == targetFaction && !IsComplete)
            currentCount += amount;
    }

    protected override bool CheckComplete() => currentCount >= requiredCount;
}
