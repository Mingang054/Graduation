using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//미사용 Dummy
public class LivingEntity : MonoBehaviour, IDamageable
{


    public float startingHealthPoint = 100;
    public float healthPointMax = 100;                   //최대체력
    public float healthPoint { get; protected set; }     //체력
    public float armor = 0f;                        //방어내구도
    public float reduction = 0f;                    //피해감소

    public bool isDead = false;
    public Action onDeath;

    protected virtual void OnEnable()
    {
        isDead = false;
        healthPoint = startingHealthPoint;
    }

    public virtual void OnHitDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal, Faction projectileFaction)
    {
        float penetratedArmor= armor - penetration;
        if (penetratedArmor < 0f) { penetratedArmor = 0f; }









        healthPoint -= damage-penetratedArmor;
        if(healthPoint <= 0f && !isDead) { Die(); } 
    }

    public virtual void Die()
    {
        //이벤트 실행되게
        if (onDeath!=null)
        {
            onDeath();   
        }
        isDead = true;
    }
}
