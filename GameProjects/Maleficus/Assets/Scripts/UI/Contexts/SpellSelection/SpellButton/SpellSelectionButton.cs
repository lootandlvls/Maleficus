using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellSelectionButton : BNJMOBehaviour
{
    public AbstractSpell Spell                  { get { return spell; } }
    public MaleficusButton MaleficusButton      { get { return GetComponent<MaleficusButton>(); } }
    public int RowIndex                         { get { return rowIndex; } }
    public int ColumnIndex                      { get { return columnIndex; } }

    [Header("Spell Selection Button")]
    [SerializeField] private bool disableDebugTextOnStart = true;
    [SerializeField] private AbstractSpell spell;
    [SerializeField] private int rowIndex;
    [SerializeField] private int columnIndex;
    [SerializeField] private SpellSkillPointText skillPointText;

    private Button myButton;
    private Text myDebugIndexText;
    private Dictionary<EPlayerID, SpellSelectionPlayerHighlight> playerHighlights = new Dictionary<EPlayerID, SpellSelectionPlayerHighlight>();


    protected override void Start()
    {
        base.Start();

        if ((myDebugIndexText != null) && (disableDebugTextOnStart == true))
        {
            myDebugIndexText.enabled = false;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        // Reinitialize debug text from object name
        skillPointText = GetComponentInChildren<SpellSkillPointText>();
        myDebugIndexText = GetComponentInChildren<Text>();
        if (myDebugIndexText != null)
        {
            myDebugIndexText.text = RowIndex + "-" + ColumnIndex;

            // Update the name of the gameobject accordingly
            name = "B_SpellSelectionButton " + RowIndex + "-" + ColumnIndex;
        }

        // Reinitialize button image 
        myButton = GetComponent<Button>();
        Text SPtext = skillPointText.GetComponent<Text>();
        if (myButton != null)
        {
            if (spell != null)
            {
                myButton.image.sprite = spell.SpellIcon;
                SPtext.text = spell.SkillPoint + "";
                // Update the name of the gameobject accordingly
                name = "B_SpellSelectionButton " + RowIndex + "-" + ColumnIndex + " : " + spell.SpellName;
            }
        }
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Initialize button image 
        myButton = GetComponent<Button>();
        if (IS_NOT_NULL(myButton))
        {
            if (IS_NOT_NULL(spell))
            {
                myButton.image.sprite = spell.SpellIcon;
            }
        }

        // Initialize debug text
        myDebugIndexText = GetComponentInChildren<Text>();
        if (IS_NOT_NULL(myDebugIndexText))
        {
            myDebugIndexText.text = RowIndex + "-" + ColumnIndex;
        }

        // Initialize player highlights
        foreach (var playerHighlight in GetComponentsInChildren<SpellSelectionPlayerHighlight>())
        {
            if ((IS_VALUE_NOT_CONTAINED(playerHighlights, playerHighlight))
                && (IS_KEY_NOT_CONTAINED(playerHighlights, playerHighlight.PlayerID)))
            {
                playerHighlights.Add(playerHighlight.PlayerID, playerHighlight);
            }
        }
        IS_KEY_CONTAINED(playerHighlights, EPlayerID.PLAYER_1);
        IS_KEY_CONTAINED(playerHighlights, EPlayerID.PLAYER_2);
        IS_KEY_CONTAINED(playerHighlights, EPlayerID.PLAYER_3);
        IS_KEY_CONTAINED(playerHighlights, EPlayerID.PLAYER_4);
    }

    public void HighlightPlayerSelection(EPlayerID playerID)
    {
        if (playerID != EPlayerID.NONE)
        {
            if ((IS_KEY_CONTAINED(playerHighlights, playerID))
                && (IS_NOT_NULL(playerHighlights[playerID])))
            {
                playerHighlights[playerID].ShowHighlight();
            }
        }
    }

    public void UnHighlightPlayerSelection(EPlayerID playerID)
    {
        if (playerID != EPlayerID.NONE)
        {
            if ((IS_KEY_CONTAINED(playerHighlights, playerID))
                && (IS_NOT_NULL(playerHighlights[playerID])))
            {
                playerHighlights[playerID].HideHighlight();
            }
        }
    }

    public void PressSpellButton(EPlayerID playerID)
    {
        if (playerID != EPlayerID.NONE)
        {

        }
    }
}
