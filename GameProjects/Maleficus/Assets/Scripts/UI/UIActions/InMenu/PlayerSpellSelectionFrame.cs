using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSpellSelectionFrame : MaleficusMonoBehaviour
{

   [SerializeField] GameObject Connected;
   [SerializeField] GameObject NotConnected;
   [SerializeField] EPlayerID PlayerID;
    // Start is called before the first frame update
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed_Event;
    }
   

    private void On_INPUT_ButtonPressed_Event(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = Maleficus.MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);
        
        if (inputButton == EInputButton.CONFIRM && playerID == PlayerID)
        {
            Connected.SetActive(true);
            NotConnected.SetActive(false);
        }
    }

    

    // Update is called once per frame

}
