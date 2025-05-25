using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("퀘스트 프리팹/스크롤뷰/상세 UI")]
    public GameObject QuestPrefab;     // 프리팹(에디터 할당)
    public RectTransform questContent; // 스크롤뷰 Content
    public GameObject questDetail;     // 상세 패널

    public Quest currentQuest;

    [Header("SO 데이터")]
    public List<QuestDataSO> allQuestSO = new();

    [Header("수주가능/진행중/완료")]
    public List<Quest> availableQuests = new();
    public List<Quest> activeQuests = new();
    public List<Quest> completedQuests = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (allQuestSO == null || allQuestSO.Count == 0)
            allQuestSO = Resources.LoadAll<QuestDataSO>("QuestData").ToList();
        UpdateAvailableQuest();
    }

    // (1) 퀘스트 버튼 리스트 생성(중복 방지, 상태 구분)
    public void PopulateQuestScroll_Mixed()
    {
        foreach (Transform child in questContent)
            Destroy(child.gameObject);

        // 진행중(보상대기 포함)
        foreach (var quest in activeQuests)
        {
            var go = Instantiate(QuestPrefab, questContent);
            var btn = go.GetComponent<QuestButton>();
            btn.quest = quest;
            btn.SetQuest();
            btn.SetStatus(quest.IsGoalComplete && !quest.isRewarded ? "완료 보상 받기" : "진행 중");
        }

        // 수주가능(중복제거)
        foreach (var quest in availableQuests)
        {
            if (activeQuests.Any(q => q.questId == quest.questId) ||
                completedQuests.Any(q => q.questId == quest.questId)) continue;
            var go = Instantiate(QuestPrefab, questContent);
            var btn = go.GetComponent<QuestButton>();
            btn.quest = quest;
            btn.SetQuest();
            btn.SetStatus("수주 가능");
        }

        // 완료퀘(옵션)
        foreach (var quest in completedQuests)
        {
            var go = Instantiate(QuestPrefab, questContent);
            var btn = go.GetComponent<QuestButton>();
            btn.quest = quest;
            btn.SetQuest();
            btn.SetStatus("완료됨");
            btn.GetComponent<Button>().interactable = false;
        }
    }

    // (2) 중복 방지 및 선행퀘스트 조건에 따라 available 갱신
    public void UpdateAvailableQuest()
    {
        availableQuests = availableQuests.GroupBy(q => q.questId).Select(g => g.First()).ToList();
        activeQuests = activeQuests.GroupBy(q => q.questId).Select(g => g.First()).ToList();
        completedQuests = completedQuests.GroupBy(q => q.questId).Select(g => g.First()).ToList();

        foreach (var so in allQuestSO)
        {
            if (availableQuests.Any(q => q.questId == so.questId)) continue;
            if (activeQuests.Any(q => q.questId == so.questId)) continue;
            if (completedQuests.Any(q => q.questId == so.questId) && !so.isRepeatable) continue;

            bool preconditionsMet = so.requiredCompletedQuests == null ||
                so.requiredCompletedQuests.All(req => completedQuests.Any(cq => cq.questId == req.questId));

            if (preconditionsMet)
                availableQuests.Add(new Quest(so));
        }
    }

    // (3) 퀘스트 수주
    public void AddQuest(Quest quest)
    {
        if (!availableQuests.Contains(quest)) return;
        activeQuests.Add(quest);
        availableQuests.Remove(quest);
        PopulateQuestScroll_Mixed();
    }

    // (4) 퀘스트 목표 진척 보고
    public void ReportEvent(string npcKey, int amount = 1, string factionKey = null)
    {
        foreach (var quest in activeQuests.ToList())
        {
            quest.ReportProgress(npcKey, amount);
            if (!string.IsNullOrEmpty(factionKey))
                quest.ReportProgress(factionKey, amount);
        }
        PopulateQuestScroll_Mixed();
    }

    // (5) 완료버튼 눌러서만 보상(완료처리)
    public void CompleteQuest(Quest quest)
    {
        if (!quest.IsGoalComplete || quest.isRewarded) return;

        // 보상 지급
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
        quest.isRewarded = true;

        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        UpdateAvailableQuest();
        PopulateQuestScroll_Mixed();
    }
}
