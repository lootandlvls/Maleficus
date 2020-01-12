using System.Collections.Generic;
using UnityEngine;

public class GameManager : AbstractSingletonManager<GameManager>
{
    public AbstractGameMode CurrentGameMode { get; private set; }
    public EGameMode ChosenGameModeType { get; private set; }

    protected override void Start()
    {
        base.Start();

        if (MotherOfManagers.Instance.IsUseDebugGameMode == true)
        {
            ChosenGameModeType = MotherOfManagers.Instance.DebugGameMode;
            if (SpawnChosenGameMode() == false)
            {
                LogConsoleError("Was not able to spawn GameMode for : " + ChosenGameModeType);
            }
        }
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed;
        EventManager.Instance.NETWORK_GameStarted.AddListener(On_NETWORK_GameStarted);

        EventManager.Instance.GAME_GameOver.AddListener(ON_GAME_GameOver); // TODO : Not clean! use public OnGameOver method instead
    }

    protected override void OnReinitializeManager()
    {
        base.OnReinitializeManager();

        FindAndBindButtonActions();
    }

    #region Game Actions

    /// <summary>
    /// Returns true if succeeded spawning a valid GameMode, otherwise false
    /// </summary>
    private bool SpawnChosenGameMode()
    {
        switch (ChosenGameModeType)
        {
            case EGameMode.FFA_LIVES:
                CurrentGameMode = gameObject.AddComponent<GM_FFA_Lives>();
                break; 

            case EGameMode.FFA_TIME:

                break;

            case EGameMode.INSANE:

                break;

            case EGameMode.DUNGEON:
                CurrentGameMode = gameObject.AddComponent<GM_Single_Dungeon>();          
                break;
        }
        if ((IS_NOT_NULL(CurrentGameMode))
           && (ARE_EQUAL(ChosenGameModeType, CurrentGameMode.GameModeType)))
        {
            LogConsole("Spawned : " + ChosenGameModeType);
            return true;
        }
        return false;
    }


    private void PauseOrUnpauseGame()
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            EventManager.Instance.Invoke_GAME_GamePaused(CurrentGameMode);
        }
        else if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_PAUSED)
        {
            EventManager.Instance.Invoke_GAME_GameUnPaused(CurrentGameMode);
        }
    }

    private void EndGame(bool wasAborted = false)
    {
        // Invoke event
        EventManager.Instance.Invoke_GAME_GameEnded(CurrentGameMode, wasAborted);
    }

    #endregion


    #region Event Callbacks 
    //public void OnGameOver<T>(AbstractGameMode<T> gameMode, ETeamID teamID) where T: AbstractPlayerStats
    //{


    //    NetEvent_GameOver gameOverEventHandle = new NetEvent_GameOver(EClientID.SERVER, teamID);
    //    EventManager.Instance.GAME_GameOver.Invoke(gameOverEventHandle, EEventInvocationType.TO_ALL);
    //}

    private void On_INPUT_ButtonPressed(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = Maleficus.MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);
        if ((AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_NOT_STARTED)
            && (eventHandle.InputButton == EInputButton.CONFIRM)
            && (PlayerManager.Instance.HasPlayerJoined(playerID) == true))
        {
            EventManager.Instance.NETWORK_GameStarted.Invoke(new NetEvent_GameStarted(eventHandle.SenderID));
        }

    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted eventHandle)
    {
        if ((IS_NOT_NULL(CurrentGameMode))
            && (ARE_EQUAL(ChosenGameModeType, CurrentGameMode.GameModeType)))
        {
            EventManager.Instance.Invoke_GAME_GameStarted(CurrentGameMode);
        }
    }

    private void ON_GAME_GameOver(NetEvent_GameOver eventHandle)
    {
        if (ARE_EQUAL(AppStateManager.Instance.CurrentState, EAppState.IN_GAME_IN_RUNNING))
        {
            EndGame();
        }
    }


    #endregion

    private void FindAndBindButtonActions()
    {
        /* In GAME */
        //StartTestGameAction[] startTestGameActions = FindObjectsOfType<StartTestGameAction>();
        //foreach (StartTestGameAction action in startTestGameActions)
        //{
        //    action.ActionButtonPressed += () =>
        //    {
        //        StartGame(EGameMode.FFA_LIVES);
        //    };
        //}

        //StartDungeonSingleGame[] startDungeonSingleGameActions = FindObjectsOfType<StartDungeonSingleGame>();
        //foreach (StartDungeonSingleGame action in startDungeonSingleGameActions)
        //{
        //    action.ActionButtonPressed += () =>
        //    {
        //        StartGame(EGameMode.DUNGEON);
        //    };
        //}

        //StartGameAction[] startGameActions = FindObjectsOfType<StartGameAction>();
        //foreach (StartGameAction action in startGameActions)
        //{
        //    action.ActionButtonPressed += () =>
        //    {
        //       // StartGame(EGameMode.FFA_LIVES);
        //    };
        //}



        PauseGameUIAction[] pauseGameActions = FindObjectsOfType<PauseGameUIAction>();
        foreach (PauseGameUIAction action in pauseGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                PauseOrUnpauseGame();
            };
        }

        AbortGameUIAction[] abortGameActions = FindObjectsOfType<AbortGameUIAction>();
        foreach (AbortGameUIAction action in abortGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                EndGame(true);
            };
        }
    }


}
