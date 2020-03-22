﻿using System.Collections.Generic;
using UnityEngine;
using System;

using static Maleficus.Utils;

public enum EHighlightSpellSide
{
    NONE = 0,
    LEFT = 1,
    RIGHT = 2
}


public class PlayerSpellSelectionContext : BNJMOBehaviour
{
    public event Action<EPlayerID> LeaveRequest;
    public event Action<EPlayerID> ReadyRequest;
    public event Action<EPlayerID> CancelReadyRequest;

    public EPlayerID PlayerID { get { return playerID; } }

    [Header("Player Spell Selection Context")]
    [SerializeField] private EPlayerID playerID;
    [SerializeField] private EHighlightSpellSide highlightSpellSide = EHighlightSpellSide.NONE;
    [SerializeField] private GameObject connectedView;
    [SerializeField] private GameObject notConnectedView;
    [SerializeField] private GameObject spellSelectionView;
    [SerializeField] private GameObject readyView;
    [SerializeField] private GameObject spellStartPosition;
    [SerializeField] private HighlightedSpell[] highlightedSpells = new HighlightedSpell[0];

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

    protected override void OnValidate()
    {
        base.OnValidate();

        // Updated visible Highlighted Spell Side
        foreach (HighlightedSpell highlightedSpell in highlightedSpells)
        {
            if (highlightedSpell)
            {
                highlightedSpell.PlayerID = PlayerID;
                if (highlightedSpell.HighlightSpellSide == highlightSpellSide)
                {
                    highlightedSpell.Show();
                }
                else
                {
                    highlightedSpell.Hide();
                }
            }
        }

        // Rotate the spell object position correctly
        switch (highlightSpellSide)
        {
            case EHighlightSpellSide.LEFT:
                spellStartPosition.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, -90.0f));
                break;

            case EHighlightSpellSide.RIGHT:
                spellStartPosition.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 90.0f));
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        IS_NOT_NONE(highlightSpellSide);
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

        // Updated Highlighted PlayerID
        foreach (HighlightedSpell highlightedSpell in highlightedSpells)
        {
            highlightedSpell.gameObject.SetActive(true);
        }

        // Update Spell Collision
        PlayerSelectionSpellCollision spellCollision = GetComponentInChildren<PlayerSelectionSpellCollision>();
        if (IS_NOT_NULL(spellCollision))
        {
            spellCollision.PlayerID = PlayerID;
        }
    }

    protected override void Start()
    {
        base.Start();

        UpdateState(ESpellSelectionState.NOT_CONNECTED);
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.PLAYERS_PlayerJoined += On_PLAYERS_PlayerJoined;
        EventManager.Instance.PLAYERS_PlayerLeft += On_PLAYERS_PlayerLeft;
        EventManager.Instance.PLAYERS_PlayerReady += On_PLAYERS_PlayerReady;
        EventManager.Instance.PLAYERS_PlayerCanceledReady += On_PLAYERS_PlayerCanceledReady;

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed_Event;
        EventManager.Instance.UI_SpellChosen += On_UI_SpellChosen;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearEventCallbakcs(LeaveRequest);
        ClearEventCallbakcs(ReadyRequest);
        ClearEventCallbakcs(CancelReadyRequest);

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.PLAYERS_PlayerJoined -= On_PLAYERS_PlayerJoined;
            EventManager.Instance.PLAYERS_PlayerLeft -= On_PLAYERS_PlayerLeft;
            EventManager.Instance.PLAYERS_PlayerReady -= On_PLAYERS_PlayerReady;
            EventManager.Instance.PLAYERS_PlayerCanceledReady -= On_PLAYERS_PlayerCanceledReady;

            EventManager.Instance.INPUT_ButtonPressed.Event -= On_INPUT_ButtonPressed_Event;
            EventManager.Instance.UI_SpellChosen -= On_UI_SpellChosen;
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


    #region Event Callbacks
    private void On_PLAYERS_PlayerJoined(EPlayerID playerID, EControllerID controllerID)
    {
        if (playerID == this.playerID)
        {
            UpdateState(ESpellSelectionState.CHOOSING_SPELLS);
            playerSkillPointsIndicator.ResetSkillPoints();
            selectedSpellsCounter = 0;
        }
    }

    private void On_PLAYERS_PlayerLeft(EPlayerID playerID)
    {
        if (playerID == this.playerID)
        {
            UpdateState(ESpellSelectionState.NOT_CONNECTED);
        }
    }

    private void On_PLAYERS_PlayerReady(EPlayerID playerID)
    {
        if (playerID == this.playerID)
        {
            UpdateState(ESpellSelectionState.READY);
        }
    }

    private void On_PLAYERS_PlayerCanceledReady(EPlayerID playerID)
    {
        if (playerID == this.playerID)
        {
            UpdateState(ESpellSelectionState.CHOOSING_SPELLS);
        }
    }

    private void On_INPUT_ButtonPressed_Event(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EControllerID controllerID = eventHandle.ControllerID;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);


        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
        {
            if (playerID == this.playerID)
            {
                switch (inputButton)
                {
                    case EInputButton.CONFIRM:
                        LogConsole(playerID + " - Player selection  : " + selectedSpellsCounter);
                        switch (spellSelectionState)
                        {
                            case ESpellSelectionState.NOT_CONNECTED:
                                // Player join is handeled by PlayerManager
                                break;

                            case ESpellSelectionState.CHOOSING_SPELLS:
                                AbstractSpell spell = SpellSelectionManager.Instance.GetHighlightedSpellButton(playerID).Spell;

                                if (selectedSpellsCounter == 3)
                                {
                                    LogConsole("ReadyRequest");
                                    InvokeEventIfBound(ReadyRequest, playerID);
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
                                    InvokeEventIfBound(LeaveRequest, playerID);
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
                                InvokeEventIfBound(CancelReadyRequest, playerID);
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

    #endregion

}