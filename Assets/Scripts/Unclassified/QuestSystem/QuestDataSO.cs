using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quests/QuestDataSO")]
public class QuestDataSO : ScriptableObject
{
    public string questId;
    public string title;
    [TextArea(2, 5)]
    public string description;

    public List<QuestObjectiveInfo> objectives;      // ��ǥ ����Ʈ
    public List<ItemRewardInfo> itemRewards;         // ������ ���� ����Ʈ
    public int moneyReward;

    public List<QuestDataSO> requiredCompletedQuests; // ��������Ʈ��
    public bool isRepeatable;
}

[System.Serializable]
public class QuestObjectiveInfo
{
    public ObjectiveType type;
    public string key;      // npcCode, itemCode, conditionKey, faction�� ��
    public int targetCount;
}

[System.Serializable]
public class ItemRewardInfo
{
    public string itemId;
    public int quantity;
}

public enum ObjectiveType
{
    KillNPC,
    DeliverItem,
    ConditionFlag,
    KillFaction // �߰�!
}