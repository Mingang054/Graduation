using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionPoint : Interactable
{
    
    public float timeToExtract =15f;
    public float extractDistance = 4f;
    
    public bool constraint = false;
    public List<bool> constraintList = new List<bool>();

    public float cost = 0;
    public bool isPaid = false;
    //public interactionName ; 탈출구 이름 "RailRoad", Driver...
    
    private void Awake()
    {

        interactType = InteractType.Extraction;
    }

    public bool getAvailable()  //탈출 조건 유무, 가능 여부 확인
    {
        if (constraint == true)
        {
            if (constraintList.Contains(false))
            {
                return false;
            }
        }

        if (cost > 0)
        {
            if (isPaid == false) {
                return false;
            }
        }
        return true;
    }
    
}
