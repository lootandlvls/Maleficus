using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MaleficusTypes 
{
    /* Scene names (in build settings) */
    public const string SCENE_MENU                  = "Dummy_MENU";         // TODO: Replace with correct scenes
    public const string SCENE_GAME                  = "Dummy_GAME";

    public const int NUMBERS_OF_FRAMES_TO_WAIT_BEFORE_CHANGING_SCENE = 3;

    /* Player characters paths */
    public const string PATH_PLAYER_RED             = "Wizard_Red";
    public const string PATH_PLAYER_BLUE            = "Wizard_Blue";
    public const string PATH_PLAYER_GREEN           = "Wizard_Green";
    public const string PATH_PLAYER_YELLOW          = "Wizard_Yellow";
    public const string PATH_PLAYER_SPAWN_POSITION  = "PlayerSpawnPosition";

    /* Spells path */
    public const string PATH_SPELL_FIRE             = "Spell_Fire";         // TODO: Define rest



    /// Threshold to know what joystick value can be considered as a directional button 
    public const float DIRECTIONAL_BUTTON_THRESHOLD = 0.5f;
    /// Threshold to know when a touch joystick can perform a spell button
    public const float SPELL_BUTTON_THRESHOLD = 0.3f;

    public const float ROTATION_THRESHOLD = 0.0f;


    /* App States classifications lists */
    // Update these lists when more states are added to AppState!
    public static EAppState[] APP_STATES_WITH_UI = new EAppState[]  // used mainly for menu navigation with controller
    {
        // In Menu
        EAppState.IN_MENU_IN_MAIN,
        EAppState.IN_MENU_IN_CONNECTING_PLAYERS,
        EAppState.IN_MENU_IN_LOGING_IN,

        // In Game
        EAppState.IN_GAME_IN_PAUSED,
        EAppState.IN_GAME_IN_ENDED
    };

    public static EAppState[] APP_STATES_IN_MENU = new EAppState[]
    {
        EAppState.IN_MENU_IN_MAIN,
        EAppState.IN_MENU_IN_CONNECTING_PLAYERS
    };

    public static EAppState[] APP_STATES_IN_GAME = new EAppState[]
    {
        EAppState.IN_GAME_IN_NOT_STARTED,
        EAppState.IN_GAME_IN_ABOUT_TO_START,
        EAppState.IN_GAME_IN_RUNNING,
        EAppState.IN_GAME_IN_PAUSED,
        EAppState.IN_GAME_IN_ENDED,
    };

    /* UI States classifications lists */
    // Update these lists when more states are added to MenuState!
    public static EMenuState[] MENU_STATES_STARTUP = new EMenuState[]
    {
        EMenuState.IN_STARTUP
    };

    public static EMenuState[] MENU_STATES_IN_LOGIN = new EMenuState[]
    {
        EMenuState.IN_LOGIN_IN_LOGIN,
        EMenuState.IN_LOGIN_IN_REGISTER,
        EMenuState.IN_LOGIN_IN_FOLLOW,
        EMenuState.IN_LOGIN_IN_LEGAL
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
/// <summary>
/// State of the whole application. 
/// "IN" is used a hierarchy separator. A state can either be in MENU or in GAME.
/// Note: Update the states classifications lists inside the class when more states are added to AppState!
/// </summary>
public enum EAppState
{
    IN_MENU_IN_MAIN,
    IN_MENU_IN_CONNECTING_PLAYERS,
    IN_MENU_IN_LOGING_IN,
    IN_MENU_IN_SHOP,
    IN_GAME_IN_NOT_STARTED,
    IN_GAME_IN_ABOUT_TO_START,
    IN_GAME_IN_RUNNING,
    IN_GAME_IN_PAUSED,
    IN_GAME_IN_ENDED,
    IN_GAME_IN_ABORTED,
    TEST
}

public enum EScene
{
    NONE,
    MENU,
    GAME
}


#endregion

#region Game
public enum EGameMode
{
    NONE,
    SINGLE_LIVES_5,
    SINGLE_TIME_2,
    SINGLE_POINTS_100,
    INSANE
        // TODO: Define rest of game modes
}

public class AbstractPlayerStats
{

}

public class SPlayerLivesStats : AbstractPlayerStats
{
    public SPlayerLivesStats()
    {

    }
    public SPlayerLivesStats(int maximumNumberOfLives)
    {
        remainingLives = maximumNumberOfLives;
        numberOfKilledPlayers = 0;
        lastHitBy = EPlayerID.NONE;
    }

    public int RemainingLives           { get { return remainingLives; } }
    public int NumberOfKilledPlayers    { get { return numberOfKilledPlayers; } }
    public bool IsGameOver                  { get { return remainingLives == 0; } }
    public EPlayerID LastHitBy          { get { return lastHitBy; } }

    /// <summary>
    /// Decrement by 1 a player's lives and tell if he died.
    /// </summary>
    /// <returns> are reamining lives = 0 </returns>
    public bool DecrementPlayerLives()
    {
        remainingLives--;
        return remainingLives == 0;
    }

    /// <summary>
    /// Icrements the number of killed players by 1
    /// </summary>
    public void IncrementNumberOfKilledPlayers()
    {
        numberOfKilledPlayers++;
    }

    /// <summary>
    /// Sets the Player ID of the last player that hit this player.
    /// Used to determine who finally killed this player.
    /// </summary>
    public void SetLastHitBy(EPlayerID hitByPlayerID)
    {
        lastHitBy = hitByPlayerID;
    }

    private int remainingLives;
    private int numberOfKilledPlayers;
    private EPlayerID lastHitBy;

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
/// <summary>
/// State of the UI menu.
/// "IN" is used here as a hierarchy separator. Generally second IN refers to a pop-up context on top of the original menu state.
/// </summary>
public enum EMenuState
{
    NONE,
    IN_MAIN,
    IN_CONNECTING_PLAYERS,
    IN_STARTUP,                                                                         //TODO: Define. Ambiguous meaning
    /* Login context */
    IN_LOGIN,
    IN_LOGIN_IN_LOGIN,
    IN_LOGIN_IN_REGISTER,
    IN_LOGIN_IN_FOLLOW,
    IN_LOGIN_IN_LEGAL,
    /* Game context */
    IN_GAME_ABOUT_TO_START,
    IN_GAME_RUNNING,
    IN_GAME_PAUSED,
    IN_GAME_OVER
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
    BROADCAST,
    LOGGED_IN,
    LOGGED_OUT,
    REGISTERED
}
#endregion
