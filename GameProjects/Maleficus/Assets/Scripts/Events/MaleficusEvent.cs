using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaleficusEvent<H> where H : AbstractEventHandle
{
    public string Name { get; }
    //public byte NetMessageID { get; }

    private event Action<H> maleficusEvent;

    public MaleficusEvent(string name)
    {
        Name = name;
        //NetMessageID = NetID.None;
    }

    //public MaleficusEvent(string name, byte netMessageID)
    //{
    //    Name = name;
    //    NetMessageID = netMessageID;
    //}

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
            // Invoke event to all listeners
            maleficusEvent.Invoke(eventHandle);

            // Debug event
            if (MotherOfManagers.Instance.IsDebugLogEvents == true)
            {
                Debug.Log("[EVENT] " + Name + " : " + eventHandle.GetDebugMessage());
            }

            // Broadcast event to server if not None
            AbstractNetMessage netMessage =  eventHandle.GetNetMessage();
            if (netMessage.ID != NetID.None)
            {
                NetworkManager.Instance.BroadcastNetMessage(netMessage);
            }
        }
    }

}
