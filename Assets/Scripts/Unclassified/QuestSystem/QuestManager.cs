using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public GameObject QuestPrefab;

    public GameObject questDetail;
    public RectTransform questContent;

    
    public Quest currentQuest;//UI상으로 실행된 Quest

    [Header("📜 모든 퀘스트 데이터")]
    public List<QuestDataSO> allQuestSO = new();

    [Header("🟢 수주 가능 퀘스트 (UI 바인딩)")]
    public List<Quest> availableQuests = new();

    [Header("📝 진행 중 퀘스트 (UI 바인딩)")]
    public List<Quest> activeQuests = new();

    [Header("✅ 완료된 퀘스트 (UI 바인딩)")]
    public List<Quest> completedQuests = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // 자동 SO 로드 (최초 한 번만)
        if (allQuestSO == null || allQuestSO.Count == 0)
            allQuestSO = Resources.LoadAll<QuestDataSO>("QuestData").ToList();

        UpdateAvailableQuest();
    }

    public void PopulateQuestScroll_Mixed()
    {
        UpdateAvailableQuest();
        // 기존 리스트 삭제
        foreach (Transform child in questContent)
            Destroy(child.gameObject);

        // 수주 가능 먼저 출력 (예: 구분선 추가 가능)
        foreach (var quest in availableQuests)
        {
            var go = Instantiate(QuestPrefab, questContent);
            var qb = go.GetComponent<QuestButton>();
            qb.quest = quest;
            qb.SetQuest();

            // 상태 표시 (수주 가능)
            //qb.SetStatus("수주 가능");
            // 또는 qb.myStatusText.text = "수주 가능";
        }

        // 진행 중 퀘스트도 출력
        foreach (var quest in activeQuests)
        {
            var go = Instantiate(QuestPrefab, questContent);
            var qb = go.GetComponent<QuestButton>();
            qb.quest = quest;
            qb.SetQuest();

            // 상태 표시 (진행 중)
            //qb.SetStatus("진행 중");
        }
    }


    // 다형성 QuestObjective 인스턴스 생성
    private Quest InstantiateQuestFromSO(QuestDataSO so)
    {
        var quest = new Quest(so)
        {
            questId = so.questId,
            title = so.title,
            description = so.description,
            itemRewards = so.itemRewards,
            moneyReward = so.moneyReward,
            isRepeatable = so.isRepeatable,
            requiredCompletedQuestIds = so.requiredCompletedQuests != null ?
                so.requiredCompletedQuests.Select(q => q.questId).ToList() : new List<string>(),
            objectives = new List<QuestObjective>()
        };

        // 목표별 다형성 객체 생성
        if (so.objectives != null)
        {
            foreach (var obj in so.objectives)
            {
                switch (obj.type)
                {
                    case ObjectiveType.KillNPC:
                        quest.objectives.Add(new KillObjective(obj.key, obj.targetCount));
                        break;
                    case ObjectiveType.DeliverItem:
                        quest.objectives.Add(new DeliverObjective(obj.key, obj.targetCount));
                        break;
                    case ObjectiveType.ConditionFlag:
                        quest.objectives.Add(new ConditionObjective(obj.key));
                        break;
                    case ObjectiveType.KillFaction:
                        quest.objectives.Add(new KillFactionObjective(obj.key, obj.targetCount));
                        break;
                }
            }
        }
        return quest;
    }

    // 수주 가능 퀘스트 평가
    public void UpdateAvailableQuest()
    {
        foreach (var so in allQuestSO)
        {
            if (availableQuests.Any(q => q.questId == so.questId)) continue;
            if (activeQuests.Any(q => q.questId == so.questId)) continue;
            if (completedQuests.Any(q => q.questId == so.questId) && !so.isRepeatable) continue;

            bool preconditionsMet = so.requiredCompletedQuests == null ||
                so.requiredCompletedQuests.All(req => completedQuests.Any(cq => cq.questId == req.questId));

            if (preconditionsMet)
                availableQuests.Add(InstantiateQuestFromSO(so));
        }
    }

    // 퀘스트 수주 (UI 버튼 등에서 호출)
    public void AddQuest(Quest quest)
    {
        if (!availableQuests.Contains(quest)) return;
        activeQuests.Add(quest);
        availableQuests.Remove(quest);
    }

    // 퀘스트 목표 진행 처리 (팩션/개별 모두 지원)
    public void ReportEvent(string npcKey, int amount = 1, string factionKey = null)
    {
        foreach (var quest in activeQuests.ToList())
        {
            quest.ReportProgress(npcKey, amount); // KillNPC, Deliver, Condition

            if (!string.IsNullOrEmpty(factionKey))
                quest.ReportProgress(factionKey, amount); // KillFaction

            if (quest.IsComplete)
                CompleteQuest(quest);
        }
    }

    // 퀘스트 완료 및 보상 처리
    public void CompleteQuest(Quest quest)
    {
        if (!quest.IsComplete) return;

        var BIM = BagInventoryManager.Instance;
        foreach (var reward in quest.itemRewards)
        {
            var item = ItemFactory.CreateItem(new ItemInitData
            {
                itemCode = reward.itemId,
                count = reward.quantity
            });
            BIM.PlaceFirstAvailableSlot(item, BIM.myItems);
        }

        PlayerStatus.instance.money += quest.moneyReward;

        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        UpdateAvailableQuest();
    }
}