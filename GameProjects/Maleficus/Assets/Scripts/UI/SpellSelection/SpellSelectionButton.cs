using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpellSelectionUIAction))]
public class SpellSelectionButton : MaleficusMonoBehaviour
{
    public MaleficusButton MaleficusButton      { get { return GetComponent<MaleficusButton>(); } }
    public int RowIndex                         { get { return rowIndex; } }
    public int ColumnIndex                      { get { return columnIndex; } }

    [Header("Spell Selection Button")]

    [SerializeField] private bool disableDebugTextOnStart = true;
    [SerializeField] private AbstractSpell spell;
    [SerializeField] private int rowIndex;
    [SerializeField] private int columnIndex;

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


    public void HighlightPlayerSelection(EPlayerID playerID)
    {
        if (playerID != EPlayerID.NONE)
        {
            if (IS_KEY_CONTAINED(playerHighlights, playerID))
            {
                playerHighlights[playerID].ShowHighlight();
            }
        }
    }

    public void UnHighlightPlayerSelection(EPlayerID playerID)
    {
        if (playerID != EPlayerID.NONE)
        {
            if (IS_KEY_CONTAINED(playerHighlights, playerID))
            {
                playerHighlights[playerID].HideHighlight();
            }
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        // Reinitialize debug text from object name
        myDebugIndexText = GetComponentInChildren<Text>();
        if (myDebugIndexText != null)
        {
            myDebugIndexText.text = RowIndex + "-" + ColumnIndex;

            // Update the name of the gameobject accordingly
            name = "B_SpellSelectionButton " + RowIndex + "-" + ColumnIndex;
        }

        // Reinitialize button image 
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            if (spell != null)
            {
                myButton.image.sprite = spell.SpellIcon;

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


}
