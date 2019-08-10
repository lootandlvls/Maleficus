using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaleficusEvent<H> where H : AbstractEventHandle
{
    private event Action<H> maleficusEvent;

    public void AddListener(Action<H> callbackAction)
    {
        maleficusEvent += callbackAction;
    }

    public void RemoveListener(Action<H> callbackAction)
    {
        maleficusEvent -= callbackAction;
    }

    public void ClearAllListeners()
    {
        Delegate[] delegates = maleficusEvent.GetInvocationList();
        foreach (Delegate myDelegate in delegates)
        {
            maleficusEvent -= (myDelegate as Action<H>);
        }
    }

    public void Invoke(H eventHandle)
    {
        if (maleficusEvent != null)
        {
            maleficusEvent.Invoke(eventHandle);
        }
    }

}
