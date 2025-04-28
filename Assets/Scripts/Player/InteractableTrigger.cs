using UnityEngine;
using UnityEngine.Events;  // ✅ 추가해야 함

public class InteractableTrigger : Interactable
{
    private void Awake()
    {
        interactType = InteractType.Trigger;
    }
    
    virtual public void TriggerInteractable()
    {

    }

}
