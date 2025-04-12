using System.Collections.Generic;
using UnityEngine;

public class Weapon : ItemInstance
{
    public float durability { get; private set; }  // 내구도
    public bool isChamber;   //약실에 탄 유무
    //public bool loadedIsAP;
    public bool isJam = false;

    public bool isMag;  //탄창 유무
    public int magCount;    //탄수
    //public Stack<bool> magStack;                    //isAP

    public Weapon(WeaponData data, int initialCount = 1, float initialDurability = 100)
        : base(data, initialCount)
    {
        this.durability = Mathf.Clamp(initialDurability, 0, 100);
        //magStack = null;
        magCount = 0;
    }

    // 내구도 감소
    public void DecreaseDurability(float amount)
    {
        durability = Mathf.Max(durability - amount, 0);
        if (durability == 0)
        {
            Debug.Log($"{data.itemName}이(가) 파손되었습니다.");
        }
    }

    // 내구도 설정
    public void SetDurability(float newDurability)
    {
        durability = Mathf.Clamp(newDurability, 0, 100);
        Debug.Log($"{data.itemName} 내구도 설정: {durability}");
    }


    // LoadInChamber 사용시
    /*
        if(isLoaded){
            if(magCount>0){LoadInChamber();}
            else{isLoaded = false;}
        
        }else {Debug.Log("약실에 탄이 없습니다.");}
     */
    public void PullReceiver()
    {
        if (isChamber)
        {
            if (magCount > 0) 
            { 
                isChamber = true;
                magCount--; 
            }else 
            {
                isChamber = false; 
            }

        }
        else { Debug.Log("약실에 탄이 없습니다."); }
        if (isJam) { isJam = false; }
        
        
    }

    /*
    public void InsertMag(bool isAP)
    {
        magStack.Push(isAP);

    }
    public bool? PullMag()
    {
        return null;
    }

    //디버그) 탄수량과 스택 맞추기
    public void SyncMagAndCount()
    {
        magCount = magStack.Count;
    }
    */
}
