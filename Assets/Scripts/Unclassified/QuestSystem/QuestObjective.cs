using System;

public abstract class QuestObjective
{
    public ObjectiveType type;
    public abstract int GetProgress();
    public abstract void SetProgress(int value);
    public abstract bool IsComplete { get; }
}

public class KillObjective : QuestObjective
{
    public string targetNpcCode;
    public int requiredCount;
    public int currentCount;
    public KillObjective(string npcCode, int count)
    {
        type = ObjectiveType.KillNPC;
        targetNpcCode = npcCode;
        requiredCount = count;
        currentCount = 0;
    }
    public override int GetProgress() => currentCount;
    public override void SetProgress(int value) => currentCount = value;
    public override bool IsComplete => currentCount >= requiredCount;
}

[Serializable]
public class KillFactionObjective : QuestObjective
{
    public string targetFaction;
    public int requiredCount;
    public int currentCount;

    public KillFactionObjective(string faction, int count)
    {
        type = ObjectiveType.KillFaction;
        targetFaction = faction;
        requiredCount = count;
        currentCount = 0;
    }

    public override int GetProgress() => currentCount;
    public override void SetProgress(int value) => currentCount = value;
    public override bool IsComplete => currentCount >= requiredCount;
}

[Serializable]
public class DeliverObjective : QuestObjective
{
    public string itemCode;
    public int requiredAmount;
    public int deliveredAmount;

    public DeliverObjective(string code, int amount)
    {
        type = ObjectiveType.DeliverItem;
        itemCode = code;
        requiredAmount = amount;
        deliveredAmount = 0;
    }

    public override int GetProgress() => deliveredAmount;
    public override void SetProgress(int value) => deliveredAmount = value;
    public override bool IsComplete => deliveredAmount >= requiredAmount;
}

[Serializable]
public class ConditionObjective : QuestObjective
{
    public string conditionKey;
    public bool isSatisfied;

    public ConditionObjective(string key)
    {
        type = ObjectiveType.ConditionFlag;
        conditionKey = key;
        isSatisfied = false;
    }

    public override int GetProgress() => isSatisfied ? 1 : 0;
    public override void SetProgress(int value) => isSatisfied = value != 0;
    public override bool IsComplete => isSatisfied;
}
