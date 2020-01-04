using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpellSelectionUIAction))]
public class SpellSelectionButton : MaleficusMonoBehaviour
{
    public int RowIndex         { get { return rowIndex; } }
    public int ColumnIndex      { get { return columnIndex; } }

    [Header("Spell Selection Button")]

    [Tooltip("Leave NONE if doesn't need to be highlighted")]
    [SerializeField] private EPlayerID highlightOnStartForPlayer = EPlayerID.NONE;

    [SerializeField] private bool disableDebugTextOnStart = true;
    [SerializeField] private AbstractSpell spell;
    [SerializeField] private int rowIndex;
    [SerializeField] private int columnIndex;


    private Button myButton;
    private Text myDebugIndexText;

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
        myDebugIndexText = GetComponentInChildren<Text>();
        if (myDebugIndexText != null)
        {
            myDebugIndexText.text = RowIndex + "-" + ColumnIndex;

            // Update the name of the gameobject accordingly
            name = "B_SpellSelectionButton " + RowIndex + " - " + ColumnIndex;
        }

        // Reinitialize button image 
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            if (spell != null)
            {
                myButton.image.sprite = spell.SpellIcon;

                // Update the name of the gameobject accordingly
                name = "B_SpellSelectionButton " + RowIndex + " - " + ColumnIndex + " : " + spell.SpellName;
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
    }


}
