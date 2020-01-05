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

    private Dictionary<ESpellSlot, SelectedSpell> selectedSpellsIcons = new Dictionary<ESpellSlot, SelectedSpell>();

    // Start is called before the first frame update
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        SpellSelectionManager.Instance.SpellButtonPressed += OnSpellButtonPressed;
        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed_Event;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Initialize selected spells
        Connected.SetActive(true);
        foreach (SelectedSpell SelectedSpell in GetComponentsInChildren<SelectedSpell>())
        {
            selectedSpellsIcons.Add(SelectedSpell.SpellSlot, SelectedSpell);
            LogConsole("adding : " + SelectedSpell.SpellSlot);

        }
        LogConsole("SelectedSpell initialzed " + selectedSpellsIcons.Count);
        Connected.SetActive(false);


    }

    private void OnSpellButtonPressed(EPlayerID playerID, AbstractSpell spell)
    {
        switch (spellCounter)
            {
            case 0:
                if (selectedSpellsIcons[ESpellSlot.SPELL_1] != null)
                {
                    selectedSpellsIcons[ESpellSlot.SPELL_1].ChangeImage(spell);
                    EventManager.Instance.Invoke_UI_SpellChosen(playerID, spell, ESpellSlot.SPELL_1);
                }
                break;
            case 1:
                if (selectedSpellsIcons[ESpellSlot.SPELL_2] != null)
                {
                    selectedSpellsIcons[ESpellSlot.SPELL_1].ChangeImage(spell);
                    EventManager.Instance.Invoke_UI_SpellChosen(playerID, spell, ESpellSlot.SPELL_1);
                }
                break;
            case 2:
                if (selectedSpellsIcons[ESpellSlot.SPELL_3] != null)
                {
                    selectedSpellsIcons[ESpellSlot.SPELL_1].ChangeImage(spell);
                    EventManager.Instance.Invoke_UI_SpellChosen(playerID, spell, ESpellSlot.SPELL_1);
                }
                break;

        }
        if (spellCounter < 3)
        {
            spellCounter++;
        }

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
