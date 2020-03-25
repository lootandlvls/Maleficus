using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

using static Maleficus.Utils;

[Serializable]
public struct SPlayerSpellButtonStartHighlight
{
    public EPlayerID PlayerID;
    public int RowID;
    public int ColumnID;

}

public class SpellSelectionManager : AbstractSingletonManager<SpellSelectionManager>
{
    public event Action<EPlayerID, AbstractSpell> SpellButtonHighlighted;

    [SerializeField] private SPlayerSpellButtonStartHighlight[] playerSpellButtonStartHighlights = new SPlayerSpellButtonStartHighlight[0];

    private Dictionary<EPlayerID, SpellSelectionButton> highlightedSpellButtons= new Dictionary<EPlayerID, SpellSelectionButton>();
    private Dictionary<int, Dictionary<int, SpellSelectionButton>> allSpellSelectionButtons = new Dictionary<int, Dictionary<int, SpellSelectionButton>>();

    public SpellSelectionButton GetHighlightedSpellButton(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(highlightedSpellButtons, playerID))
        {
            return highlightedSpellButtons[playerID];
        }
        return null;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed_Event;
        EventManager.Instance.PLAYERS_PlayerJoined      += On_PLAYERS_PlayerJoined;
        EventManager.Instance.PLAYERS_PlayerLeft        += On_PLAYERS_PlayerLeft;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.INPUT_ButtonPressed.Event -= On_INPUT_ButtonPressed_Event;
            EventManager.Instance.PLAYERS_PlayerJoined      -= On_PLAYERS_PlayerJoined;
            EventManager.Instance.PLAYERS_PlayerLeft        -= On_PLAYERS_PlayerLeft;
        }
    }


    private void On_PLAYERS_PlayerJoined(EPlayerID playerID, EControllerID controllerID)
    {
        if ((IS_KEY_CONTAINED(highlightedSpellButtons, playerID))
            && (IS_NOT_NULL(highlightedSpellButtons[playerID])))
        {
            highlightedSpellButtons[playerID].HighlightPlayerSelection(playerID);
        }
    }

    private void On_PLAYERS_PlayerLeft(EPlayerID playerID)
    {
        if ((IS_KEY_CONTAINED(highlightedSpellButtons, playerID))
            && (IS_NOT_NULL(highlightedSpellButtons[playerID])))
        {
            highlightedSpellButtons[playerID].UnHighlightPlayerSelection(playerID);
        }
    }

    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        InitializeSpellButtonsDictionaries();
        InitializeStartPlayerHighlights();

    }

    private void On_INPUT_ButtonPressed_Event(NetEvent_ButtonPressed eventHandle)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
        {
            EInputButton inputButton = eventHandle.InputButton;
            EControllerID controllerID = eventHandle.ControllerID;
            EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);

            if (PlayerManager.Instance.HasPlayerJoined(playerID))
            {
                if (IS_KEY_CONTAINED(highlightedSpellButtons, playerID))
                {
                    SpellSelectionButton currentSpellButton = highlightedSpellButtons[playerID];

                    switch (inputButton)
                    {
                        case EInputButton.UP:

                            BNJMOButton upperMaleficusButton = currentSpellButton.Button.UpperButton;
                            if (IS_NOT_NULL(upperMaleficusButton))
                            {
                                currentSpellButton.UnHighlightPlayerSelection(playerID);
                                currentSpellButton = upperMaleficusButton.GetComponentWithCheck<SpellSelectionButton>();
                                currentSpellButton.HighlightPlayerSelection(playerID);
                                highlightedSpellButtons[playerID] = currentSpellButton;
                                InvokeEventIfBound(SpellButtonHighlighted, playerID, currentSpellButton.Spell);
                            }
                            break;

                        case EInputButton.DOWN:
                            BNJMOButton bottomMaleficusButton = currentSpellButton.Button.BottomButton;
                            if (IS_NOT_NULL(bottomMaleficusButton))
                            {
                                currentSpellButton.UnHighlightPlayerSelection(playerID);
                                currentSpellButton = bottomMaleficusButton.GetComponentWithCheck<SpellSelectionButton>();
                                currentSpellButton.HighlightPlayerSelection(playerID);
                                highlightedSpellButtons[playerID] = currentSpellButton;
                                InvokeEventIfBound(SpellButtonHighlighted, playerID, currentSpellButton.Spell);
                            }
                            break;

                        case EInputButton.LEFT:
                            BNJMOButton leftMaleficusButton = currentSpellButton.Button.LeftButton;
                            if (IS_NOT_NULL(leftMaleficusButton))
                            {
                                currentSpellButton.UnHighlightPlayerSelection(playerID);
                                currentSpellButton = leftMaleficusButton.GetComponentWithCheck<SpellSelectionButton>();
                                currentSpellButton.HighlightPlayerSelection(playerID);
                                highlightedSpellButtons[playerID] = currentSpellButton;
                                InvokeEventIfBound(SpellButtonHighlighted, playerID, currentSpellButton.Spell);
                            }
                            break;

                        case EInputButton.RIGHT:
                            BNJMOButton rightMaleficusButton = currentSpellButton.Button.RightButton;
                            if (IS_NOT_NULL(rightMaleficusButton))
                            {
                                currentSpellButton.UnHighlightPlayerSelection(playerID);
                                currentSpellButton = rightMaleficusButton.GetComponentWithCheck<SpellSelectionButton>();
                                currentSpellButton.HighlightPlayerSelection(playerID);
                                highlightedSpellButtons[playerID] = currentSpellButton;
                                InvokeEventIfBound(SpellButtonHighlighted, playerID, currentSpellButton.Spell);
                            }
                            break;

                    }
                }
            }
        }
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
                    if (allSpellSelectionButtons[i].ContainsKey(j))
                    {
                        BNJMOButton currentButton = allSpellSelectionButtons[i][j].Button;
                        currentButton.UnPopulateNavigationButtons();
                        
                        /* Upper + Bottom buttons */
                        // First row
                        if (i == 0)
                        {
                            // Get upper button if exist : from last row
                            if ((allSpellSelectionButtons.ContainsKey(rowCount - 1))
                                && (allSpellSelectionButtons[rowCount - 1].ContainsKey(j)))
                            {
                                currentButton.UpperButton = allSpellSelectionButtons[rowCount - 1][j].Button;
                            }

                            // Get bottom button if exist
                            if ((allSpellSelectionButtons.ContainsKey(i + 1))
                                  && (allSpellSelectionButtons[i + 1].ContainsKey(j)))
                            {
                                currentButton.BottomButton = allSpellSelectionButtons[i + 1][j].Button;
                            }
                        }
                        // Last row
                        else if (i == rowCount - 1)
                        {
                            // Get upper button if exist 
                            if ((allSpellSelectionButtons.ContainsKey(i - 1))
                                && (allSpellSelectionButtons[i - 1].ContainsKey(j)))
                            {
                                currentButton.UpperButton = allSpellSelectionButtons[i - 1][j].Button;
                            }

                            // Get bottom button if exist : from first row
                            if ((allSpellSelectionButtons.ContainsKey(0))
                                  && (allSpellSelectionButtons[0].ContainsKey(j)))
                            {
                                currentButton.BottomButton = allSpellSelectionButtons[0][j].Button;
                            }
                        }
                        // Row in the middle
                        else
                        {
                            // Get upper button if exist
                            if ((allSpellSelectionButtons.ContainsKey(i - 1))
                                && (allSpellSelectionButtons[i - 1].ContainsKey(j)))
                            {
                                currentButton.UpperButton = allSpellSelectionButtons[i - 1][j].Button;
                            }

                            // Get bottom button if exist
                            if ((allSpellSelectionButtons.ContainsKey(i + 1))
                                  && (allSpellSelectionButtons[i + 1].ContainsKey(j)))
                            {
                                currentButton.BottomButton = allSpellSelectionButtons[i + 1][j].Button;
                            }
                        }

                        /* Left + Right buttons */
                        // First element in the row
                        if (j == 0)
                        {
                            // Get left button : last element on the row 
                            if (allSpellSelectionButtons[i].ContainsKey(columnCount - 1))
                            {
                                currentButton.LeftButton = allSpellSelectionButtons[i][columnCount - 1].Button;
                            }

                            // Get right button
                            if (allSpellSelectionButtons[i].ContainsKey(j + 1))
                            {
                                currentButton.RightButton = allSpellSelectionButtons[i][j + 1].Button;
                            }
                        }
                        // Last element in the row
                        else if (j == columnCount - 1)
                        {
                            // Get left button 
                            if (allSpellSelectionButtons[i].ContainsKey(j - 1))
                            {
                                currentButton.LeftButton = allSpellSelectionButtons[i][j - 1].Button;
                            }

                            // Get right button : first element on the row 
                            if (allSpellSelectionButtons[i].ContainsKey(0))
                            {
                                currentButton.RightButton = allSpellSelectionButtons[i][0].Button;
                            }
                        }
                        // Element in the middle of the row
                        else
                        {
                            // Get left button 
                            if (allSpellSelectionButtons[i].ContainsKey(j - 1))
                            {
                                currentButton.LeftButton = allSpellSelectionButtons[i][j - 1].Button;
                            }

                            // Get right button
                            if (allSpellSelectionButtons[i].ContainsKey(j + 1))
                            {
                                currentButton.RightButton = allSpellSelectionButtons[i][j + 1].Button;
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
        foreach (SpellSelectionButton spellSelectionButton in GetComponentsInChildren<SpellSelectionButton>())
        {
            int rowIndex = spellSelectionButton.RowIndex - 1;
            int columnIndex = spellSelectionButton.ColumnIndex - 1;

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

        foreach (SPlayerSpellButtonStartHighlight playerHighlight in playerSpellButtonStartHighlights)
        {
            InitializeStartPlayerHighlight(playerHighlight.PlayerID, playerHighlight.RowID - 1, playerHighlight.ColumnID - 1);
        }

        //// Upper left corner
        //InitializeStartPlayerHighlight(EPlayerID.PLAYER_1, 0, 0);

        //// Upper right corner
        //InitializeStartPlayerHighlight(EPlayerID.PLAYER_2, 0, allSpellSelectionButtons[0].Count - 1);

        //// Lower left corner
        //InitializeStartPlayerHighlight(EPlayerID.PLAYER_3, allSpellSelectionButtons.Count - 1, 0);

        //// Lower right corner
        //InitializeStartPlayerHighlight(EPlayerID.PLAYER_4, allSpellSelectionButtons.Count - 1, allSpellSelectionButtons[allSpellSelectionButtons.Count - 1].Count - 1);
    }

    private void InitializeStartPlayerHighlight(EPlayerID playerID, int rowIndex, int columnIndex)
    {
        if ((IS_KEY_CONTAINED(allSpellSelectionButtons, rowIndex))
            && (IS_KEY_CONTAINED(allSpellSelectionButtons[rowIndex], columnIndex)))
        {
            SpellSelectionButton player1Button = allSpellSelectionButtons[rowIndex][columnIndex];
            highlightedSpellButtons.Add(playerID, player1Button);
        }
    }
}

#if UNITY_EDITOR
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
#endif