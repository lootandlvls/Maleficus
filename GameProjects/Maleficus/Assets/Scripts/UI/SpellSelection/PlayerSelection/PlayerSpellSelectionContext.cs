using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSpellSelectionContext : MaleficusMonoBehaviour
{
    public EPlayerID PlayerID { get { return playerID; } }


    [Header("Player Spell Selection Context")]
    [SerializeField] EPlayerID playerID;
    [SerializeField] GameObject connectedView;
    [SerializeField] GameObject notConnectedView;
    [SerializeField] GameObject spellSelectionView;
    [SerializeField] GameObject readyView;


    private ESpellSelectionState spellSelectionState = ESpellSelectionState.NOT_CONNECTED;
    private enum ESpellSelectionState
    {
        NOT_CONNECTED,
        CHOOSING_SPELLS,
        READY
    }

    private Dictionary<ESpellSlot, SelectedSpell> selectedSpellsIcons = new Dictionary<ESpellSlot, SelectedSpell>();
    private int selectedSpellsCounter = 0;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed_Event;
        EventManager.Instance.APP_AppStateUpdated.Event += On_APP_AppStateUpdated_Event;        

    }

    private void On_APP_AppStateUpdated_Event(Event_StateUpdated<EAppState> eventHandle)
    {
        
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Initialize selected spells
        connectedView.SetActive(true);
        foreach (SelectedSpell SelectedSpell in GetComponentsInChildren<SelectedSpell>())
        {
            selectedSpellsIcons.Add(SelectedSpell.SpellSlot, SelectedSpell);
        }
        connectedView.SetActive(false);
    }

    private void On_INPUT_ButtonPressed_Event(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = Maleficus.MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);


        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
        {
            if (playerID == this.playerID)
            {
                switch (inputButton)
                {
                    case EInputButton.CONFIRM:
                        switch (spellSelectionState)
                        {
                            case ESpellSelectionState.NOT_CONNECTED:
                                ConnectPlayer();
                                break;

                            case ESpellSelectionState.CHOOSING_SPELLS:

                                AbstractSpell spell = SpellSelectionManager.Instance.GetHighlightedSpellButton(playerID).Spell;

                                switch (selectedSpellsCounter)
                                {
                                    case 0:
                                        if (selectedSpellsIcons[ESpellSlot.SPELL_1] != null)
                                        {
                                            CheckAndAddSpell(playerID, spell, ESpellSlot.SPELL_1);

                                        }
                                        break;
                                    case 1:
                                        if (selectedSpellsIcons[ESpellSlot.SPELL_2] != null)
                                        {
                                            CheckAndAddSpell(playerID, spell, ESpellSlot.SPELL_2);
                                        }
                                        break;
                                    case 2:
                                        if (selectedSpellsIcons[ESpellSlot.SPELL_3] != null)
                                        {
                                            CheckAndAddSpell(playerID, spell, ESpellSlot.SPELL_3);
                                        }
                                        break;

                                    case 3:
                                        SetToReady();
                                        break;
                                    
                                }
                                break;
                        }
                        break;

                    case EInputButton.CANCEL:
                        switch (spellSelectionState)
                        {
                            case ESpellSelectionState.CHOOSING_SPELLS:
                                if (selectedSpellsCounter == 0)
                                {
                                    DisconnectPlayer();
                                }
                                else
                                {
                                    RemoveSpell(playerID);
                                }
                                break;

                            case ESpellSelectionState.READY:
                                CancelReady();
                                break;
                        }

                        break;
                }
            }
        }
    }

   


    private void CheckAndAddSpell(EPlayerID playerID, AbstractSpell spell , ESpellSlot spellSlot)
    {
        if (SpellManager.Instance.CheckPlayerSpells(playerID, spell))
        {
            LogConsole("Spell 1 has been Chosen");
            selectedSpellsIcons[spellSlot].ChangeImage(spell);
            EventManager.Instance.Invoke_UI_SpellChosen(playerID, spell, spellSlot);
            if (selectedSpellsCounter < 3)
            {
                selectedSpellsCounter++;
            }
        }
    }

    //Remove a spell 
    private void RemoveSpell(EPlayerID playerID)
    {
        if (playerID == this.playerID)
        {
            switch (selectedSpellsCounter)
            {
                case 1:
                    if (selectedSpellsIcons[ESpellSlot.SPELL_1] != null)
                    {
                        LogConsole("Spell 1 has been Chosen");
                        selectedSpellsIcons[ESpellSlot.SPELL_1].RemoveImage();
                        EventManager.Instance.Invoke_UI_SpellRemoved(playerID, ESpellSlot.SPELL_1);
                    }
                    break;

                case 2:
                    if (selectedSpellsIcons[ESpellSlot.SPELL_2] != null)
                    {
                        LogConsole("Spell 2 has been Chosen");
                        selectedSpellsIcons[ESpellSlot.SPELL_2].RemoveImage();
                        EventManager.Instance.Invoke_UI_SpellRemoved(playerID, ESpellSlot.SPELL_2);
                    }
                    break;

                case 3:
                    if (selectedSpellsIcons[ESpellSlot.SPELL_3] != null)
                    {
                        LogConsole("Spell 3 has been Chosen");
                        selectedSpellsIcons[ESpellSlot.SPELL_3].RemoveImage();
                        EventManager.Instance.Invoke_UI_SpellRemoved(playerID, ESpellSlot.SPELL_3);
                    }
                    break;
            }
            if (selectedSpellsCounter >= 0)
            {
                selectedSpellsCounter--;
            }
            LogConsole("Counter : " + selectedSpellsCounter);
        }
    }

    

    private void ConnectPlayer()
    {
        UpdateState(ESpellSelectionState.CHOOSING_SPELLS);

        selectedSpellsCounter = 0;

        EventManager.Instance.Invoke_PLAYERS_PlayerJoined(playerID);
    }

    private void DisconnectPlayer()
    {
        UpdateState(ESpellSelectionState.NOT_CONNECTED);

        EventManager.Instance.Invoke_PLAYERS_PlayerLeft(playerID);
    }

    private void SetToReady()
    {
        UpdateState(ESpellSelectionState.READY);

        EventManager.Instance.Invoke_PLAYERS_PlayerReady(playerID);
    }

    private void CancelReady()
    {
        UpdateState(ESpellSelectionState.CHOOSING_SPELLS);

        EventManager.Instance.Invoke_PLAYERS_PLAYERS_PlayerCanceledReady(playerID);
    }


    private void UpdateState(ESpellSelectionState newSpellSelectionState)
    {
        spellSelectionState = newSpellSelectionState;
        switch (newSpellSelectionState)
        {
            case ESpellSelectionState.NOT_CONNECTED:
                notConnectedView.SetActive(true);

                connectedView.SetActive(false);
                break;

            case ESpellSelectionState.CHOOSING_SPELLS:
                notConnectedView.SetActive(false);

                connectedView.SetActive(true);
                spellSelectionView.SetActive(true);
                readyView.SetActive(false);

                break;

            case ESpellSelectionState.READY:
                connectedView.SetActive(true);
                spellSelectionView.SetActive(false);
                readyView.SetActive(true);
                break;
        }
    }
   
}
