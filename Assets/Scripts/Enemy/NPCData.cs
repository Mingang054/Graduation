using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class NPCData : ScriptableObject
{
    public bool essential; //�⺻ false

    // hp �ִ밪 �� �ʱⰪ
    public float health;
    public float armor;

    // �̵��ӵ�
    public float speed;

    public float detectionRange;
    public float fireRange;
    
    public AttackType attckType;
    //public AlertState alertState; ������ ���� ����
    public TrackType trackType;
    public Faction faction;


    public float damage;
    public float penetration;
    public AudioClip attackAudio;

    //bodySprite �ʼ�
    public Sprite bodySprite;
    public AnimationClip bodyAnimation;

    public Sprite armSprite;
    public AnimationClip armAnimation;

    public Sprite attackSprite;
    public AnimationClip attackAnimation;


    public AnimationClip deathAnimation;
    public AudioClip deathAudio;

    public AudioClip onHitAudio;
    // ��� ���̺� ������ �ڵ�, �������� ���� ����Ʈ�� ������ list<stirng,int,int>

    public Sprite projectileSprite;

}
