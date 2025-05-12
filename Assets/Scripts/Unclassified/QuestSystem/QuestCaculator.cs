using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestCaculator : MonoBehaviour
{
    //한 라운드 내에서 수행된 행위를 기록하기 위해 사용하는 스크립트
    /// <summary>
    /// Dictionary 구조
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
        // 1. NPC 코드별 킬 수 추가
        if (npcKillCount.ContainsKey(npcCode))
        {
            npcKillCount[npcCode]++;
        }
        else
        {
            npcKillCount[npcCode] = 1;
        }

        // 2. Faction 별 킬 수 추가 (Key는 faction.ToString())
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
