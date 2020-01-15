using System.Collections.Generic;
using UnityEngine;

using static Maleficus.Utils;

public class PlayerSpellSelectionContext : BNJMOBehaviour
{
    public EPlayerID PlayerID { get { return playerID; } }


    [Header("Player Spell Selection Context")]
    [SerializeField] private EPlayerID playerID;
    [SerializeField] private GameObject connectedView;
    [SerializeField] private GameObject notConnectedView;
    [SerializeField] private GameObject spellSelectionView;
    [SerializeField] private GameObject readyView;
    
    private PlayerSkillPointsIndicator playerSkillPointsIndicator;

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

        playerSkillPointsIndicator = GetComponentInChildren<PlayerSkillPointsIndicator>();
        IS_NOT_NULL(playerSkillPointsIndicator);

        connectedView.SetActive(false);
        
    }

    protected override void Start()
    {
        base.Start();

        UpdateState(ESpellSelectionState.NOT_CONNECTED);
    }

    private void On_INPUT_ButtonPressed_Event(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);


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

                                if (selectedSpellsCounter == 3)
                                {
                                    LogConsole("Set ready");
                                    SetToReady();
                                }
                                else 
                                {
                                    // Get and check spell slot
                                    ESpellSlot spellSlot = GetSpellSlotFrom(selectedSpellsCounter + 1);
                                    if (IS_NOT_NULL(spellSlot))
                                    {
                                        CheckAndAddSpell(playerID, spell, spellSlot);
                                    }
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
            if ((IS_NOT_NULL(playerSkillPointsIndicator))
                && (playerSkillPointsIndicator.CanChoseSpell(spell) == true))
            {
                playerSkillPointsIndicator.RemoveSkillPoints(spell.SkillPoint);

                selectedSpellsIcons[spellSlot].ChangeImage(spell);

                EventManager.Instance.Invoke_UI_SpellChosen(playerID, spell, spellSlot);
                if (selectedSpellsCounter < 3)
                {
                    selectedSpellsCounter++;
                }
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
                        AbstractSpell spell = SpellManager.Instance.playersChosenSpells[playerID][ESpellSlot.SPELL_1];
                        if (playerSkillPointsIndicator != null && spell != null)
                        {
                            playerSkillPointsIndicator.AddSkillPoints(spell.SkillPoint);
                        }
                        EventManager.Instance.Invoke_UI_SpellRemoved(playerID, ESpellSlot.SPELL_1);
                    }
                    break;

                case 2:
                    if (selectedSpellsIcons[ESpellSlot.SPELL_2] != null)
                    {
                        LogConsole("Spell 2 has been Chosen");
                        selectedSpellsIcons[ESpellSlot.SPELL_2].RemoveImage();
                        AbstractSpell spell = SpellManager.Instance.playersChosenSpells[playerID][ESpellSlot.SPELL_2];
                        if (playerSkillPointsIndicator != null && spell != null)
                        {
                            playerSkillPointsIndicator.AddSkillPoints(spell.SkillPoint);
                        }
                        EventManager.Instance.Invoke_UI_SpellRemoved(playerID, ESpellSlot.SPELL_2);
                    }
                    break;

                case 3:
                    if (selectedSpellsIcons[ESpellSlot.SPELL_3] != null)
                    {
                        LogConsole("Spell 3 has been Chosen");
                        selectedSpellsIcons[ESpellSlot.SPELL_3].RemoveImage();
                        AbstractSpell spell = SpellManager.Instance.playersChosenSpells[playerID][ESpellSlot.SPELL_3];
                        if (playerSkillPointsIndicator != null && spell != null)
                        {
                            playerSkillPointsIndicator.AddSkillPoints(spell.SkillPoint);
                        }
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
        playerSkillPointsIndicator.ResetSkillPoints();
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
