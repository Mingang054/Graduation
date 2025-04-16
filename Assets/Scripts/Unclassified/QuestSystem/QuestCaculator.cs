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
    
    public void AddKillCount(string npcCode, Faction facion)
    {
        
    }


}
