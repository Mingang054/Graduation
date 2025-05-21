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
        UpdateButtonAndInfo(); // ��ư ���� ����ȭ
    }

    /// <summary>
    /// ��ư �ؽ�Ʈ�� ���ͷ��� ������ ����Ʈ ���¿� �°� ����
    /// </summary>
    private void UpdateButtonAndInfo()
    {
        var qm = QuestManager.instance;
        var quest = qm.currentQuest;
        if (quest == null)
        {
            actionButtonText.text = "����";
            actionButton.interactable = false;
            return;
        }

        if (qm.completedQuests.Contains(quest))
        {
            actionButtonText.text = "�Ϸ��";
            actionButton.interactable = false;
        }
        else if (qm.activeQuests.Contains(quest))
        {
            if (quest.IsComplete)
            {
                actionButtonText.text = "�Ϸ� ���� �ޱ�";
                actionButton.interactable = true;
            }
            else
            {
                actionButtonText.text = "���� ��";
                actionButton.interactable = false;
            }
        }
        else if (qm.availableQuests.Contains(quest))
        {
            actionButtonText.text = "�����ϱ�";
            actionButton.interactable = true;
        }
        else
        {
            actionButtonText.text = "����";
            actionButton.interactable = false;
        }
    }

    // ��ư Ŭ�� �̺�Ʈ (�����Ϳ��� ����)
    public void TryClickButton()
    {
        var qm = QuestManager.instance;
        var quest = qm.currentQuest;

        if (quest == null)
            return;

        if (qm.completedQuests.Contains(quest))
        {
            // �̹� �Ϸ�� ����Ʈ: �ƹ��͵� ����
        }
        else if (qm.activeQuests.Contains(quest))
        {
            if (quest.IsComplete)
            {
                qm.CompleteQuest(quest); // ���� ����, �Ϸ� ó��
                UpdateButtonAndInfo();
                // UI ���� (����Ʈ ���ΰ�ħ ��)
                qm.PopulateQuestScroll_Mixed();
            }
        }
        else if (qm.availableQuests.Contains(quest))
        {
            qm.AddQuest(quest); // ���� ó��
            UpdateButtonAndInfo();
            qm.PopulateQuestScroll_Mixed();
        }
    }

    private void OnDisable()
    {
        QuestManager.instance.currentQuest = null;
    }
}
