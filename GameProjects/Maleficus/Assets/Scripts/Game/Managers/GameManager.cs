
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : AbstractSingletonManager<GameManager>
{
    public AbstractGameMode CurrentGameMode { get; private set; }
    public EGameMode ChosenGameModeType { get; private set; }
    public int GameRemainingTime { get; private set; }


    protected override void Start()
    {
        base.Start();

        if (MotherOfManagers.Instance.IsUseDebugGameMode == true)
        {
            ChosenGameModeType = MotherOfManagers.Instance.DebugGameMode;
        }
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed;
        EventManager.Instance.NETWORK_GameStarted.AddListener(On_NETWORK_GameStarted);
        EventManager.Instance.GAME_GameTimeUpdated += On_GAME_GameTimeUpdated;
        EventManager.Instance.GAME_GameOver.AddListener(ON_GAME_GameOver); // TODO : Not clean! use public OnGameOver method instead

        EventManager.Instance.APP_SceneChanged.Event += On_APP_SceneChanged;
    }

    private void On_APP_SceneChanged(Event_GenericHandle<EScene> eventHandle)
    {
        EScene scene = eventHandle.Arg1;
        switch (scene)
        {
            case EScene.MENU:
                if (CurrentGameMode != null)
                {
                    Destroy(CurrentGameMode);
                }
                break;

            case EScene.GAME:
                SpawnChosenGameMode();
                break;
        }
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
           && (ARE_ENUMS_EQUAL(ChosenGameModeType, CurrentGameMode.GameModeType)))
        {
            LogConsole("Spawned : " + ChosenGameModeType);
            return true;
        }
        LogConsole("Couldn't spawn : " + ChosenGameModeType);
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
        EPlayerID playerID = Maleficus.Utils.GetPlayerIDFrom(eventHandle.SenderID);
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
            && (ARE_ENUMS_EQUAL(ChosenGameModeType, CurrentGameMode.GameModeType)))
        {
            EventManager.Instance.Invoke_GAME_GameStarted(CurrentGameMode);
        }
    }

    private void On_GAME_GameTimeUpdated(int newTime)
    {
        GameRemainingTime = newTime;
        if (newTime == 0)
        {
            EndGame();
        }
    }

    private void ON_GAME_GameOver(NetEvent_GameOver eventHandle)
    {
        if (ARE_ENUMS_EQUAL(AppStateManager.Instance.CurrentState, EAppState.IN_GAME_IN_RUNNING))
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
            action.ActionButtonExecuted += () =>
            {
                PauseOrUnpauseGame();
            };
        }

        AbortGameUIAction[] abortGameActions = FindObjectsOfType<AbortGameUIAction>();
        foreach (AbortGameUIAction action in abortGameActions)
        {
            action.ActionButtonExecuted += () =>
            {
                EndGame(true);
            };
        }
    }

  

}
