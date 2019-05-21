using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region AppState
public enum EAppState
{
    IN_MENU,
    CONNECTING_PLAYERS,
    IN_GAME,
    TEST
}
#endregion

#region Player
public enum EPlayerID
{
    NONE,
    PLAYER_1,
    PLAYER_2,
    PLAYER_3,
    PLAYER_4,
    TEST
}

public struct HitInfo
{
    public HitInfo(ISpell castedSpell, EPlayerID castingPlayerID, EPlayerID hitplayerID, Vector3 hitPosition)
    {
        this.castedSpell = castedSpell;
        this.castingPlayerID = castingPlayerID;
        this.hitPlayerID = hitplayerID;
        this.hitPosition = hitPosition;
    }

    public ISpell CastedSpell { get { return castedSpell; } }
    public EPlayerID CastingPlayerID { get { return castingPlayerID; } }
    public EPlayerID HitPlayerID { get { return hitPlayerID; } }
    public Vector3 HitPosition { get { return hitPosition; } }
    public Vector3 HitVelocity { get { return hitPosition + castedSpell.Direction * castedSpell.HitPower; } }


    private ISpell castedSpell;
    private EPlayerID castingPlayerID;
    private EPlayerID hitPlayerID;
    private Vector3 hitPosition;
}
#endregion 

#region Input
public enum EInputMode
{
    CONTROLLER,
    TOUCH,
    TEST
}
public enum EInputButton
{
    CONFIRM,
    CANCEL,
    CAST_SPELL_1,
    CAST_SPELL_2,
    CAST_SPELL_3,
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum EInputAxis
{
    MOVE_X,
    MOVE_Y,
    ROTATE_X,
    ROTATE_Y
}

public enum ETouchJoystickType
{
    MOVE,
    SPELL_1,
    SPELL_2,
    SPELL_3
}

public enum EJoystickAxisRestriction
{
    BOTH,
    HORIZONTAL,
    VERTICAL
}

public enum EJoystickState
{
    NONE,
    IDLE,
    SELECTED_CAN_TRIGGER_BUTTON,
    SELECTED_CANNOT_TRIGGER_BUTTON
}

public class PlayerInput
{
    public float Move_X;
    public float Move_Y;
    public float Rotate_X;
    public float Rotate_Y;

    public void Flush()
    {
        Move_X = 0.0f;
        Move_Y = 0.0f;
        Rotate_X = 0.0f;
        Rotate_Y = 0.0f;
    }

    public bool HasMoved()
    {
        return ((Move_X != 0.0f) || (Move_Y != 0.0f));
    }

    public bool HasRotated()
    {
        return ((Rotate_X != 0.0f) || (Rotate_Y != 0.0f));
    }
}
#endregion

#region UI
public enum EMenuState
{
    NONE,
    MAIN,
    IN_GAME
}

public enum EButtonDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}
#endregion



public static class MaleficusTypes 
{
    public static int PlayerIDToInt(EPlayerID playerID)
    {
        int id = 0;
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                id = 1;
                break;
            case EPlayerID.PLAYER_2:
                id = 2;
                break;
            case EPlayerID.PLAYER_3:
                id = 3;
                break;
            case EPlayerID.PLAYER_4:
                id = 4;
                break;
        }
        return id;
    }

    public static EPlayerID IntToPlayerID(int playerID)
    {
        EPlayerID id = 0;
        switch (playerID)
        {
            case 1:
                id = EPlayerID.PLAYER_1;
                break;
            case 2:
                id = EPlayerID.PLAYER_2;
                break;
            case 3:
                id = EPlayerID.PLAYER_3;
                break;
            case 4:
                id = EPlayerID.PLAYER_4;
                break;
        }
        return id;
    }
}
