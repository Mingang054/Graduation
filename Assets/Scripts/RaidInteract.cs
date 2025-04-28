using UnityEngine;

public class RaidInteract : InteractableTrigger
{
    public void Awake()
    {

        interactType = InteractType.Trigger;
        interactionName = "Turn on Radio";
    }
    override public void TriggerInteractable()
    {
        UIManager.Instance.EnableRaid();
    }

}
