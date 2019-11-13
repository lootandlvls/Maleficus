﻿using System.Collections.Generic;

[System.Serializable]
public class Net_OnRequestGameInfo : AbstractNetMessage
{
    public Net_OnRequestGameInfo()
    {
        MessageType = ENetMessageType.ON_REQUEST_GAME_SESSION_INFO;
    }

    public byte ownPlayerId { set; get; }
    public Local_Account initialiser { set; get; }

   // public EGameMode GameMode { set; get; }

    // TODO [Leon]: add array with connected players


    public Local_Account Player1 { set; get; }
    public Local_Account Player2 { set; get; }
    public Local_Account Player3 { set; get; }
    public Local_Account Player4 { set; get; }
}