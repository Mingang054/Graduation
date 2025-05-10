using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioTalk : MonoBehaviour
{
    private int indexOfTalk = 0;
    public List<string> freeTalkList = new List<string>();
    public List<string> shopTalkList = new List<string>();
    public TMP_Text myText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void FreeTalk()
    {
        if (indexOfTalk > freeTalkList.Count)
        {
            
        }
    }

    public void ShopTalk()
    {
        
    }

}
