using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quests/QuestDataSO")]
public class QuestDataSO : ScriptableObject
{
    public string questId;
    public string title;
    [TextArea(2, 5)]
    public string description;

    public List<QuestObjectiveInfo> objectives;      // 목표 리스트
    public List<ItemRewardInfo> itemRewards;         // 아이템 보상 리스트
    public int moneyReward;

    public List<QuestDataSO> requiredCompletedQuests; // 선행퀘스트들
    public bool isRepeatable;
}

[System.Serializable]
public class QuestObjectiveInfo
{
    public ObjectiveType type;
    public string key;      // npcCode, itemCode, conditionKey, faction명 등
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
    KillFaction // 추가!
}