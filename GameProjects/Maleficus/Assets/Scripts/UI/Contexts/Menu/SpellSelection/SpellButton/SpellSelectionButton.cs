using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellSelectionButton : BNJMOBehaviour
{
    public AbstractSpell Spell                  { get { return spell; } }
    public BNJMOButton MaleficusButton      { get { return GetComponent<BNJMOButton>(); } }
    public int RowIndex                         { get { return rowIndex; } }
    public int ColumnIndex                      { get { return columnIndex; } }

    [Header("Spell Selection Button")]
    [SerializeField] private bool disableDebugTextOnStart = true;
    [SerializeField] private AbstractSpell spell;
    [SerializeField] private int rowIndex;
    [SerializeField] private int columnIndex;
    [SerializeField] private SpellSkillPointStar[] skillPointStars = new SpellSkillPointStar[3];


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

        // Update the debug text
        myDebugIndexText = GetComponentInChildren<Text>();
        if (myDebugIndexText != null)
        {
            myDebugIndexText.text = RowIndex + "-" + ColumnIndex;
        }

        // Update the name of the gameobject
        name = "B_SpellSelectionButton " + RowIndex + "-" + ColumnIndex;

        // Update according to given spell
        if (spell != null)
        {
            // Update the name of the gameobject accordingly
            name = "B_SpellSelectionButton " + RowIndex + "-" + ColumnIndex + " : " + spell.SpellName;

            // Update Button image 
            myButton = GetComponent<Button>();
            if (myButton != null)
            {
                myButton.image.sprite = spell.SpellIcon;
            }

            // Update Stars count
            if (ARE_EQUAL(skillPointStars.Length, 3))
            {
                for (int i = 0; i < 3; i++)
                {
                    SpellSkillPointStar spellSkillPointStar = skillPointStars[i];
                    if (IS_NOT_NULL(spellSkillPointStar))
                    {
                        if (spell.SkillPoint >= i + 1)
                        {
                            spellSkillPointStar.Show();
                        }
                        else
                        {
                            spellSkillPointStar.Hide();
                        }
                    }
                }
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
