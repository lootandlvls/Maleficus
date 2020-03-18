using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSelectionActivator : BNJMOBehaviour
{
    private MenuCameraMover menuCameraMover;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        MenuCameraMover menuCameraMover = FindObjectOfType<MenuCameraMover>();
        if (IS_NOT_NULL(menuCameraMover))
        {
            menuCameraMover.CameraMovementEnded += On_MenuCameraMover_CameraMovementEnded;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (menuCameraMover)
        {
            menuCameraMover.CameraMovementEnded -= On_MenuCameraMover_CameraMovementEnded;
        }
    }

    private void On_MenuCameraMover_CameraMovementEnded(EMenuState menuState)
    {
        if (menuState == EMenuState.IN_MENU_IN_SPELL_SELECTION)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
