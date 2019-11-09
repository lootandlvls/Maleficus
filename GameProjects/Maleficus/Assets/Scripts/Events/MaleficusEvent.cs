using UnityEngine;
using System;
using static Maleficus.MaleficusUtilities;

public class MaleficusEvent<H> where H : AbstractEventHandle
{
    public string Name { get; }

    private event Action<H> maleficusEvent;

    public MaleficusEvent(string name)
    {
        Name = name;
    }

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

    public void Invoke(H eventHandle, EEventInvocationType eventInvocationType = EEventInvocationType.TO_ALL)
    {
        if (maleficusEvent != null)
        {
            // Add current time stamp to the event handle
            eventHandle.TimeStamp = GetSystemTime();

            // Invoke event locally
            if ((eventInvocationType != EEventInvocationType.TO_SERVER_ONLY)
                || (MotherOfManagers.Instance.ConnectionMode == EConnectionMode.EVERYTHING_LOCAL))
            {
                // Invoke event to all local listeners
                maleficusEvent.Invoke(eventHandle);

                // Debug event
                string debugMessage = eventHandle.GetDebugMessage();
                if ((MotherOfManagers.Instance.IsDebugLogEvents == true) && (debugMessage != ""))
                {
                    Debug.Log("[EVENT] " + Name + " : " + debugMessage);
                }
            }

            // Broadcast event to the Server
            if (eventInvocationType != EEventInvocationType.LOCAL_ONLY)
            {
                // Broadcast event message to server if not None and if not server
                if ((eventHandle.MessageType != ENetMessageType.NONE) && (NetworkManager.Instance.HasAuthority == false))
                {
                    NetworkManager.Instance.BroadcastNetMessage(eventHandle);
                }
            }
        }
    }


    public void Invoke(H eventHandle)
    {
        Invoke(eventHandle, EEventInvocationType.TO_ALL);
    }

}
