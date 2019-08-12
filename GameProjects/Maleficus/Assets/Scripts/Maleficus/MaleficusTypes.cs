using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MaleficusTypes
{

}

#region AppState
/// <summary>
/// State of the whole application. 
/// "IN" is used a hierarchy separator. A state can either be in MENU or in GAME.
/// Note: Update the states classifications lists inside the class when more states are added to AppState!
/// </summary>
public enum EAppState
{
    NONE,
    IN_ENTRY,
    IN_ENTRY_IN_LOGIN,
    IN_ENTRY_IN_LOADING,
    IN_MENU_IN_MAIN,
    IN_MENU_IN_CONNECTING_PLAYERS,
    IN_MENU_IN_LOGING_IN,
    IN_MENU_IN_SHOP,
    IN_MENU_IN_STARTING_GAME,
    IN_MENU_IN_STARTING_AR_GAME,
    IN_GAME_IN_NOT_STARTED,
    IN_GAME_IN_ABOUT_TO_START,
    IN_GAME_IN_RUNNING,
    IN_GAME_IN_PAUSED,
    IN_GAME_IN_ENDED,
    IN_GAME_IN_END_SCENE,
    TEST
}

public enum EScene
{
    NONE,
    ENTRY,
    MENU,
    GAME,
    AR_GAME,
    DUNGEON_SELECTION
}
#endregion

#region Game
public enum EGameMode
{
    NONE,
    FFA_LIVES,
    FFA_TIME,
    FFA_POINTS,
    INSANE,
    DUNGEON
        // TODO: Define rest of game modes
}




#endregion

#region Spells

public enum ESpellID
{
    NONE,
    SPELL_1,
    SPELL_2,
    SPELL_3
}

public enum SpellType                       // TODO [Nassim]: enums start with 'E' + define NONE
{
    BUFF_ONSELF,
    PUSH,
    BUFF_ONTEAMMATE
}



public enum SpellEffects                    // TODO [Nassim]: enums start with 'E' + define NONE
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



public enum MovementType                    // TODO [Nassim]: enums start with 'E' + define NONE
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

public struct SHitInfo
{
    public SHitInfo(ISpell castedSpell, EPlayerID castingPlayerID, EPlayerID hitplayerID, Vector3 hitPosition, bool hasPower, List<SpellEffects> debuffEffects, List<SpellEffects> buffEffects)
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
    NONE,
    CONTROLLER,
    TOUCH,
    TEST
}

public enum EControllerID
{
    NONE = 'X',
    CONTROLLER_A,
    CONTROLLER_B,
    CONTROLLER_C,
    CONTROLLER_D
}

public enum EInputButton
{
    NONE,
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
    NONE,
    MOVE_X,
    MOVE_Y,
    ROTATE_X,
    ROTATE_Y
}

public enum ETouchJoystickType
{
    NONE,
    MOVE,
    SPELL_1,
    SPELL_2,
    SPELL_3
}

public enum EJoystickAxisRestriction
{
    NONE,
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
    public bool IsButtonPressed_1;
    public bool IsButtonPressed_2;
    public bool IsButtonPressed_3;

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
/// <summary>
/// State of the UI menu.
/// "IN" is used here as a hierarchy separator. Generally second IN refers to a pop-up context on top of the original menu state.
/// </summary>
public enum EMenuState
{
    NONE,
    IN_ENTRY,
    IN_MENU,
    IN_CONNECTING_PLAYERS,
    IN_STARTUP,                                                                         //TODO [Leon]: Define. Ambiguous meaning
    /* Login context */
    IN_ENTRY_IN_LOGIN,
    IN_ENTRY_IN_LOGIN_IN_LOGIN,
    IN_ENTRY_IN_LOGIN_IN_REGISTER,
    IN_ENTRY_IN_LOGIN_IN_FOLLOW,
    IN_ENTRY_IN_LOGIN_IN_LEGAL,
    /* Game context */
    IN_GAME_NOT_STARTED,
    IN_GAME_ABOUT_TO_START,
    IN_GAME_RUNNING,
    IN_GAME_PAUSED,
    IN_GAME_OVER
}

public enum EButtonDirection
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN
}
#endregion

                                                            //Todo [Leon]: move data_ to own enums
#region NETWORK
public enum ENetworkMessage
{
    NONE,
    CONNECTED,
    DISCONNECTED,
    DATA,
    DATA_NONE,
    DATA_ONCREATEACCOUNT,
    DATA_ONLOGINREQUEST,
    DATA_ONADDFOLLOW,
    DATA_ONREQUESTFOLLOW,
    DATA_ONUPDATEFOLLOW,
    DATA_SPELLINPUT,
    BROADCAST,
    LOGGED_IN,
    LOGGED_OUT,
    REGISTERED
}

public enum EClientID
{
    NONE,
    CLIENT_1,
    CLIENT_2,
    CLIENT_3,
    CLIENT_4
}
#endregion

#region ENEMIES
public enum EEnemyType
{
    NONE,
    BASIC,
    CHAMPION,
    BOSS,
    MINION
}

public enum EEnemyMovementMethod
{
    NONE,
    NAV_MESH,
    WAYPOINTS
}

public enum EDungeonID
{
    NONE,
    ONE,
    TWO,
    THREE
}
#endregion

#region AR
public enum EARState
{
    NONE,
    NO_POSE,
    LIMITED,
    DETECTED,
    TRACKED,
    EXTENDED_TRACKED
}

public enum EPlacementMethod
{
    NONE,
    ON_PLANE,
    MID_AIR
}
#endregion

