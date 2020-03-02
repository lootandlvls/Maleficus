using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : BNJMOBehaviour
{
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.GAME_IntroFinished += On_GAME_IntroFinished;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.GAME_IntroFinished -= On_GAME_IntroFinished;
        }
    }

    private void On_GAME_IntroFinished(bool obj)
    {
        Debug.Log("INTRO FINISHED");
        Destroy(this.gameObject);
    }
}
