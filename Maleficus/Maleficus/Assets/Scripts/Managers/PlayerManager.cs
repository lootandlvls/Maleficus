using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{


    [SerializeField] private Player player_1;
    [SerializeField] private Player player_2;
    [SerializeField] private Player player_3;
    [SerializeField] private Player player_4;

    Dictionary<int, Player> players;


    private void Start()
    {
        players = new Dictionary<int, Player>();
        players[1] = player_1;
        players[2] = player_2;
        players[3] = player_3;
        players[4] = player_4;

        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_JoystickMoved += On_INPUT_JoystickMoved;
    }

    

    private void On_INPUT_ButtonPressed(InputButton inputButton, int playerID)
    {
        if (playerID == 0) return;

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

    private void On_INPUT_JoystickMoved(InputAxis axisType, float axisValue, int playerID)
    {
        if (playerID == 0) return;
        switch(axisType)
        {
            case InputAxis.MOVE_X:
                players[playerID].Move(axisValue, 0.0f);
                break;

            case InputAxis.MOVE_Y:
                players[playerID].Move(0.0f, axisValue);
                break;
        }
    }


    /// <summary>
    /// Connect new player to a free spot
    /// </summary>
    /// <returns> The ID of the connected player </returns>
    public int ConnectNextPlayerToController()
    {
        if (player_1.IsConnected == false)
        {
            player_1.Connect();
            return 1;
        }
        else if (player_2.IsConnected == false)
        {
            player_2.Connect();
            return 2;
        }
        else if (player_3.IsConnected == false)
        {
            player_3.Connect();
            return 3;
        }
        else if (player_4.IsConnected == false)
        {
            player_4.Connect();
            return 4;
        }
        else
        {
            Debug.Log("Couldn't connect any new player");
            return 0;
        }
    }
}
