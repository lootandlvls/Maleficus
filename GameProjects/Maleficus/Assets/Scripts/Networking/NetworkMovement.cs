using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;


public class NetworkMovement : MaleficusMonoBehaviour
{
    private TrailRenderer myTrailRenderer;

    private List<NetEvent_JoystickMoved> notAcknowledgedMovementMessages = new List<NetEvent_JoystickMoved>();
    private EClientID myClientID = EClientID.NONE;
    private Vector3 initialPosition;
    private IEnumerator ReplicateMovementEnumerator;


    private Vector2 currentMovementInput = Vector2.zero;
    private int currentExecution = 0;

    private ServerMovementRepresentation serverMovementRepresentation;

    protected override void Awake()
    {
        base.Awake();

        myTrailRenderer = GetComponent<TrailRenderer>();

        Player myPlayer = GetComponent<Player>();
        if (myPlayer != null)
        {
            myClientID = GetClientIDFrom(myPlayer.PlayerID);
        }
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_JoystickMoved.AddListener(On_INPUT_JoystickMoved);
        EventManager.Instance.NETWORK_GameStateReplication.AddListener(On_NETWORK_GameStateReplicate);
    }

    protected override void Start()
    {
        base.Start();

        initialPosition = transform.position;

        SpawnServerVisualRepresentation();
    }

    private void SpawnServerVisualRepresentation()
    {
        GameObject spawnedObject = Instantiate(Resources.Load<GameObject>(PATH_PLAYER_SERVER_REPRESENTATION), transform.position, transform.rotation);
        serverMovementRepresentation = spawnedObject.GetComponent<ServerMovementRepresentation>();
        serverMovementRepresentation.transform.localScale = transform.localScale;
    }

    protected override void LateStart()
    {
        base.LateStart();

    }

    protected override void Update()
    {
        base.Update();

        DebugManager.Instance.Log(92, "Not Ack # : " + notAcknowledgedMovementMessages.Count);
        DebugManager.Instance.Log(93, "Current execution # : " + currentExecution);

        if (Input.GetKeyDown("r"))
        {
            StartNewCoroutine(ref ReplicateMovementEnumerator, ReplicateMovementCoroutine());
        }

    }

    protected override void FixedUpdate()
    {
        base.Update();

        if (NetworkManager.Instance.HasAuthority == true)
        {
            ServerUpdate();
        }
        else if (NetworkManager.Instance.OwnerClientID == myClientID)
        {
            OwnerClientUpdate();
        }
        else
        {
            OtherClientUpdate();
        }
    }

    private void ServerUpdate()
    {

    }
    private void OwnerClientUpdate()
    {
        Move(currentMovementInput.x, currentMovementInput.y, Time.fixedDeltaTime * 1000.0f);
        LookAtMovingDirection(currentMovementInput.x, currentMovementInput.y);
    }

    private void OtherClientUpdate()
    {

    }


    private void On_INPUT_JoystickMoved(NetEvent_JoystickMoved eventHandle)
    {
        if (eventHandle.JoystickType == EJoystickType.MOVEMENT)
        {
            currentMovementInput = new Vector2(eventHandle.Joystick_X, eventHandle.Joystick_Y);


            if (MotherOfManagers.Instance.InputMode == EInputMode.TOUCH)
            {
                notAcknowledgedMovementMessages.Add(eventHandle);
            }
        }

        if (eventHandle.JoystickType == EJoystickType.ROTATION)
        {
            RotateRelativeToGrandParentRotation(eventHandle.Joystick_X, -eventHandle.Joystick_Y);
        }

    }

    private void On_NETWORK_GameStateReplicate(NetEvent_GameStateReplication eventHandle)
    {
        if (eventHandle.UpdatedPlayerID == GetPlayerIDFrom(myClientID))
        {
            serverMovementRepresentation.Position = GetVectorFrom(eventHandle.playerPosition);

            List<NetEvent_JoystickMoved> newList = new List<NetEvent_JoystickMoved>();
            foreach (NetEvent_JoystickMoved notAcknowledgedMovementMessage in notAcknowledgedMovementMessages)
            {
                // Stored net message is more recenet than received game state
                if (notAcknowledgedMovementMessage.TimeStamp > eventHandle.TimeStamp)
                {
                    MoveServerRepresentation(notAcknowledgedMovementMessage.Joystick_X, notAcknowledgedMovementMessage.Joystick_Y, Time.fixedDeltaTime * 1000.0f);

                    newList.Add(notAcknowledgedMovementMessage);
                }
            }

            notAcknowledgedMovementMessages = newList;
        }
    }



    private void Move(float axis_X, float axis_Z, float remainingTime)
    {

        float fixedTimePercentage = remainingTime / (Time.fixedDeltaTime * 1000.0f);

        Vector3 movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));

        Vector3 movementVelocity = movingDirection * 75 * 0.1f;

        Vector3 finalVelocity = movementVelocity /*+ pushVelocity + GravityVelocity*/;
        transform.localPosition += finalVelocity * Time.fixedDeltaTime * fixedTimePercentage;
    }


    private void MoveServerRepresentation(float axis_X, float axis_Z, float remainingTime)
    {
        float fixedTimePercentage = remainingTime / (Time.fixedDeltaTime * 1000.0f);

        Vector3 movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));

        Vector3 movementVelocity = movingDirection * 75 * 0.1f;

        Vector3 finalVelocity = movementVelocity /*+ pushVelocity + GravityVelocity*/;
        serverMovementRepresentation.Position += finalVelocity * Time.fixedDeltaTime * fixedTimePercentage;
    }

    private void LookAtMovingDirection(float axis_X, float axis_Z)
    {
        RotateRelativeToGrandParentRotation(axis_X, axis_Z);
    }

    private void RotateRelativeToGrandParentRotation(float axis_X, float axis_Z)
    {
        if (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > 0.0f)
        {
            Transform myGrandTransform = GetGrandFatherTransfrom();
            Vector3 newForwardDirection = myGrandTransform.TransformDirection(new Vector3(axis_X, 0.0f, axis_Z));
            Quaternion newRotation = Quaternion.LookRotation(newForwardDirection, transform.up);
            transform.rotation = newRotation;
        }
    }

    private Transform GetGrandFatherTransfrom()
    {
        Transform myGrandParentTransform = transform;
        while (myGrandParentTransform.parent != null)
        {
            myGrandParentTransform = myGrandParentTransform.parent;
        }

        return myGrandParentTransform;
    }


    private IEnumerator ReplicateMovementCoroutine()
    {
        // Reset position
        serverMovementRepresentation.Position = initialPosition;

        // Clear trail 
        if (myTrailRenderer != null)
        {
            serverMovementRepresentation.ClearTrailRenderer();
        }

        if (notAcknowledgedMovementMessages.Count > 2)
        {

            currentExecution = 0;

            DebugLog("Fixed delta time : " + Time.fixedDeltaTime * 1000, "Replication Movement");

            for (int i = 0; i < notAcknowledgedMovementMessages.Count - 1; i++)
            {
                NetEvent_JoystickMoved currentEvent = notAcknowledgedMovementMessages[i];
                NetEvent_JoystickMoved nextEvent = notAcknowledgedMovementMessages[i + 1];
                float timeDifference = nextEvent.TimeStamp - currentEvent.TimeStamp;

                DebugLog(i + " - " + (i + 1) + "  time difference : " + timeDifference, "Replication Movement");
            }

            for (int i = 0; i < notAcknowledgedMovementMessages.Count - 1; i++)
            {

                NetEvent_JoystickMoved currentEvent = notAcknowledgedMovementMessages[i];
                NetEvent_JoystickMoved nextEvent = notAcknowledgedMovementMessages[i + 1];

                float timeDifference = nextEvent.TimeStamp - currentEvent.TimeStamp;

                while (timeDifference >= Time.fixedDeltaTime * 1000)
                {
                    MoveServerRepresentation(currentEvent.Joystick_X, currentEvent.Joystick_Y, Time.fixedDeltaTime * 1000.0f);

                    timeDifference -= Time.fixedDeltaTime * 1000;
                    yield return new WaitForFixedUpdate();
                }

                MoveServerRepresentation(currentEvent.Joystick_X, currentEvent.Joystick_Y, timeDifference);

                currentExecution++;
            }

            NetEvent_JoystickMoved lastEvent = notAcknowledgedMovementMessages[notAcknowledgedMovementMessages.Count - 1];

            MoveServerRepresentation(lastEvent.Joystick_X, lastEvent.Joystick_Y, Time.fixedDeltaTime * 1000.0f);

            currentExecution++;



            yield return new WaitForFixedUpdate();
        }

    }


    

}
