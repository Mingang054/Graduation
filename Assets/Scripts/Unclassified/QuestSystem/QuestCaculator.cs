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
    
    public void AddKillCount(string npcCode, Faction facion)
    {
        
    }


}
