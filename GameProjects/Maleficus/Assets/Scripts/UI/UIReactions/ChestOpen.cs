using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class ChestOpen : BNJMOBehaviour
{
    [SerializeField] private Transform chestTransform;

    private AnimationLerpTransform lerpTransform;

    private bool isClosed = true;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        lerpTransform = GetComponentWithCheck<AnimationLerpTransform>();
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.UI_MenuStateUpdated.Event += On_UI_MenuStateUpdated_Event;    
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.UI_MenuStateUpdated.Event -= On_UI_MenuStateUpdated_Event;    
        }
    }

    private void On_UI_MenuStateUpdated_Event(Event_StateUpdated<EMenuState> eventHandle)
    {
        EMenuState newMenuState = eventHandle.NewState;
        
        if (newMenuState == EMenuState.IN_MENU_IN_SPELL_SELECTION)
        {
            lerpTransform.PlayInReverse = false;
            lerpTransform.StartAnimation(chestTransform);
            isClosed = false;
        }
        else if (isClosed == false)
        {
            lerpTransform.PlayInReverse = true;
            lerpTransform.StartAnimation(chestTransform);
            isClosed = true;
        }
    }
}
