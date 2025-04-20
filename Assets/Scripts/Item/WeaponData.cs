using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
public class WeaponData : ItemData
{
    public WeaponCategory category;   // 무기 분류 (권총, 소총 등)
    public int damage;                // 기본 데미지
    public int penetration;           // 관통력 (방어력 무시 수치)
    public int damageAP;                // 기본 데미지 
    public int penetrationAP;         // 관통력 (방어력 무시 수치)
    public float fireRate;            // 발사 간격 (초 단위)
    public float fireDelay;           // 발사 지연 시간 (클릭 후 발사까지 지연)
    public float projectileSpeed;     // 투사체 속도
    public int pelletCount;           // 펠릿 수 (산탄총의 경우)
    public float barrelLength;          //투사체생성위치
    public float timeToSwap;           //스왑시간  (X)
    public float colliderSize;                  //투사체 크기
    public float dispersion;            //분산
    public float dispersionRate;        //분산도

    public int magCountMax;             //탄창식은 구현문제 상 탄약 종류에 의존
                                        //light 15, middle...

    public GameObject WeaponPrefab;

    public ReloadType reloadMode;     // 장전 방식 (탄창, 한 발씩 장전)
    public FireMode fireMode;         // 발사 방식 (반자동, 자동)
    public AttackMode attackMode;     // 공격 방식 (히트스캔, 투사체 등)


    public AudioClip attackClip;
    public AudioClip loadClip;
    public AudioClip loadbarClip;
    public AudioClip empty;

    //플레이어 오브젝트에서 나타날 무기와 팔의 모습
    public Sprite weaponArm;
    
    public Sprite weaponBody;           //탄창 있는 스프라이트
    public Sprite weaponUnloadBody;     //탄창 없는 스프라이트
    public Sprite weaponLoadBar;        //장전바의 모습
    public List<Sprite> roadCount;      //리볼버 등의 장전 모습
    public Sprite bullet;

    public AmmoType ammoType;          // 탄약 종류
                                       
    public float durability;          // 내구도
    public float wearRate;            // 마모 계수
}
