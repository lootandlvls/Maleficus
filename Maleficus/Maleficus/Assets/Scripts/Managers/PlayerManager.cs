using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerManager : Singleton<PlayerManager>
{

    [SerializeField] private Player player_1;
    [SerializeField] private Player player_2;
    [SerializeField] private Player player_3;
    [SerializeField] private Player player_4;

    Dictionary<PlayerID, Player> players;
    Dictionary<PlayerID, PlayerInput> playersInput;


    private void Start()
    {
        players = new Dictionary<PlayerID, Player>();
        players[PlayerID.PLAYER_1] = player_1;
        players[PlayerID.PLAYER_2] = player_2;
        players[PlayerID.PLAYER_3] = player_3;
        players[PlayerID.PLAYER_4] = player_4;

        playersInput = new Dictionary<PlayerID, PlayerInput>();
        playersInput[PlayerID.PLAYER_1] = new PlayerInput();
        playersInput[PlayerID.PLAYER_2] = new PlayerInput();
        playersInput[PlayerID.PLAYER_3] = new PlayerInput();
        playersInput[PlayerID.PLAYER_4] = new PlayerInput();

        // Input events
        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_JoystickMoved += On_INPUT_JoystickMoved;
        //  Spell events
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }


    private void Update()
    {
        MovePlayers();
    }


    private void On_SPELLS_SpellHitPlayer(HitInfo hitInfo)
    {
        Debug.Log("Spell " + hitInfo.CastedSpell.SpellName + " from player " + hitInfo.CastingPlayerID + " hit player " + hitInfo.HitPlayerID);
    }


    #region Input
    private void On_INPUT_ButtonPressed(InputButton inputButton, PlayerID playerID)
    {
        Debug.Log("Button " + inputButton + " pressed by " + playerID);
        if (playerID == PlayerID.TEST) return;

        switch (inputButton)
        {
            case InputButton.CAST_SPELL_1:
                players[playerID].CastSpell_1();
                break;

            case InputButton.CAST_SPELL_2:
                players[playerID].CastSpell_2();
                break;

            case InputButton.CAST_SPELL_3:
                players[playerID].CastSpell_3();
                break;
        }
    }

    private void On_INPUT_JoystickMoved(InputAxis axisType, float axisValue, PlayerID playerID)
    {
        if (playerID == PlayerID.TEST) return;
        // TODO: i think this needs to be changed because the way it works now the player can only move in one direction at a time!!! he needs to be able to move in both axis at the same time  for fluent movement
        switch (axisType)
        {
            case InputAxis.MOVE_X:
                playersInput[playerID].Move_X = axisValue;
                break;

            case InputAxis.MOVE_Y:
                playersInput[playerID].Move_Y = axisValue;
                break;

            case InputAxis.ROTATE_X:
                playersInput[playerID].Rotate_X = axisValue;
                break;

            case InputAxis.ROTATE_Y:
                playersInput[playerID].Rotate_Y = axisValue;
                break;
        }
    }


    /// <summary>
    /// Connect new player to a free spot
    /// </summary>
    /// <returns> The ID of the connected player </returns>
    public PlayerID ConnectNextPlayerToController()
    {
        if (player_1.IsConnected == false)
        {
            player_1.Connect(PlayerID.PLAYER_1);
            return PlayerID.PLAYER_1;
        }
        else if (player_2.IsConnected == false)
        {
            player_2.Connect(PlayerID.PLAYER_2);
            return PlayerID.PLAYER_2;
        }
        else if (player_3.IsConnected == false)
        {
            player_3.Connect(PlayerID.PLAYER_3);
            return PlayerID.PLAYER_3;
        }
        else if (player_4.IsConnected == false)
        {
            player_4.Connect(PlayerID.PLAYER_4);
            return PlayerID.PLAYER_4;
        }
        else
        {
            Debug.Log("Couldn't connect any new player");
            return 0;
        }
    }

    private void MovePlayers()
    {
        for (int i = 1; i < 5; i++)
        {
            PlayerID playerID = MaleficusTypes.IntToPlayerID(i);
            if (players[playerID].IsConnected)
            {
                PlayerInput playerInput = playersInput[playerID];
                Player player = players[playerID];
                if (playerInput.HasMoved())
                {
                    player.Move(playerInput.Move_X, playerInput.Move_Y);
                }
                if (playerInput.HasRotated())
                {
                    player.Move(playerInput.Rotate_X, playerInput.Rotate_Y);
                }

                playerInput.Flush();
            }
        }
    }
    #endregion


}
