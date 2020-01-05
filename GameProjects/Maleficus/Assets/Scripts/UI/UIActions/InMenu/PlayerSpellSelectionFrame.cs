using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSpellSelectionFrame : MaleficusMonoBehaviour
{

   [SerializeField] GameObject Connected;
   [SerializeField] GameObject NotConnected;
   [SerializeField] EPlayerID PlayerID;

    private bool isPlayerConnected = false;

    private int spellCounter = 0;

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

        if (playerID == PlayerID)
        {
            switch (inputButton)
            {
                case EInputButton.CONFIRM:
                    ConnectPlayer();
                    break;

                case EInputButton.CANCEL:
                    if (spellCounter == 0)
                    {
                        DisconnectPlayer();
                    }
                    else
                    {
                        // TODO : remove spell slot
                    }
                    break;
            }
        }
    }

    private void ConnectPlayer()
    {
        Connected.SetActive(true);
        NotConnected.SetActive(false);

        isPlayerConnected = true;

        spellCounter = 0;

        EventManager.Instance.Invoke_PLAYERS_PlayerJoined(PlayerID);
    }

    private void DisconnectPlayer()
    {
        Connected.SetActive(false);
        NotConnected.SetActive(true);

        isPlayerConnected = false;

        EventManager.Instance.Invoke_PLAYERS_PlayerLeft(PlayerID);
    }

    // Update is called once per frame

}
