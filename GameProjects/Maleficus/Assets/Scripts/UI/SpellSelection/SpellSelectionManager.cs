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
        InitializeStartPlayerHighlights();

        //SpellSelectionUIAction[] spellSelectionAction = FindObjectsOfType<SpellSelectionButton>();
        //foreach (SpellSelectionButton Action in spellSelectionAction)
        //{
        //    Action. += () =>
        //    {
        //        EventManager.Instance.Invoke_UI_SpellChosen(Action.Spell);
        //        Debug.Log("A BUTTON HAS BEEN PRESSED !!!!!!!!!!!!!!");
        //    };

        //}

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
        allSpellSelectionButtons = new Dictionary<int, Dictionary<int, SpellSelectionButton>>();

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

    private void InitializeStartPlayerHighlights()
    {

        highlightedSpellButtons = new Dictionary<EPlayerID, SpellSelectionButton>();

        // Upper left corner
        InitializeStartPlayerHighlight(EPlayerID.PLAYER_1, 0, 0);

        // Upper right corner
        InitializeStartPlayerHighlight(EPlayerID.PLAYER_2, 0, allSpellSelectionButtons[0].Count - 1);

        // Lower left corner
        InitializeStartPlayerHighlight(EPlayerID.PLAYER_3, allSpellSelectionButtons.Count - 1, 0);

        // Lower right corner
        InitializeStartPlayerHighlight(EPlayerID.PLAYER_4, allSpellSelectionButtons.Count - 1, allSpellSelectionButtons[allSpellSelectionButtons.Count - 1].Count - 1);
    }

    private void InitializeStartPlayerHighlight(EPlayerID playerID, int rowIndex, int columnIndex)
    {
        if ((IS_KEY_CONTAINED(allSpellSelectionButtons, rowIndex))
            && (IS_KEY_CONTAINED(allSpellSelectionButtons[rowIndex], columnIndex)))
        {
            SpellSelectionButton player1Button = allSpellSelectionButtons[rowIndex][columnIndex];
            highlightedSpellButtons.Add(playerID, player1Button);
            player1Button.HighlightPlayerSelection(playerID);
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