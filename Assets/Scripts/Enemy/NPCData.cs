using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class NPCData : ScriptableObject
{
    public bool essential; //기본 false

    // hp 최대값 및 초기값
    public float health;
    public float armor;

    // 이동속도
    public float speed;

    public float detectionRange;
    public float fireRange;
    
    public AttackType attckType;
    //public AlertState alertState; 경계상태 없이 생성
    public TrackType trackType;
    public Faction faction;


    public float damage;
    public float penetration;
    public AudioClip attackAudio;

    //bodySprite 필수
    public Sprite bodySprite;
    public AnimationClip bodyAnimation;

    public Sprite armSprite;
    public AnimationClip armAnimation;

    public Sprite attackSprite;
    public AnimationClip attackAnimation;


    public AnimationClip deathAnimation;
    public AudioClip deathAudio;

    public AudioClip onHitAudio;
    // 드롭 테이블 아이템 코드, 수량범위 쌍을 리스트로 가지게 list<stirng,int,int>

    public Sprite projectileSprite;

}
