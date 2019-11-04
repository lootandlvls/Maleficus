using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusUtilities;
using System.Collections;

public class NetworkMovement : MonoBehaviour
{

    private List<NetEvent_JoystickMoved> notAcknowledgedMovementMessages = new List<NetEvent_JoystickMoved>();

    private Vector2 currentMovementInput;

    private EClientID myClientID = EClientID.NONE;

    private int currentExecution = 0;

    private Vector3 initialPosition;

    private void Awake()
    {
        Player myPlayer = GetComponent<Player>();
        if (myPlayer != null)
        {
            myClientID = GetClientIDFrom(myPlayer.PlayerID);
        }
    }

    private void Start()
    {
        EventManager.Instance.INPUT_JoystickMoved.AddListener                   (On_INPUT_JoystickMoved);
        EventManager.Instance.NETWORK_GameStateReplication.AddListener          (On_NETWORK_GameStateReplicate);

        initialPosition = transform.position;

    }

    private void Update()
    {
        DebugManager.Instance.Log(92, "Not Ack # : " + notAcknowledgedMovementMessages.Count);
        DebugManager.Instance.Log(93, "Current execution # : " + currentExecution);

        if (Input.GetKeyDown("r"))
        {
            StartCoroutine(ReplicateMovementCoroutine());
        }

    }

    private void FixedUpdate()
    {
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
    }

    private void OtherClientUpdate()
    {

    }


    private void On_INPUT_JoystickMoved(NetEvent_JoystickMoved eventHandle)
    {
        currentMovementInput = new Vector2(eventHandle.Joystick_X, eventHandle.Joystick_Y);

        notAcknowledgedMovementMessages.Add(eventHandle);

    }

    private void On_NETWORK_GameStateReplicate(NetEvent_GameStateReplication eventHandle)
    {
        if (eventHandle.UpdatedPlayerID == GetPlayerIDFrom(myClientID))
        {
            transform.position = GetVectorFrom(eventHandle.playerPosition);

            List<NetEvent_JoystickMoved> newList = new List<NetEvent_JoystickMoved>();
            foreach (NetEvent_JoystickMoved notAcknowledgedMovementMessage in notAcknowledgedMovementMessages)
            {
                // Stored net message is more recenet than received game state
                if (notAcknowledgedMovementMessage.TimeStamp > eventHandle.TimeStamp)
                {
                    Move(notAcknowledgedMovementMessage.Joystick_X, notAcknowledgedMovementMessage.Joystick_Y, Time.fixedDeltaTime * 1000.0f);

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


    private IEnumerator ReplicateMovementCoroutine()
    {
        transform.position = initialPosition;

        if (notAcknowledgedMovementMessages.Count > 2)
        {

            currentExecution = 0;

            Debug.Log("Fixed delta time : " + Time.fixedDeltaTime * 1000);
            for (int i = 0; i < notAcknowledgedMovementMessages.Count - 1; i++)
            {
                NetEvent_JoystickMoved currentEvent = notAcknowledgedMovementMessages[i];
                NetEvent_JoystickMoved nextEvent = notAcknowledgedMovementMessages[i + 1];
                float timeDifference = nextEvent.TimeStamp - currentEvent.TimeStamp;

                Debug.Log(i + " - " + (i+1) + "  time difference : " + timeDifference);
            }

            for (int i = 0; i < notAcknowledgedMovementMessages.Count - 1; i++)
            {

                NetEvent_JoystickMoved currentEvent = notAcknowledgedMovementMessages[i];
                NetEvent_JoystickMoved nextEvent = notAcknowledgedMovementMessages[i + 1];

                float timeDifference = nextEvent.TimeStamp - currentEvent.TimeStamp;

                while (timeDifference >= Time.fixedDeltaTime * 1000)
                {
                    Move(currentEvent.Joystick_X, currentEvent.Joystick_Y, Time.fixedDeltaTime * 1000.0f);

                    timeDifference -= Time.fixedDeltaTime * 1000;

                    yield return new WaitForFixedUpdate();
                }

                Move(currentEvent.Joystick_X, currentEvent.Joystick_Y, timeDifference);

                currentExecution++;
            }

            NetEvent_JoystickMoved lastEvent = notAcknowledgedMovementMessages[notAcknowledgedMovementMessages.Count - 1];

            Move(lastEvent.Joystick_X, lastEvent.Joystick_Y, Time.fixedDeltaTime * 1000.0f);

            currentExecution++;


            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

    }

}
