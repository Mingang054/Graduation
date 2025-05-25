using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class QuestDetailUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text objectiveText;
    public Button actionButton;
    public TMP_Text actionButtonText;

    private void Awake()
    {
        // 중복 연결 방지용 리스너 초기화 후 직접 연결
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(TryClickButton);
    }

    private void OnEnable()
    {
        UpdateButtonAndInfo();
    }

    public void setTextOnQUI(string title, string desc, string objective)
    {
        titleText.text = title;
        descriptionText.text = desc;
        objectiveText.text = objective;
        UpdateButtonAndInfo();
    }

    private void UpdateButtonAndInfo()
    {
        var qm = QuestManager.instance;
        var quest = qm.currentQuest;
        if (quest == null)
        {
            actionButtonText.text = "에러";
            actionButton.interactable = false;
            return;
        }

        if (qm.completedQuests.Contains(quest) && quest.isRewarded)
        {
            actionButtonText.text = "완료됨";
            actionButton.interactable = false;
        }
        else if (qm.activeQuests.Contains(quest))
        {
            if (quest.IsGoalComplete && !quest.isRewarded)
            {
                actionButtonText.text = "완료 보상 받기";
                actionButton.interactable = true;
            }
            else
            {
                actionButtonText.text = "진행 중";
                actionButton.interactable = false;
            }
        }
        else if (qm.availableQuests.Any(q => q.questId == quest.questId))
        {
            actionButtonText.text = "수주하기";
            actionButton.interactable = true;
        }
        else
        {
            actionButtonText.text = "에러";
            actionButton.interactable = false;
        }
    }

    private void TryClickButton()
    {
        var qm = QuestManager.instance;
        var quest = qm.currentQuest;
        if (quest == null) return;

        if (qm.completedQuests.Contains(quest) && quest.isRewarded)
        {
            // 아무 것도 안 함
        }
        else if (qm.activeQuests.Contains(quest))
        {
            if (quest.IsGoalComplete && !quest.isRewarded)
            {
                qm.CompleteQuest(quest);
                UpdateButtonAndInfo();
            }
        }
        else if (qm.availableQuests.Any(q => q.questId == quest.questId))
        {
            qm.AddQuest(quest);
            UpdateButtonAndInfo();
        }
    }
}
