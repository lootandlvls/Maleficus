using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MaleficusTypes 
{
    // Player characters paths
    public const string PATH_PLAYER_RED             = "Wizard_Red";
    public const string PATH_PLAYER_BLUE            = "Wizard_Blue";
    public const string PATH_PLAYER_GREEN           = "Wizard_Green";
    public const string PATH_PLAYER_YELLOW          = "Wizard_Yellow";
    public const string PATH_PLAYER_SPAWN_POSITION  = "PlayerSpawnPosition";

    // Spells path
    public const string PATH_SPELL_FIRE             = "Spell_Fire";         // TODO: Define rest




    /// Threshold to know what joystick value can be considered as a directional button 
    public const float DIRECTIONAL_BUTTON_THRESHOLD = 0.5f;
    /// Threshold to know when a touch joystick can perform a spell button
    public const float SPELL_BUTTON_THRESHOLD = 0.3f;

    public const float ROTATION_THRESHOLD = 0.0f;


    /// Update these lists when more states are added to AppState!
    public static EAppState[] STATES_WITH_UI = new EAppState[]  // used mainly for menu navigation with controller
    {
        // In Lobby
        EAppState.IN_LOBBY_MAIN,
        EAppState.IN_LOBBY_CONNECTING_PLAYERS,
        EAppState.IN_LOGIN,
        EAppState.IN_STARTUP,
        // In Game
        EAppState.IN_GAME_PAUSED,
        EAppState.IN_GAME_ENDED
    };

    public static EAppState[] STATES_IN_LOBBY = new EAppState[]
    {
        EAppState.IN_LOBBY_MAIN,
        EAppState.IN_LOBBY_CONNECTING_PLAYERS
    };

    public static EAppState[] STATES_IN_GAME = new EAppState[]
    {
        EAppState.IN_GAME_NOT_STARTED,
        EAppState.IN_GAME_ABOUT_TO_START,
        EAppState.IN_GAME_RUNNING,
        EAppState.IN_GAME_PAUSED,
        EAppState.IN_GAME_ENDED,
    };



    /// Convert a PlayerID enum to an int
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

    /// Convert an int to a PlayerID enum 
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







#region AppState
/// Update the lists inside the class when more states are added to AppState!
public enum EAppState
{
    IN_STARTUP,
    IN_LOGIN,
    IN_LOBBY_MAIN,
    IN_LOBBY_CONNECTING_PLAYERS,
    IN_GAME_NOT_STARTED,
    IN_GAME_ABOUT_TO_START,
    IN_GAME_RUNNING,
    IN_GAME_PAUSED,
    IN_GAME_ENDED,
    TEST
}


#endregion

#region Game
public enum EGameMode
{
    SINGLE_LIVES_5,
    SINGLE_TIME_2,
    SINGLE_POINTS_100,
    INSANE
        // TODO: Define rest of game modes
}


#endregion

#region Spells



public enum SpellType
{
    BUFF_ONSELF,
    PUSH,
    BUFF_ONTEAMMATE
}



public enum SpellEffects
{
    PROTECT,
    INCREASE_DAMAGE,
    INCREACE_SPEED,
    INCREASE_CASTING_SPEED,
    REMOVE_DEBUFF,
    INCREASE_OFFENSIVE_SPELL_SIZE,
    FROZEN,
    STUN,
    SLOWDOWN,
    CHARM,
    NONE
}



public enum MovementType
{
    LINEAR_HIT,
    LINEAR_EXPLOSIVE,
    PARABOLIC_EXPLOSIVE,
    LINEAR_INSTANT, //Single or multi
    TELEPORT,
    AOE,
    LINEAR_LASER

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

public enum ETeamID
{
    NONE,
    TEAM_1,
    TEAM_2,
    TEAM_3,
    TEAM_4
}

public struct HitInfo
{
    public HitInfo(ISpell castedSpell, EPlayerID castingPlayerID, EPlayerID hitplayerID, Vector3 hitPosition, bool hasPower, List<SpellEffects> debuffEffects, List<SpellEffects> buffEffects)
    {
        this.castedSpell = castedSpell;
        this.castingPlayerID = castingPlayerID;
        this.hitPlayerID = hitplayerID;
        this.hitPosition = hitPosition;
        this.hasPower = hasPower;
        this.buffEffects = buffEffects;
        this.debuffEffects = debuffEffects;
        
    }

    public ISpell CastedSpell { get { return castedSpell; } }
    public EPlayerID CastingPlayerID { get { return castingPlayerID; } }
    public EPlayerID HitPlayerID { get { return hitPlayerID; } }
    public Vector3 HitPosition { get { return hitPosition; } }
    public Vector3 HitVelocity { get { return hitPosition + castedSpell.Direction ; } }
    public bool HasPower { get { return hasPower; } }
    public List<SpellEffects> DebuffEffects { get {return debuffEffects; }}
    public List<SpellEffects> BuffEffects { get { return buffEffects; } }


    private ISpell castedSpell;
    private EPlayerID castingPlayerID;
    private EPlayerID hitPlayerID;
    private Vector3 hitPosition;
    private bool hasPower;
    private List<SpellEffects> debuffEffects;
    private List<SpellEffects> buffEffects;
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
    STARTUP,
    LOGIN,
    LOGIN_REGISTER,
    LOGIN_LOGIN,
    LOGIN_FOLLOW,
    LOGGIN_LEGAL,
    MAIN,
    CONNECTING_PLAYERS,
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

#region NETWORK
public enum ENetworkMessage
{
    NONE,
    CONNECTED,
    DISCONNECTED,
    DATA,
    BROADCAST
}
#endregion
