using UnityEngine;

[CreateAssetMenu(menuName = "Vest/VestSpriteSet")]
public class VestSpriteSet : ScriptableObject
{
    //mag ���
    //0�� empty
    public Sprite[] pistolSprite = new Sprite[4];

    public Sprite[] lightSprite = new Sprite[4];
    public Sprite[] mediumSprite = new Sprite[3];
    public Sprite[] heavySprite = new Sprite[3];
    //single ���
    public Sprite[] shellSprite = new Sprite[9];
    public Sprite[] magnumSprite;


    public Sprite equippedSprite;

}