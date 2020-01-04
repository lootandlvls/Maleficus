using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSelectionManager : AbstractSingletonManager<SpellSelectionManager>
{
    private Dictionary<EPlayerID, SpellSelectionButton> highlightedSpellButtons= new Dictionary<EPlayerID, SpellSelectionButton>();

    private Dictionary<int, Dictionary<int, SpellSelectionButton>> allSpellSelectionButtons = new Dictionary<int, Dictionary<int, SpellSelectionButton>>();

    private int numberOfRows = 0;


    public override void OnSceneStartReinitialize()
    {
        
    }


    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        // Find all Spell Selection Buttons in the scene
        foreach (var spellSelectionButton in FindObjectsOfType<SpellSelectionButton>())
        {
            int rowIndex    = spellSelectionButton.RowIndex;
            int columnIndex = spellSelectionButton.ColumnIndex;

            // Add row in the dictionary if not already there
            if (allSpellSelectionButtons.ContainsKey(rowIndex) == false)
            {
                allSpellSelectionButtons.Add(rowIndex, new Dictionary<int, SpellSelectionButton>());
            }

            if (IS_KEY_NOT_CONTAINED(allSpellSelectionButtons[rowIndex], columnIndex))
            {
                allSpellSelectionButtons[rowIndex].Add(rowIndex, spellSelectionButton);
            }
        }

        Debug.Log(allSpellSelectionButtons.Count + " spell rows found");

    }
}
