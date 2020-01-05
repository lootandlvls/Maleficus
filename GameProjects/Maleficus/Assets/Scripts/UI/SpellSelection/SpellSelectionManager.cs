using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpellSelectionManager : AbstractSingletonManager<SpellSelectionManager>
{
    // TODO : defining every player on a corner 
    private Dictionary<EPlayerID, SpellSelectionButton> highlightedSpellButtons= new Dictionary<EPlayerID, SpellSelectionButton>();

    private Dictionary<int, Dictionary<int, SpellSelectionButton>> allSpellSelectionButtons = new Dictionary<int, Dictionary<int, SpellSelectionButton>>();

    public override void OnSceneStartReinitialize()
    {
        
    }


    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        InitializeSpellButtonsDictionaries();


    }

    public void PopulateSpellButtonsNavigation()
    {
        LogConsole("Populate spell buttons navigation");

        InitializeSpellButtonsDictionaries();

        // Set up buttons navigation
        int rowCount = allSpellSelectionButtons.Count;
        for (int i = 0; i < rowCount; i++)
        {
            if (IS_KEY_CONTAINED(allSpellSelectionButtons, i))
            {
                int columnCount = allSpellSelectionButtons[i].Count;
                for (int j = 0; j < columnCount; j++)
                {
                    if (IS_KEY_CONTAINED(allSpellSelectionButtons[i], j))
                    {
                        MaleficusButton currentButton = allSpellSelectionButtons[i][j].MaleficusButton;
                        currentButton.UnPopulateNavigationButtons();
                        
                        /* Upper + Bottom buttons */
                        // First row
                        if (i == 0)
                        {
                            // Get upper button if exist : from last row
                            if ((allSpellSelectionButtons.ContainsKey(rowCount - 1))
                                && (allSpellSelectionButtons[rowCount - 1].ContainsKey(j)))
                            {
                                currentButton.UpperButton = allSpellSelectionButtons[rowCount - 1][j].MaleficusButton;
                            }

                            // Get bottom button if exist
                            if ((allSpellSelectionButtons.ContainsKey(i + 1))
                                  && (allSpellSelectionButtons[i + 1].ContainsKey(j)))
                            {
                                currentButton.BottomButton = allSpellSelectionButtons[i + 1][j].MaleficusButton;
                            }
                        }
                        // Last row
                        else if (i == rowCount - 1)
                        {
                            // Get upper button if exist 
                            if ((allSpellSelectionButtons.ContainsKey(i - 1))
                                && (allSpellSelectionButtons[i - 1].ContainsKey(j)))
                            {
                                currentButton.UpperButton = allSpellSelectionButtons[i - 1][j].MaleficusButton;
                            }

                            // Get bottom button if exist : from first row
                            if ((allSpellSelectionButtons.ContainsKey(0))
                                  && (allSpellSelectionButtons[0].ContainsKey(j)))
                            {
                                currentButton.BottomButton = allSpellSelectionButtons[0][j].MaleficusButton;
                            }
                        }
                        // Row in the middle
                        else
                        {
                            // Get upper button if exist
                            if ((allSpellSelectionButtons.ContainsKey(i - 1))
                                && (allSpellSelectionButtons[i - 1].ContainsKey(j)))
                            {
                                currentButton.UpperButton = allSpellSelectionButtons[i - 1][j].MaleficusButton;
                            }

                            // Get bottom button if exist
                            if ((allSpellSelectionButtons.ContainsKey(i + 1))
                                  && (allSpellSelectionButtons[i + 1].ContainsKey(j)))
                            {
                                currentButton.BottomButton = allSpellSelectionButtons[i + 1][j].MaleficusButton;
                            }
                        }

                        /* Left + Right buttons */
                        // First element in the row
                        if (j == 0)
                        {
                            // Get left button : last element on the row 
                            if (allSpellSelectionButtons[i].ContainsKey(columnCount - 1))
                            {
                                currentButton.LeftButton = allSpellSelectionButtons[i][columnCount - 1].MaleficusButton;
                            }

                            // Get right button
                            if (allSpellSelectionButtons[i].ContainsKey(j + 1))
                            {
                                currentButton.RightButton = allSpellSelectionButtons[i][j + 1].MaleficusButton;
                            }
                        }
                        // Last element in the row
                        else if (j == columnCount - 1)
                        {
                            // Get left button 
                            if (allSpellSelectionButtons[i].ContainsKey(j - 1))
                            {
                                currentButton.LeftButton = allSpellSelectionButtons[i][j - 1].MaleficusButton;
                            }

                            // Get right button : first element on the row 
                            if (allSpellSelectionButtons[i].ContainsKey(0))
                            {
                                currentButton.RightButton = allSpellSelectionButtons[i][0].MaleficusButton;
                            }
                        }
                        // Element in the middle of the row
                        else
                        {
                            // Get left button 
                            if (allSpellSelectionButtons[i].ContainsKey(j - 1))
                            {
                                currentButton.LeftButton = allSpellSelectionButtons[i][j - 1].MaleficusButton;
                            }

                            // Get right button
                            if (allSpellSelectionButtons[i].ContainsKey(j + 1))
                            {
                                currentButton.RightButton = allSpellSelectionButtons[i][j + 1].MaleficusButton;
                            }
                        }
                    }
                }
            }
        }
    }

    private void InitializeSpellButtonsDictionaries()
    {
        // Find all Spell Selection Buttons in the scene
        foreach (SpellSelectionButton spellSelectionButton in FindObjectsOfType<SpellSelectionButton>())
        {
            int rowIndex = spellSelectionButton.RowIndex;
            int columnIndex = spellSelectionButton.ColumnIndex;

            // Add row in the dictionary if not already there
            if (allSpellSelectionButtons.ContainsKey(rowIndex) == false)
            {
                allSpellSelectionButtons.Add(rowIndex, new Dictionary<int, SpellSelectionButton>());
            }

            if (IS_KEY_NOT_CONTAINED(allSpellSelectionButtons[rowIndex], columnIndex))
            {
                allSpellSelectionButtons[rowIndex].Add(columnIndex, spellSelectionButton);
            }
        }
    }
}

 [CustomEditor(typeof(SpellSelectionManager))]
public class SpellSelectionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpellSelectionManager spellSelectionManager = (SpellSelectionManager)target;

        GUILayout.Space(20);

        if (GUILayout.Button("Populate Spell Butons Navigation"))
        {
            spellSelectionManager.PopulateSpellButtonsNavigation();
        }
    }

}