using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameMode : MonoBehaviour
{
    public EGameMode GameMode { get { return gameMode; } }

    protected EGameMode gameMode;

 
    //public abstract void StartGame();

    //public abstract void StopGame();

    
}
