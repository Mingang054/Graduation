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
    //public interactionName ; Ż�ⱸ �̸� "RailRoad", Driver...
    
    private void Awake()
    {

        interactType = InteractType.Extraction;
    }

    public bool getAvailable()  //Ż�� ���� ����, ���� ���� Ȯ��
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
