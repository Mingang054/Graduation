using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text objectiveText;

    public Button actionButton;
    public TMP_Text actionButtonText;

    private void OnEnable()
    {
        UpdateButtonAndInfo();
    }

    public void setTextOnQUI(string title, string desc, string objective)
    {
        titleText.text = title;
        descriptionText.text = desc;
        objectiveText.text = objective;
        UpdateButtonAndInfo(); // 버튼 상태 동기화
    }

    /// <summary>
    /// 버튼 텍스트와 인터랙션 로직을 퀘스트 상태에 맞게 변경
    /// </summary>
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

        if (qm.completedQuests.Contains(quest))
        {
            actionButtonText.text = "완료됨";
            actionButton.interactable = false;
        }
        else if (qm.activeQuests.Contains(quest))
        {
            if (quest.IsComplete)
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
        else if (qm.availableQuests.Contains(quest))
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

    // 버튼 클릭 이벤트 (에디터에서 연결)
    public void TryClickButton()
    {
        var qm = QuestManager.instance;
        var quest = qm.currentQuest;

        if (quest == null)
            return;

        if (qm.completedQuests.Contains(quest))
        {
            // 이미 완료된 퀘스트: 아무것도 안함
        }
        else if (qm.activeQuests.Contains(quest))
        {
            if (quest.IsComplete)
            {
                qm.CompleteQuest(quest); // 보상 지급, 완료 처리
                UpdateButtonAndInfo();
                // UI 갱신 (리스트 새로고침 등)
                qm.PopulateQuestScroll_Mixed();
            }
        }
        else if (qm.availableQuests.Contains(quest))
        {
            qm.AddQuest(quest); // 수주 처리
            UpdateButtonAndInfo();
            qm.PopulateQuestScroll_Mixed();
        }
    }

    private void OnDisable()
    {
        QuestManager.instance.currentQuest = null;
    }
}
