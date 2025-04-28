using UnityEngine;

public class radioInteracter : InteractableTrigger
{
    public void Awake()
    {

        interactType = InteractType.Trigger;
        interactionName = "Turn on Radio";
    }
    override public void TriggerInteractable()
    {
        UIManager.Instance.EnableRadio();
    }

}
