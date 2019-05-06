using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigationCommand : AbstractMenuCommand {

    [SerializeField] private MenuState fromState;
    [SerializeField] private MenuState toState;


    public override void Execute()
    {
        UIManager.Instance.UpdateState(toState);
    }
}
