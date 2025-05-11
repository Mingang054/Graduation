using UnityEngine;

public class Ond : MonoBehaviour
{
    public RadioTalk target;
    public RadioTalkType type;
    
    
    private void OnDisable()
    {
        target.TryTalk(type);
    }
}