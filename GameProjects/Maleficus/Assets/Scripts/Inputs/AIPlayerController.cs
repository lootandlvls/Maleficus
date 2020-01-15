using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static Maleficus.Utils;

public class AIPlayerController : BNJMOBehaviour
{
    public event Action<EControllerID, EJoystickType, float, float> MoveJoystick;
    public event Action<AIPlayerController> WillGetDestroyed;

    public EControllerID controllerID   { get; private set; } = EControllerID.NONE;
    public EPlayerID    playerID        { get; private set; } = EPlayerID.NONE;
    public float RotationSpeed          { get; set; } = 3.0f;
    public float SafeRadius             { get; set; } = 0.3f;
    public float DangerRadius           { get; set; } = 0.65f;
    private Player myPlayer;
    private Arena currentArena;

    private Vector2 currentRotation;
    private Vector2 currentInArenaPosition;
    private EAIArenaZone currentArenaZone;
    private float currentArenaInRadius;
    private Player currentClosestPlayer;
    private Vector2 currentToCenterDirection;
    private Vector2 currentToClosestPlayerDirection;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myPlayer = GetComponentWithCheck<Player>();
    }

    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        currentArena = FindObjectOfType<Arena>();
        IS_NOT_NULL(currentArena);
    }

    protected override void Start()
    {
        base.Start();

        playerID = myPlayer.PlayerID;
        controllerID = InputManager.Instance.GetConnectedControllerIDFrom(playerID);
        IS_NOT_NONE(playerID);
        IS_NOT_NONE(controllerID);
    }

    protected override void Update()
    {
        base.Update();

        if (IS_NOT_NULL(currentArena))
        {
            // Arena 
            currentInArenaPosition = currentArena.GetInArenaPosition(myPlayer);
            currentToCenterDirection = -(currentInArenaPosition.normalized);
            currentArenaInRadius = currentInArenaPosition.magnitude;
            UpdateArenaZone();

            // Closest player
            currentClosestPlayer = GetClosestPlayerTo();
            currentToClosestPlayerDirection = Get2DVector((currentClosestPlayer.Position - myPlayer.Position).normalized);

            // Debug
            DebugDrawArrow(myPlayer.Position, Get3DVector(currentToCenterDirection) * 2.0f, Color.green);
            LogCanvas(6, playerID + " - arena pos : " + currentArenaInRadius + " - " + currentArenaZone);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IS_NOT_NULL(myPlayer))
        {
            // Rotate
            if (currentClosestPlayer)
            {
                currentRotation = Vector2.Lerp(currentRotation, currentToClosestPlayerDirection, Time.deltaTime * RotationSpeed);
                InvokeEventIfBound(MoveJoystick, controllerID, EJoystickType.ROTATION, currentRotation.x, -currentRotation.y);
            }

            // Move
            switch (currentArenaZone)
            {
                case EAIArenaZone.SAFE:

                    break;

                case EAIArenaZone.NORMAL:

                    break;

                case EAIArenaZone.DANGER:

                    break;
            }
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        InvokeEventIfBound(WillGetDestroyed, this);

    }


    private void UpdateArenaZone()
    {        
        if (currentArenaInRadius < SafeRadius)
        {
            currentArenaZone = EAIArenaZone.SAFE;
        }
        else if ((currentArenaInRadius >= SafeRadius) && (currentArenaInRadius < DangerRadius))
        {
            currentArenaZone = EAIArenaZone.NORMAL;
        }
        else
        {
            currentArenaZone = EAIArenaZone.DANGER;
        }
    }

    private Player GetClosestPlayerTo()
    {
        Player closestOtherPlayer = null;
        float closestDistanceToOtherPlayer = float.MaxValue;
        foreach (Player otherPlayer in PlayerManager.Instance.ActivePlayers.Values)
        {
            if (otherPlayer.PlayerID == playerID)
            {
                continue;
            }

            float distanceToOtherPlayer = Vector3.Distance(myPlayer.Position, otherPlayer.Position);
            if (distanceToOtherPlayer < closestDistanceToOtherPlayer)
            {
                closestOtherPlayer = otherPlayer;
                closestDistanceToOtherPlayer = distanceToOtherPlayer;
            }
        }
        return closestOtherPlayer;
    }

    private enum EAIArenaZone
    {
        SAFE,
        NORMAL,
        DANGER
    }
}
