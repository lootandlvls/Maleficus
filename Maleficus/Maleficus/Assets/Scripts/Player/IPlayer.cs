using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    EPlayerID PlayerID { get; }


    Vector3 Position { get; }

    Quaternion Rotation { get; }



    bool IsARPlayer { get; }

}
