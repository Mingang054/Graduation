using UnityEngine;

[CreateAssetMenu(menuName = "Vest/VestSpriteSet")]
public class VestSpriteSet : ScriptableObject
{
    //mag 방식
    //0은 empty
    public Sprite[] pistolSprite = new Sprite[4];

    public Sprite[] lightSprite = new Sprite[4];
    public Sprite[] mediumSprite = new Sprite[3];
    public Sprite[] heavySprite = new Sprite[3];
    //single 방식
    public Sprite[] shellSprite = new Sprite[9];
    public Sprite[] magnumSprite;


    public Sprite equippedSprite;

}