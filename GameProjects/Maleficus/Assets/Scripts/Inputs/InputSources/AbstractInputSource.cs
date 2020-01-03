using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbstractInputSource : MaleficusMonoBehaviour
{
    public event Action<EControllerID, EInputButton>                        ButtonPressed;
    public event Action<EControllerID, EInputButton>                        ButtonReleased;
    public event Action<EControllerID, EJoystickType, float, float>         JoystickMoved;


    protected void InvokeButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        if (ButtonPressed != null)
        {
            ButtonPressed.Invoke(controllerID, inputButton);
        }
    }

    protected void InvokeButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        if (ButtonReleased != null)
        {
            ButtonReleased.Invoke(controllerID, inputButton);
        }
    }

    protected void InvokeJoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        if (JoystickMoved != null)
        {
            JoystickMoved.Invoke(controllerID, joystickType, x, y);
        }
    }


}
