using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity  : MonoBehaviour, IDamageable
{
    public string npcCode;
    public float healthPointMax= 100;                   //�ִ�ü��
    public float healthPoint ;//{ get; protected set; }     //ü��
    public float armorPoint = 0f;                        //������
    public float reduction = 0f;                    //���ذ���

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
         entityCollider = GetComponent<Collider2D>();     //��� �� �ݶ��̴� ��Ȱ��ȭ
    }



    public virtual void OnHitDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal, Faction projectileFaction)
    {
        float penetratedArmor = armorPoint - penetration;
        float penetratedDamage;
        if (penetratedArmor > 0f)           //���� ����*2 ��ŭ�� ������ ����
        {
            penetratedDamage = damage - (penetratedArmor * 2);
        }else if (damage >= 1f)
        {
            penetratedDamage = 1f;          //�ּ� ������ ����
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

                //����Ʈ �ݿ��� �ڵ� �ۼ�
            }
        }
        
        
    }

    public virtual void Die()
    {
        if(isEssential)
        {
            //�������
            return;
        }
        //onDeath�̺�Ʈ ����ǰ�
        if (onDeath != null)
        {
            onDeath();
        }
        isDead = true;
    }
}
