using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Maleficus.MaleficusConsts;
using static Maleficus.MaleficusUtilities;

public class AIInputSource : AbstractInputSource
{
    //private List<EControllerID> connectedAIControllers = new List<EControllerID>();


    protected override void Update()
    {
        base.Update();

        DebugAI();

    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();




        foreach (Player aiPlayer in PlayerManager.Instance.ActivePlayers.Values)
        {
            EControllerID controllerID = InputManager.Instance.GetConnectedControllerID(aiPlayer.PlayerID);
            if (controllerID.ContainedIn(AI_CONTROLLERS) == true)
            {
                Player closestPlayer = GetClosestPlayerTo(aiPlayer);
                if (closestPlayer == null)
                {
                    continue;
                }

                Vector3 lookVector3 = (closestPlayer.Position - aiPlayer.Position).normalized;

                InvokeJoystickMoved(controllerID, EJoystickType.ROTATION, lookVector3.x, -lookVector3.z);
            }
        }

    }


    private void DebugAI()
    {
        string aIDebug = "";
        foreach (EControllerID controllerID in AI_CONTROLLERS)
        {
            if (InputManager.Instance.IsControllerConnected(controllerID))
            {
                aIDebug += "Controlling : " + controllerID + " \n";
            }
        }
        DebugLogOnCanvas(aIDebug, 6);
    }




    private Player GetClosestPlayerTo(Player player)
    {
        Player closestOtherPlayer = null;
        float closestDistanceToOtherPlayer = float.MaxValue;
        foreach (Player otherPlayer in PlayerManager.Instance.ActivePlayers.Values)
        {
            if (otherPlayer.PlayerID == player.PlayerID)
            {
                continue;
            }

            float distanceToOtherPlayer = Vector3.Distance(player.Position, otherPlayer.Position);
            if (distanceToOtherPlayer < closestDistanceToOtherPlayer)
            {
                closestOtherPlayer = otherPlayer;
                closestDistanceToOtherPlayer = distanceToOtherPlayer;
            }
        }
        return closestOtherPlayer;
    }
}
