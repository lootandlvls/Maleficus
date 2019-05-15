using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerID
{
    PLAYER_1,
    PLAYER_2,
    PLAYER_3,
    PLAYER_4,
    TEST
}

public class PlayerManager : Singleton<PlayerManager>
{


    [SerializeField] private Player player_1;
    [SerializeField] private Player player_2;
    [SerializeField] private Player player_3;
    [SerializeField] private Player player_4;

    Dictionary<PlayerID, Player> players;


    private void Start()
    {
        players = new Dictionary<PlayerID, Player>();
        players[PlayerID.PLAYER_1] = player_1;
        players[PlayerID.PLAYER_2] = player_2;
        players[PlayerID.PLAYER_3] = player_3;
        players[PlayerID.PLAYER_4] = player_4;
         
        // Input events
        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_JoystickMoved += On_INPUT_JoystickMoved;
        //  Spell events
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }

    private void On_SPELLS_SpellHitPlayer(HitInfo hitInfo)
    {
        
    }

    private void On_INPUT_ButtonPressed(InputButton inputButton, PlayerID playerID)
    {
        Debug.Log("Button " + inputButton + " pressed by " + playerID);
        if (playerID == PlayerID.TEST) return;

        switch(inputButton)
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
        switch(axisType)
        {
            case InputAxis.MOVE_X:
                
                players[playerID].Move(axisValue, 0.0f);
                break;

            case InputAxis.MOVE_Y:
                players[playerID].Move(0.0f, axisValue);
                break;
            case InputAxis.ROTATE_X:
                players[playerID].Rotate(axisValue, 0.0f);
                break;
            case InputAxis.ROTATE_Y:
                players[playerID].Rotate(0.0f, axisValue);
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


    //public static int GetPlayerID(PlayerID playerID)
    //{
    //    int id = 0;
    //    switch(playerID)
    //    {
    //        case PlayerID.PLAYER_1:
    //            id = 1;
    //            break;
    //        case PlayerID.PLAYER_2:
    //            id = 2;
    //            break;
    //        case PlayerID.PLAYER_3:
    //            id = 3;
    //            break;
    //        case PlayerID.PLAYER_4:
    //            id = 4;
    //            break;
    //    }
    //    return id;
    //}
}
