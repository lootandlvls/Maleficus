using System.Collections.Generic;
using UnityEngine;

using static Maleficus.Consts;
using static Maleficus.Utils;

public class AIInputSource : AbstractInputSource
{
    [Header("AI Initialization")]
    [SerializeField] float rotationSpeed = 3.0f;

    private List<AIPlayerController> activeAIControllers = new List<AIPlayerController>();

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.PLAYERS_PlayerSpawned += On_PLAYERS_PlayerSpawned;
    }

    private void On_PLAYERS_PlayerSpawned(EPlayerID playerID)
    {
        // Add an AIPlayerController on the spawned player if he's an AI
        EControllerID controllerID = InputManager.Instance.GetConnectedControllerIDFrom(playerID);
        
        if ((controllerID.ContainedIn(AI_CONTROLLERS))
            && (IS_KEY_CONTAINED(PlayerManager.Instance.ActivePlayers, playerID))
            && (IS_NOT_NULL(PlayerManager.Instance.ActivePlayers[playerID])))
        {
            Player player = PlayerManager.Instance.ActivePlayers[playerID];
            AIPlayerController aIPlayerController = player.gameObject.AddComponent<AIPlayerController>();
            aIPlayerController.MoveJoystick += On_AIPlayerController_MoveJoystick;
            aIPlayerController.WillGetDestroyed += On_AIPlayerController_WillGetDestroyed;
            aIPlayerController.RotationSpeed = rotationSpeed;
            activeAIControllers.Add(aIPlayerController);
        }
    }

    private void On_AIPlayerController_MoveJoystick(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        InvokeJoystickMoved(controllerID, joystickType, x, y);
    }

    private void On_AIPlayerController_WillGetDestroyed(AIPlayerController aIPlayerController)
    {
        if (aIPlayerController)
        {
            aIPlayerController.MoveJoystick -= On_AIPlayerController_MoveJoystick;
            aIPlayerController.WillGetDestroyed -= On_AIPlayerController_WillGetDestroyed;
            if (IS_VALUE_CONTAINED(activeAIControllers, aIPlayerController))
            {
                activeAIControllers.Remove(aIPlayerController);
            }
        }
    }
}
