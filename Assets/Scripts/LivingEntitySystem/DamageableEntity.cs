using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity  : MonoBehaviour, IDamageable
{
    public string npcCode;
    public float healthPointMax= 100;                   //최대체력
    public float healthPoint ;//{ get; protected set; }     //체력
    public float armorPoint = 0f;                        //방어내구도
    public float reduction = 0f;                    //피해감소

    public bool isEssential = false;
    public bool isDead = false;
    
    public Action onDeath;

    public Faction faction;


    public Collider2D entityCollider;


    protected virtual void OnEnable()
    {
        isDead = false;
    }


    public virtual void Awake()
    {
         entityCollider = GetComponent<Collider2D>();     //사망 시 콜라이더 비활성화
    }



    public virtual void OnHitDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal, Faction projectileFaction)
    {
        float penetratedArmor = armorPoint - penetration;
        float penetratedDamage;
        if (penetratedArmor > 0f)           //잔존 방어력*2 만큼의 데미지 감소
        {
            penetratedDamage = damage - (penetratedArmor * 2);
        }else if (damage >= 1f)
        {
            penetratedDamage = 1f;          //최소 데미지 보장
        }
        else
        {
            penetratedDamage = damage;
        }


        healthPoint -= penetratedDamage; 
        if (healthPoint <= 0f && !isDead) { 
            Die();
            if(projectileFaction == Faction.Friendly)
            {

                //퀘스트 반영용 코드 작성
            }
        }
        
        
    }

    public virtual void Die()
    {
        if(isEssential)
        {
            //사망없음
            return;
        }
        //onDeath이벤트 실행되게
        if (onDeath != null)
        {
            onDeath();
        }
        isDead = true;
    }
}
