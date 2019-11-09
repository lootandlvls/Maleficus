using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maleficus
{
    /// <summary>
    /// Contains pre-defines constant members.
    /// </summary>
    public static class MaleficusConsts
    {
        /* GameObjects Tags */
        public const string TAG_SPELL_HITTABLE = "SpellHittable";

        /* Scene names (in build settings) */
        public const string SCENE_ENTRY = "ENTRY";
        public const string SCENE_MENU = "MENU";
        public const string SCENE_GAME = "GAME";
        public const string SCENE_DUNGEON_SELECTION = "LVL_SELECTION";

        /* Managers path */
        public const string PATH_MANAGERS_CLIENT = "Managers_CLIENT";
        public const string PATH_MANAGERS_SERVER = "Managers_SERVER";

        /* Spells path */
        public const string PATH_SPELL_FIREBALL_LVL_1 = "S_FireBall_lvl_1";
        public const string PATH_SPELL_FIREBALL_LVL_2 = "S_FireBall_lvl_2";
        public const string PATH_SPELL_ICEBALL_LVL_1 = "S_IceBall_lvl_1";
        public const string PATH_SPELL_AOE_EXPLOSION_LVL_1 = "S_AOE_Explosion";
        public const string PATH_SPELL_FIRE_SHOCKBLAST_LVL_1 = "S_Fire_ShockBlast";
        public const string PATH_SPELL_PARABOLIC_ENERGY_BALL_LVL_1 = "S_EnergyBalll_Parabolic";
        public const string PATH_SPELL_FIRE__LASER_LVL_1 = "S_FireLaser";
        public const string PATH_SPELL_TELEPORT_LVL_1 = "S_Teleportation";

        /* Effects path */
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
        public const string PATH_PLAYER_SERVER_REPRESENTATION = "PlayerServerRepresentation";

        /* Networking */
        public const int CLIENT_MAX_USER = 100;
        public const int SERVER_MAX_USER = 10000;
        public const int SERVER_INSTANCE_MANAGER_PORT = 9999;
        public const int PORT = 26002;
        public const int WEB_PORT = 26004;
        public const int BYTE_SIZE = 1024;
        public const string LOCAL_SERVER_IP = "127.0.0.1";
        public const string CLOUD_SERVER_IP = "83.171.237.227";
        public const string GOOGLE_CLOUD_SERVER_IP = "35.246.234.228";
        public const string BNJMO_SERVER_IP = "192.168.137.1";
        public const string INSTANCE_MANAGER_SERVER_IP = "54.167.218.82";
        public const string PLAY_OFFLINE_IP = "0.0.0";
        // Frequency numbers should be represantable by binary numbers so no conversion errors happen https://www.h-schmidt.net/FloatConverter/IEEE754.html
        public const float GAME_STATE_UPDATE_FREQUENCY = 0.75f;
        public const float NETWORK_UPDATE_FREQUENCY = 0.0078125f;
        public const float NETWORK_CONNECT_FREQUENCY = 0.75f;



        /// <summary> Threshold to know what joystick value can be considered as a directional button </summary>  
        public const float DIRECTIONAL_BUTTON_THRESHOLD = 0.5f;

        /// <summary> Threshold to know when a touch joystick can perform a spell button </summary>
        public const float SPELL_BUTTON_THRESHOLD = 0.3f;

        // Input
        public const float THRESHOLD_JOYSTICK_ACTIVATION = 0.2f;
        public const float THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT = 0.3f;
        public const float THRESHOLD_JOYSTICK_DISTANCE_ROTATION = 0.2f;

        public const float ENG_GAME_SCENE_TRANSITION_DURATION = 4.0f;

        public const int PLAYER_LIVES_IN_DUNGEON_MODE = 10;
        public const int PLAYER_LIVES_IN_FFA_MODE = 2;

        public const int PLAYER_FALLING_TIME = 2;

        public const int PLAYER_RESPAWNING_TIME = 4;

        /// <summary> Scenes switch logic </summary>
        public static readonly Dictionary<EScene, EScene> FROM_SCENE_TO = new Dictionary<EScene, EScene>()
    {
        { EScene.NONE,              EScene.NONE },
        { EScene.ENTRY,             EScene.MENU },
        { EScene.MENU,              EScene.GAME },
        { EScene.GAME,              EScene.MENU },
    };

        /* Start states for the different scenes*/
        public static Dictionary<EScene, EAppState> START_APP_STATES = new Dictionary<EScene, EAppState>()
    {
        { EScene.NONE,              EAppState.NONE},
        { EScene.ENTRY,             EAppState.IN_ENTRY },
        { EScene.MENU,              EAppState.IN_MENU_IN_MAIN },
        { EScene.GAME,              EAppState.IN_GAME_IN_NOT_STARTED },
    };
        public static Dictionary<EScene, EMenuState> START_MENU_STATES = new Dictionary<EScene, EMenuState>()
    {
        { EScene.NONE,              EMenuState.NONE},
        { EScene.ENTRY,             EMenuState.IN_ENTRY},
        { EScene.MENU,              EMenuState.IN_MENU},
        { EScene.GAME,              EMenuState.IN_GAME_NOT_STARTED}
    };


        public static EAppState[] APP_STATES_THAT_TRIGGER_SCENE_CHANGE = new EAppState[]
        {
        EAppState.IN_ENTRY_IN_LOADING,
        EAppState.IN_MENU_IN_STARTING_GAME,
        EAppState.IN_GAME_IN_END_SCENE
        };


        /* App States classifications lists */
        // Update these lists when more states are added to AppState!
        public static EAppState[] APP_STATES_WITH_UI = new EAppState[]  // used mainly for menu navigation with controller
        {
        // In Menu
        EAppState.IN_MENU_IN_MAIN,
        EAppState.IN_MENU_IN_CONNECTING_GAMEPADS,
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
        EAppState.IN_MENU_IN_CONNECTING_GAMEPADS,
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

        /// <summary> Scenes in that are equivalent to the GAME scene </summary>
        public static readonly EScene[] GAME_SCENES = new EScene[]
    {
        EScene.GAME,
        EScene.GAME_DUNGEON
    };

        public static EControllerID[] NETWORK_CONTROLLERS = new EControllerID[]
        {
        EControllerID.NETWORK_1,
        EControllerID.NETWORK_2,
        EControllerID.NETWORK_3,
        EControllerID.NETWORK_4
        };

        public static EControllerID[] GAMEPADS_CONTROLLERS = new EControllerID[]
        {
        EControllerID.GAMEPAD_A,
        EControllerID.GAMEPAD_B,
        EControllerID.GAMEPAD_C,
        EControllerID.GAMEPAD_D
        };


    }
}