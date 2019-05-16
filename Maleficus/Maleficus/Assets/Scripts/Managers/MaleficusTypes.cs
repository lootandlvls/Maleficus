using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// AppState
public enum AppState
{
    IN_MENU,
    CONNECTING_PLAYERS,
    IN_GAME,
    TEST
}


// Player
public enum PlayerID
{
    PLAYER_1,
    PLAYER_2,
    PLAYER_3,
    PLAYER_4,
    TEST
}


// Input
public enum InputButton
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

public enum InputAxis
{
    MOVE_X,
    MOVE_Y,
    ROTATE_X,
    ROTATE_Y
}


// UI
public enum MenuState
{
    NONE,
    MAIN,
    IN_GAME
}

public enum ButtonDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}




public class MaleficusTypes 
{
  
}
