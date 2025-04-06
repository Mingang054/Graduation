using UnityEngine;

public class InteractableTrigger : Interactable
{
    private void Awake()
    {
        interactType = InteractType.Trigger;
    }

}
