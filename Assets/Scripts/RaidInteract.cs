using UnityEngine;

public class RaidInteract : InteractableTrigger
{
    public void Awake()
    {

        interactType = InteractType.Trigger;
        interactionName = "Enter Raid";
    }
    override public void TriggerInteractable()
    {
        UIManager.Instance.EnableRaid();
    }

}
