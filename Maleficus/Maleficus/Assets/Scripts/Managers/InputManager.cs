using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Player Player_1 { get { return player_1; } }
    public Player Player_2 { get { return player_2; } }
    public Player Player_3 { get { return player_3; } }
    public Player Player_4 { get { return player_4; } }

    [SerializeField] private Player player_1;
    [SerializeField] private Player player_2;
    [SerializeField] private Player player_3;
    [SerializeField] private Player player_4;

    private Dictionary<char, Player> playerMapping;

    protected override void Awake()
    {
        base.Awake();
        playerMapping = new Dictionary<char, Player>();
    }


    private void Update()
    {


        // Spells
        CheckAndCallSpell(1, 'A');
        CheckAndCallSpell(1, 'B');
        CheckAndCallSpell(1, 'C');
        CheckAndCallSpell(1, 'D');

        //CheckAndCallSpell(2, 'A');
        //CheckAndCallSpell(2, 'B');
        //CheckAndCallSpell(2, 'C');
        //CheckAndCallSpell(2, 'D');

        //CheckAndCallSpell(3, 'A');
        //CheckAndCallSpell(3, 'B');
        //CheckAndCallSpell(3, 'C');
        //CheckAndCallSpell(3, 'D');

        // Confirm
        CheckConfirmButton('A');
        CheckConfirmButton('B');
        CheckConfirmButton('C');
        CheckConfirmButton('D');

    }

    private void CheckAndCallSpell(int spellID, char controllerID)
    {
        if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerID))
        {
            if (playerMapping.ContainsKey(controllerID) == true)
            {
                Player player = playerMapping[controllerID];
                if (spellID == 1)
                {
                    player.CastSpell_1();
                }
                else if (spellID == 2)
                {
                    player.CastSpell_2();
                }
                else if (spellID == 3)
                {
                    player.CastSpell_3();
                }
            }
        }
    }

    private void CheckConfirmButton(char controllerID)
    {
        if (Input.GetButtonDown("Confirm_" + controllerID))
        {
            if (playerMapping.ContainsKey(controllerID) == false)
            {
                MapNextPlayer(controllerID);
            }
        }
    }

    private void MapNextPlayer(char controllerID)
    {
        if (player_1.IsConnected == false)
        {
            player_1.Connect(controllerID);
            playerMapping[controllerID] = player_1;
        }
        else if (player_2.IsConnected == false)
        {
            player_2.Connect(controllerID);
            playerMapping[controllerID] = player_2;
        }
        else if (player_3.IsConnected == false)
        {
            player_3.Connect(controllerID);
            playerMapping[controllerID] = player_3;
        }
        else if (player_4.IsConnected == false)
        {
            player_4.Connect(controllerID);
            playerMapping[controllerID] = player_4;
        }
    }

    

}
