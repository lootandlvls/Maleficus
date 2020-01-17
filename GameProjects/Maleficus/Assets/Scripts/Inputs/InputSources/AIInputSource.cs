using System.Collections.Generic;
using UnityEngine;

using static Maleficus.Consts;
using static Maleficus.Utils;

public class AIInputSource : AbstractInputSource
{

    [Header("AI Initialization")]
    [SerializeField] public float MinMovementThreshold = 0.2f;
    [SerializeField] public float MinSmoothMovementThreshold = 0.06f;
    [SerializeField] public float RotationSmoothSpeed = 3.0f;
    [SerializeField] public float MovemeSmoothSpeed = 3.0f;
    [SerializeField] public float ArenaSafeRadius = 0.3f;
    [SerializeField] public float ArenaDangerRadius = 0.65f;

    [Header("AI Distance Thresholds")]
    [SerializeField] public float ClosestEnemyDangerArenaDistanceStartThreshold = 0.8f;
    [SerializeField] public float ClosestEnemyDangerArenaDistanceMaxThreshold = 0.4f;
    [SerializeField] public float ClosestSpellDangerArenaDistanceStartThreshold = 0.6f;
    [SerializeField] public float ClosestSpellDangerArenaDistanceMaxThreshold = 0.17f;

    [Header("AI Influence factors")]
    public float ArenaPositionInfluenceFactor = 1.0f;
    public float ClosestPlayerInfluenceFactor = 1.0f;
    public float ClosestSpellInfluenceFactor = 1.0f;
    public float PushInfluenceFactor = 1.0f;

    [Header("AI Influence Ear Curves")]
    public AnimationCurve ArenaPositionInfluenceCurve;
    public AnimationCurve ClosestPlayerInfluenceCurve;
    public AnimationCurve ClosestSpellInfluenceCurve;
    public AnimationCurve PushInfluenceCurve;

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
            aIPlayerController.MoveJoystick += On_AIPlayerController_JoystickMoved;
            aIPlayerController.ButtonPressed += On_AIPlayerController_ButtonPressed;
            aIPlayerController.ButtonReleased += On_AIPlayerController_ButtonReleased;
            aIPlayerController.WillGetDestroyed += On_AIPlayerController_WillGetDestroyed;
            aIPlayerController.InitializeAIController(this);
            activeAIControllers.Add(aIPlayerController);
        }
    }

    

    private void On_AIPlayerController_JoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        InvokeJoystickMoved(controllerID, joystickType, x, y);
    }


    private void On_AIPlayerController_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        InvokeButtonPressed(controllerID, inputButton);
    }

    private void On_AIPlayerController_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        InvokeButtonReleased(controllerID, inputButton);
    }

    private void On_AIPlayerController_WillGetDestroyed(AIPlayerController aIPlayerController)
    {
        if (aIPlayerController)
        {
            aIPlayerController.MoveJoystick -= On_AIPlayerController_JoystickMoved;
            aIPlayerController.WillGetDestroyed -= On_AIPlayerController_WillGetDestroyed;
            if (IS_VALUE_CONTAINED(activeAIControllers, aIPlayerController))
            {
                activeAIControllers.Remove(aIPlayerController);
            }
        }
    }
}
