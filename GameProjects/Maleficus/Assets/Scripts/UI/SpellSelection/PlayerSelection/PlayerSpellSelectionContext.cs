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
    [SerializeField] private GameObject spellStartPosition;

    
    private PlayerSkillPointsIndicator playerSkillPointsIndicator;

    private ESpellSelectionState spellSelectionState = ESpellSelectionState.NOT_CONNECTED;
    private enum ESpellSelectionState
    {
        NOT_CONNECTED,
        CHOOSING_SPELLS,
        READY
    }

    private Dictionary<ESpellSlot, SelectedSpell> selectedSpells = new Dictionary<ESpellSlot, SelectedSpell>();
    private int selectedSpellsCounter = 0;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed_Event;
        EventManager.Instance.UI_SpellChosen            += On_UI_SpellChosen;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.INPUT_ButtonPressed.Event -= On_INPUT_ButtonPressed_Event;
            EventManager.Instance.UI_SpellChosen            -= On_UI_SpellChosen;
        }
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Initialize selected spells
        connectedView.SetActive(true);

        foreach (SelectedSpell SelectedSpell in GetComponentsInChildren<SelectedSpell>())
        {
            selectedSpells.Add(SelectedSpell.SpellSlot, SelectedSpell);
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
                                    SetToReady();
                                }
                                else 
                                {
                                    // Get and check spell slot
                                    ESpellSlot spellSlot = GetNextUnselectedSpellSlotFromLeft();
                                    if ((IS_NOT_NONE(spellSlot))
                                        && (CanAddSpell(spell, spellSlot)))
                                    {
                                        AddSpell(spell, spellSlot);
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
                                    ESpellSlot spellSlot = GetNextSelectedSpellSlotFromRight();
                                    if (IS_NOT_NONE(spellSlot))
                                    {
                                        RemoveSpell(spellSlot);
                                    }
                                }
                                break;

                            case ESpellSelectionState.READY:
                                CancelReady();
                                break;
                        }

                        break;

                    default:
                        if (spellSelectionState == ESpellSelectionState.CHOOSING_SPELLS)
                        {
                            AbstractSpell spell = SpellSelectionManager.Instance.GetHighlightedSpellButton(playerID).Spell;

                            ESpellSlot spellSlot = GetSpellSlotFrom(inputButton);
                            if (spellSlot != ESpellSlot.NONE)
                            {
                                if (CanAddSpell(spell, spellSlot))
                                {
                                    if (selectedSpells[spellSlot].IsSelected == true)
                                    {
                                        RemoveSpell(spellSlot);
                                    }
                                    AddSpell(spell, spellSlot);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    private void On_UI_SpellChosen(EPlayerID playerID, AbstractSpell abstractSpell, ESpellSlot spellSlot)
    {
        if (playerID == PlayerID)
        {
            SpellManager.Instance.SpawnPreviewSpell(abstractSpell, spellStartPosition.transform);
        }
    }


    private void AddSpell(AbstractSpell spell , ESpellSlot spellSlot)
    {
        playerSkillPointsIndicator.RemoveSkillPoints(spell.SkillPoint);

        selectedSpells[spellSlot].SelectSpell(spell);
                
        EventManager.Instance.Invoke_UI_SpellChosen(playerID, spell, spellSlot);
        if (selectedSpellsCounter < 3)
        {
            selectedSpellsCounter++;
        }
    }

    private bool CanAddSpell(AbstractSpell spell, ESpellSlot spellSlot)
    {
        if ((SpellManager.Instance.IsSpellAlreadyChosen(playerID, spell) == false)
            && (IS_NOT_NULL(playerSkillPointsIndicator)))
        {
            if (selectedSpells[spellSlot].IsSelected == true)
            {
                if (playerSkillPointsIndicator.CanSwapSpell(spell, selectedSpells[spellSlot].CurrentSelectedSpell) == true)
                {
                    LogConsole("spell will be swapped");
                    return true;
                }
            }
            else
            {
                if (playerSkillPointsIndicator.CanChoseSpell(spell) == true)
                {
                    LogConsole("spell will be chosen");
                    return true;
                }
            }
        }
        return false;
    }

    private void RemoveSpell(ESpellSlot spellSlot)
    {
        if (selectedSpells[spellSlot] != null)
        {
            selectedSpells[spellSlot].RemoveSpell();
            LogConsole("Removing spell");
            AbstractSpell spell = SpellManager.Instance.playersChosenSpells[playerID][spellSlot];
            if (playerSkillPointsIndicator != null && spell != null)
            {
                LogConsole("Adding SKP");
                playerSkillPointsIndicator.AddSkillPoints(spell.SkillPoint);
            }
            EventManager.Instance.Invoke_UI_SpellRemoved(playerID, spellSlot);
        }
                 
        if (selectedSpellsCounter >= 0)
        {
            selectedSpellsCounter--;
        }
            
        LogConsole("Counter : " + selectedSpellsCounter);
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
   

    private ESpellSlot ConvertSpellSlot(int index)
    {
        LogConsole("convertin : " + index);
        switch (index)
        {
            case 0:
                return ESpellSlot.SPELL_2;

            case 1:
                return ESpellSlot.SPELL_1;
            
            case 2:
                return ESpellSlot.SPELL_3;
        }
        return ESpellSlot.NONE;
    }

    private ESpellSlot GetNextSelectedSpellSlotFromRight()
    {
        ESpellSlot spellSlot = ESpellSlot.NONE;
        for (int i = 2; i >= 0; i--)
        {
            spellSlot = ConvertSpellSlot(i);
            LogConsole("i : " + i + " | spell slot " + spellSlot);

            if (selectedSpells[spellSlot].IsSelected == true)
            {
                return spellSlot;
            }
        }
        return ESpellSlot.NONE;
    }

    private ESpellSlot GetNextUnselectedSpellSlotFromLeft()
    {
        ESpellSlot spellSlot = ESpellSlot.NONE;
        for (int i = 0; i <= 2; i++)
        {
            spellSlot = ConvertSpellSlot(i);
            LogConsole("i : " + i + " | spell slot " + spellSlot);

            if (selectedSpells[spellSlot].IsSelected == false)
            {
                return spellSlot;
            }
        }
        return ESpellSlot.NONE;
    }


}
