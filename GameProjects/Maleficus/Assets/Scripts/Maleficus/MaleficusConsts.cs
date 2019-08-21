using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains pre-defines constant members.
/// </summary>
public static class MaleficusConsts
{
    /* Scene names (in build settings) */
    public const string SCENE_ENTRY = "ENTRY";
    public const string SCENE_MENU = "MENU";
    public const string SCENE_GAME = "GAME";
    public const string SCENE_DUNGEON_1 = "LVL_1_AR";
    public const string SCENE_DUNGEON_2 = "LVL_2_AR";
    public const string SCENE_DUNGEON_3 = "LVL_3_AR";
    public const string SCENE_DUNGEON_SELECTION = "LVL_SELECTION";


    // Spells path
    public const string PATH_SPELL_FIREBALL_LVL_1 = "S_FireBall_lvl_1";
    public const string PATH_SPELL_FIREBALL_LVL_2 = "S_FireBall_lvl_2";
    public const string PATH_SPELL_ICEBALL_LVL_1 = "S_IceBall_lvl_1";
    public const string PATH_SPELL_AOE_EXPLOSION_LVL_1 = "S_AOE_Explosion";
    public const string PATH_SPELL_FIRE_SHOCKBLAST_LVL_1 = "S_Fire_ShockBlast";
    public const string PATH_SPELL_PARABOLIC_ENERGY_BALL_LVL_1 = "S_EnergyBalll_Parabolic";
    public const string PATH_SPELL_FIRE__LASER_LVL_1 = "S_FireLaser";
    public const string PATH_SPELL_TELEPORT_LVL_1 = "S_Teleportation";

    //Effects path
    public const string PATH_EFFECT_FROZEN = "E_Snowman";
    public const string PATH_EFFECT_CHARGING_BODYENERGY = "BodyEnergyEffect";
    public const string PATH_EFFECT_CHARGING_WANDENERGY = "WandEnergyEffect";

    public const int NUMBERS_OF_FRAMES_TO_WAIT_BEFORE_CHANGING_SCENE = 3;

    /* Player characters paths */
    public const string PATH_PLAYER_RED = "Wizard_Red";
    public const string PATH_PLAYER_BLUE = "Wizard_Blue";
    public const string PATH_PLAYER_GREEN = "Wizard_Green";
    public const string PATH_PLAYER_YELLOW = "Wizard_Yellow";
    public const string PATH_PLAYER_SPAWN_POSITION = "PlayerSpawnPosition";


    /* Tags */
    public const string TAG_SPELL_HITTABLE = "SpellHittable";


    /* Managers path */
    public const string PATH_MANAGERS = "Managers";



    /// <summary> Threshold to know what joystick value can be considered as a directional button </summary>  
    public const float DIRECTIONAL_BUTTON_THRESHOLD = 0.5f;

    /// <summary> Threshold to know when a touch joystick can perform a spell button </summary>
    public const float SPELL_BUTTON_THRESHOLD = 0.3f;

    public const float ROTATION_THRESHOLD = 0.0f;

    public const float ENG_GAME_SCENE_TRANSITION_DURATION = 4.0f;

    public const int PLAYER_LIVES_IN_DUNGEON_MODE = 10;
    public const int PLAYER_LIVES_IN_FFA_MODE = 5;

    public const int PLAYER_FALLING_TIME = 2;

    /// <summary> Scenes switch logic </summary>
    public static readonly Dictionary<EScene, EScene> FROM_SCENE_TO = new Dictionary<EScene, EScene>()
    {
        { EScene.NONE,  EScene.NONE },
        { EScene.ENTRY, EScene.MENU },
        { EScene.MENU,  EScene.GAME },
        { EScene.GAME,  EScene.MENU },

        // AR Mode
        { EScene.DUNGEON_SELECTION,     EScene.AR_GAME },
        { EScene.AR_GAME,               EScene.DUNGEON_SELECTION },
    };

    /* Start states for the different scenes*/
    public static Dictionary<EScene, EAppState> START_APP_STATES = new Dictionary<EScene, EAppState>()
    {
        { EScene.NONE,                  EAppState.NONE},
        { EScene.ENTRY,                 EAppState.IN_ENTRY },
        { EScene.MENU,                  EAppState.IN_MENU_IN_MAIN },
        { EScene.GAME,                  EAppState.IN_GAME_IN_NOT_STARTED },

        // AR Mode
        { EScene.AR_GAME,               EAppState.IN_GAME_IN_NOT_STARTED },
        { EScene.DUNGEON_SELECTION,     EAppState.IN_MENU_IN_MAIN },
    };
    public static Dictionary<EScene, EMenuState> START_MENU_STATES = new Dictionary<EScene, EMenuState>()
    {
        { EScene.NONE,                  EMenuState.NONE},
        { EScene.ENTRY,                 EMenuState.IN_ENTRY},
        { EScene.MENU,                  EMenuState.IN_MENU},
        { EScene.GAME,                  EMenuState.IN_GAME_NOT_STARTED},

        // AR Mode
        { EScene.AR_GAME,               EMenuState.IN_GAME_NOT_STARTED},
        { EScene.DUNGEON_SELECTION,     EMenuState.IN_MENU},
    };
    public static Dictionary<EScene, EARState> START_AR_STATES = new Dictionary<EScene, EARState>()
    {
        { EScene.NONE,                  EARState.NO_POSE},
        { EScene.ENTRY,                 EARState.NO_POSE},
        { EScene.MENU,                  EARState.NO_POSE},
        { EScene.GAME,                  EARState.NO_POSE},

        // AR Mode
        { EScene.AR_GAME,               EARState.NO_POSE},
        { EScene.DUNGEON_SELECTION,     EARState.NO_POSE},
    };
    public static EAppState[] APP_STATES_THAT_TRIGGER_SCENE_CHANGE = new EAppState[]
    {
        EAppState.IN_ENTRY_IN_LOADING,
        EAppState.IN_MENU_IN_STARTING_GAME,
        EAppState.IN_MENU_IN_STARTING_AR_GAME,
        EAppState.IN_GAME_IN_END_SCENE
    };


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

    public static EAppState[] APP_STATES_IN_ENTRY = new EAppState[]
    {
        EAppState.IN_ENTRY_IN_LOADING
    };

    public static EAppState[] APP_STATES_IN_MENU = new EAppState[]
    {
        EAppState.IN_MENU_IN_MAIN,
        EAppState.IN_MENU_IN_CONNECTING_PLAYERS,
        EAppState.IN_MENU_IN_STARTING_GAME
,
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
        EMenuState.IN_ENTRY_IN_LOGIN_IN_LOGIN,
        EMenuState.IN_ENTRY_IN_LOGIN_IN_REGISTER,
        EMenuState.IN_ENTRY_IN_LOGIN_IN_FOLLOW,
        EMenuState.IN_ENTRY_IN_LOGIN_IN_LEGAL
    };
}
