using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigationCommand : AbstractMenuCommand {

    [SerializeField] private EMenuState fromState;
    [SerializeField] private EMenuState toState;


    public override void Execute()
    {
        UIManager.Instance.UpdateState(toState);
    }
}
