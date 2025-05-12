using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestCaculator : MonoBehaviour
{
    //�� ���� ������ ����� ������ ����ϱ� ���� ����ϴ� ��ũ��Ʈ
    /// <summary>
    /// Dictionary ����
    /// 
    /// </summary>
    public static QuestCaculator instance;

    public Dictionary<string, int> npcKillCount;
    public Dictionary<string, int> factionKillCount;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        factionKillCount = new Dictionary<string, int>();
        npcKillCount = new Dictionary<string, int>();
    
    }

    public void AddKillCount(string npcCode, Faction faction)
    {
        // 1. NPC �ڵ庰 ų �� �߰�
        if (npcKillCount.ContainsKey(npcCode))
        {
            npcKillCount[npcCode]++;
        }
        else
        {
            npcKillCount[npcCode] = 1;
        }

        // 2. Faction �� ų �� �߰� (Key�� faction.ToString())
        string factionKey = faction.ToString();

        if (factionKillCount.ContainsKey(factionKey))
        {
            factionKillCount[factionKey]++;
        }
        else
        {
            factionKillCount[factionKey] = 1;
        }
    }



}
