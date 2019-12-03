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
    IN_MENU_IN_CONNECTING_GAMEPADS,
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
    GAME_AR,
    GAME_DUNGEON,
    MENU_DUNGEON,
    SERVER,
    INSTANCE_MANAGER,
    INSTANCE
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

public enum ESpellSlot
{
    NONE,
    SPELL_1,
    SPELL_2,
    SPELL_3
}

public enum ESpellType
{
    BUFF_ONSELF,
    PUSH,
    BUFF_ONTEAMMATE
}



public enum ESpellEffects
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



public enum ESpellMovementType
{
    LINEAR_HIT,
    LINEAR_EXPLOSIVE,
    PARABOLIC_EXPLOSIVE,
    LINEAR_INSTANT, //Single or multi
    TELEPORT,
    AOE,
    LINEAR_LASER,
    RAPID_FIRE

}

public enum ESpellID
{
    FIREBALL,
    FIRE_LASER,
    FIRE_EXPLOSION,
    ICEBALL,
    TELEPORT,
    ELECTRICBALL,
    VAMPERICBALL
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
    public SHitInfo(ISpell castedSpell, EPlayerID castingPlayerID, EPlayerID hitplayerID, Vector3 hitPosition, bool hasPower, bool chargeable  , bool isTripleCast, List<ESpellEffects> debuffEffects, List<ESpellEffects> buffEffects)
    {
        this.castedSpell = castedSpell;
        this.castingPlayerID = castingPlayerID;
        this.hitPlayerID = hitplayerID;
        this.hitPosition = hitPosition;
        this.hasPower = hasPower;
        this.buffEffects = buffEffects;
        this.debuffEffects = debuffEffects;
        this.chargeable = chargeable;
        this.isTripleCast = isTripleCast;

    }

    public ISpell CastedSpell { get { return castedSpell; } }
    public EPlayerID CastingPlayerID { get { return castingPlayerID; } }
    public EPlayerID HitPlayerID { get { return hitPlayerID; } }
    public Vector3 HitPosition { get { return hitPosition; } }
    public Vector3 HitVelocity
    {
        get
        {
            Debug.Log("§$%§$%§ dir : " + castedSpell.Direction + " | pow : " + castedSpell.HitPower);
            return castedSpell.Direction * castedSpell.HitPower;
        }
    }
    public bool HasPushPower { get { return hasPower; } }
    public List<ESpellEffects> DebuffEffects { get { return debuffEffects; } }
    public List<ESpellEffects> BuffEffects { get { return buffEffects; } }
    public bool Chargeable { get { return chargeable; } }
    public bool IsTripleCast { get { return isTripleCast; } }

    private bool isTripleCast;
    private bool chargeable;
    private ISpell castedSpell;
    private EPlayerID castingPlayerID;
    private EPlayerID hitPlayerID;
    private Vector3 hitPosition;
    private bool hasPower;
    private List<ESpellEffects> debuffEffects;
    private List<ESpellEffects> buffEffects;
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
    TOUCH,
    AI_1,
    AI_2,
    AI_3,
    AI_4,
    GAMEPAD_A,
    GAMEPAD_B,
    GAMEPAD_C,
    GAMEPAD_D,
    NETWORK_1,
    NETWORK_2,
    NETWORK_3,
    NETWORK_4
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

public enum EJoystickType
{
    NONE,
    MOVEMENT,
    ROTATION
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

public class JoystickInput
{
    public Dictionary<EInputAxis, float> JoystickValues = new Dictionary<EInputAxis, float>();

    public JoystickInput()
    {
        Flush();
    }

    public void Flush()
    {
        // Initialize joystick moved
        JoystickValues[EInputAxis.MOVE_X] = 0.0f;
        JoystickValues[EInputAxis.MOVE_Y] = 0.0f;
        JoystickValues[EInputAxis.ROTATE_X] = 0.0f;
        JoystickValues[EInputAxis.ROTATE_Y] = 0.0f;
    }

    public bool HasMoved()
    {
        return ((JoystickValues[EInputAxis.MOVE_X] != 0.0f) || (JoystickValues[EInputAxis.MOVE_Y] != 0.0f));
    }

    public bool HasRotated()
    {
        return ((JoystickValues[EInputAxis.ROTATE_X] != 0.0f) || (JoystickValues[EInputAxis.ROTATE_Y] != 0.0f));
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
    NONE = 0,

    /* Entry */
    IN_ENTRY = 1,
    IN_ENTRY_IN_LOGIN = 2,
    IN_ENTRY_IN_LOGIN_IN_LOGIN = 3,
    IN_ENTRY_IN_LOGIN_IN_AUTO_REGISTER = 5,
    IN_ENTRY_IN_LOGIN_IN_REGISTER = 6,
    IN_ENTRY_IN_LOGIN_IN_FOLLOW = 7,
    IN_ENTRY_IN_LOGIN_IN_LEGAL = 8,

    /* Menu */
    IN_MENU_MAIN = 9,
    IN_MENU_IN_GAME_MODE_SELECTION = 10,
    IN_MENU_IN_SPELL_SELECTION = 11,
    IN_MENU_IN_ARENA_SELECTION = 12,

    /* Game */
    IN_GAME_IN_NOT_STARTED = 13,
    IN_GAME_IN_COUNTDOWN = 14,
    IN_GAME_IN_RUNNING = 15,
    IN_GAME_IN_PAUSED = 16,
    IN_GAME_OVER = 17
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

#region NETWORK

public enum ENetMessageType
{
    NONE = 0,

    CREATE_ACCOUNT = 1,
    LOGIN_REQUEST = 2,

    ON_CREATE_ACCOUNT = 3,
    ON_LOGIN_REQUEST = 4,

    ADD_FOLLOW = 5,
    REMOVE_FOLLOW = 6,
    REQUEST_FOLLOW = 7,

    ON_ADD_FOLLOW = 8,
    ON_REQUEST_FOLLOW = 9,
    UPDATE_FOLLOW = 10,

    INIT_LOBBY = 11,

    ON_INITI_LOBBY = 12,

    CONNECTED = 13,
    DISCONNECTED = 14,

    REQUEST_GAME_SESSION_INFO = 17,
    ON_REQUEST_GAME_SESSION_INFO = 18,

    GAME_STATE_REPLICATION = 19,

    GAME_STARTED = 20,

    AR_STAGE_PLACED = 21,

    GAME_OVER = 22,

    UPDATE_ACCOUNT = 23,
    ON_UPDATE_ACCOUNT = 24,

    BUTTON_PRESSED = 30,
    BUTTON_RELEASEED = 31,
    JOYSTICK_MOVED = 32

}

public enum ENetworkMessageType
{
    NONE,
    OFFLINE,
    CONNECTED,
    DISCONNECTED,
    DATA,
    DATA_NONE,
    DATA_ONCREATEACCOUNT,
    DATA_ONLOGINREQUEST,
    DATA_ONADDFOLLOW,
    DATA_ONREQUESTFOLLOW,
    DATA_ONUPDATEFOLLOW,
    DATA_ONINITLOBBY,
    DATA_REQUESTGAMEINFO,
    DATA_ONREQUESTGAMEINFO,
    DATA_SPELLINPUT,
    BROADCAST,
    LOGGED_IN,
    LOGGED_OUT,
    REGISTERED
}


public enum EClientID
{
    NONE,
    SERVER,
    CLIENT_1,
    CLIENT_2,
    CLIENT_3,
    CLIENT_4,
    TEST
}

public enum EEventInvocationType
{
    TO_ALL,
    LOCAL_ONLY,
    TO_SERVER_ONLY,
}

public enum EConnectionMode
{
    LOCAL_SERVER,
    CLOUD_SERVER,
    PLAY_OFFLINE,
    BNJMO_SERVER,
    GOOGLE_CLOUD_SERVER
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

