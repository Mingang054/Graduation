using System.Collections.Generic;
using UnityEngine;

public class Magazine
{
    private AmmoType ammoType;              // 탄약 종류 (구경)
    private int ammoCapacity;               // 최대 탄약 수
    private Stack<bool> ammoStack;          // 탄약을 AP 여부로 관리 (true: AP 탄, false: 일반 탄)

    // 생성자: 탄약 종류와 최대 용량을 초기화
    public Magazine(AmmoType ammoType, int ammoCapacity)
    {
        this.ammoType = ammoType;
        this.ammoCapacity = ammoCapacity;
        ammoStack = new Stack<bool>(ammoCapacity);
    }

    // 탄약 추가
    public bool AddAmmo(bool isAP)
    {
        if (ammoStack.Count >= ammoCapacity)
        {
            Debug.Log("탄창이 가득 찼습니다.");
            return false;
        }

        ammoStack.Push(isAP);
        Debug.Log($"{(isAP ? "AP 탄" : "일반 탄")} 추가됨. 현재 탄 수: {ammoStack.Count}");
        return true;
    }

    // 탄약 제거 (발사 시)
    public bool RemoveAmmo()
    {
        if (ammoStack.Count == 0)
        {
            Debug.Log("탄약이 없습니다.");
            return false;  // 기본 값: 일반 탄으로 처리 (예외 상황)
        }

        return ammoStack.Pop();
    }

    // 현재 탄 수 반환
    public int GetAmmoCount()
    {
        return ammoStack.Count;
    }

    // 최대 용량 반환
    public int GetCapacity()
    {
        return ammoCapacity;
    }

    // 탄약 종류 반환
    public AmmoType GetAmmoType()
    {
        return ammoType;
    }
}
