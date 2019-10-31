using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Responsible for managing input controller sources attached to the same gameobject
/// </summary>
public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode                                   InputMode                               { get { return MotherOfManagers.Instance.InputMode; } }

    /// <summary> Mapping from controllerID to playerID </summary> 
    public Dictionary<EControllerID, EPlayerID>         ConnectedControllers                    { get { return connectedControllers; } }


    private Dictionary<EControllerID, EPlayerID>        connectedControllers                    = new Dictionary<EControllerID, EPlayerID>();

    protected override void Awake()
    {
        base.Awake();

        AbstractInputSource[] inputSources = GetComponents<AbstractInputSource>();
        foreach (AbstractInputSource inputSource in inputSources)
        {
            inputSource.ButtonPressed   += On_InputSource_ButtonPressed;
            inputSource.ButtonReleased  += On_InputSource_ButtonReleased;
            inputSource.JoystickMoved   += On_InputSource_JoystickMoved;
        }
    }


    private void Start()
    {

        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener           (On_NETWORK_ReceivedGameSessionInfo);
        EventManager.Instance.GAME_GameOver.AddListener                             (On_GAME_GameOver);

        StartCoroutine(LateStartCoroutine());
    }

    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // Connect Touch player as first player
        if ((MotherOfManagers.Instance.InputMode == EInputMode.TOUCH)
            && (MotherOfManagers.Instance.IsSpawnTouchAsPlayer1 == true))
        {
            ConnectControllerToPlayer(EControllerID.TOUCH, EPlayerID.PLAYER_1);
        }

        // Connect all players
        if ((MotherOfManagers.Instance.IsSpawnAllPlayers == true)
            && (AppStateManager.Instance.CurrentScene.ContainedIn(MaleficusConsts.GAME_SCENES)))
        {
            ConnectControllerToPlayer(EControllerID.AI_1, EPlayerID.PLAYER_1);
            ConnectControllerToPlayer(EControllerID.AI_2, EPlayerID.PLAYER_2);
            ConnectControllerToPlayer(EControllerID.AI_3, EPlayerID.PLAYER_3);
            ConnectControllerToPlayer(EControllerID.AI_4, EPlayerID.PLAYER_4);
        }
    }

    public override void OnSceneStartReinitialize()
    {
        
    }

    private void Update()
    {

    }


    #region Input Source Callbacks
    private void On_InputSource_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = ConnectedControllers[controllerID];
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonPressed buttonPressed = new NetEvent_ButtonPressed(clientID, inputButton);
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.TO_SERVER_ONLY);
            }
        }
    }

    private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
                Debug.Log(controllerID + " released " + inputButton);
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = ConnectedControllers[controllerID];
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonReleased buttonReleased = new NetEvent_ButtonReleased(clientID, inputButton);
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.TO_SERVER_ONLY);
            }
        }
    }


    private void On_InputSource_JoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = ConnectedControllers[controllerID];
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            if (joystickType != EJoystickType.NONE)
            {
                NetEvent_JoystickMoved joystickMoved = new NetEvent_JoystickMoved(clientID, joystickType, x, y);
                EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.TO_SERVER_ONLY);
            }
        }
    }
    #endregion



    #region Server Input
    private void On_NETWORK_ReceivedGameSessionInfo(Event_GenericHandle<List<EPlayerID>, EPlayerID> eventHandle)
    {
        List<EPlayerID> connectedPlayers = eventHandle.Arg1;
        EPlayerID ownPlayer = eventHandle.Arg2;

        // Connect controllers
        foreach (EPlayerID playerID in connectedPlayers)
        {
            if (playerID == ownPlayer)
            {
                ConnectControllerToPlayer(EControllerID.TOUCH, playerID);
            }
            else
            {
                ConnectControllerToPlayer(MaleficusUtilities.GetControllerNeteworkID(playerID), playerID);
            }
        }
    }
    #endregion


    public void ConnectControllerToPlayer(EControllerID controllerID, EPlayerID playerID)
    {
        // Check parameters
        if ((ConnectedControllers.ContainsKey(controllerID) == true)
            || (ConnectedControllers.ContainsValue(playerID) == true))
        {
            if (MotherOfManagers.Instance.IsSpawnAllPlayers == false)
            {
                Debug.LogError("Trying to connect a controller that is already connected.");
            }
            return;
        }
        if ((playerID == EPlayerID.NONE)
            || (controllerID == EControllerID.NONE))
        {
            Debug.LogError("Trying to connect a controller or player that is NONE.");
            return;
        }

        ConnectedControllers.Add(controllerID, playerID);

        Debug.Log("Connecting new controller " + controllerID);

        // Invoke event
        Event_GenericHandle<EControllerID, EPlayerID> controllerConnected = new Event_GenericHandle<EControllerID, EPlayerID>(controllerID, playerID);
        EventManager.Instance.INPUT_ControllerConnected.Invoke(controllerConnected);
    }

    public void DisconnectController(EControllerID controllerID)
    {
        if (ConnectedControllers.ContainsKey(controllerID) == false)
        {
            Debug.LogError("Trying to disconnect a controller that is not connected.");
            return;
        }

        EPlayerID playerID = connectedControllers[controllerID];

        ConnectedControllers.Remove(controllerID);

        // Invoke event
        Event_GenericHandle<EControllerID, EPlayerID> controllerDisconnected = new Event_GenericHandle<EControllerID, EPlayerID>(controllerID, playerID);
        EventManager.Instance.INPUT_ControllerDisconnected.Invoke(controllerDisconnected);
    }


    public bool IsControllerConnected(EControllerID controllerID)
    {
        return ConnectedControllers.ContainsKey(controllerID);
    }

    private void On_GAME_GameOver(NetEvent_GameOver eventHandle)
    {
        // Disconnect all network controllers and AI
        List<EControllerID> controllerIDsToRemove = new List<EControllerID>();
        foreach (EControllerID controllerID in ConnectedControllers.Keys)
        {
            if (controllerID.ContainedIn(MaleficusConsts.NETWORK_CONTROLLERS)
                || (controllerID == EControllerID.AI_1))
            {
                controllerIDsToRemove.Add(controllerID);
            }
        }
        foreach(EControllerID controllerID in controllerIDsToRemove)
        {
            ConnectedControllers.Remove(controllerID);
        }
    }

    
}
