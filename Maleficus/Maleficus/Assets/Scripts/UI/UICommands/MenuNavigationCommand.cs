using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigationCommand : AbstractMenuCommand {

    [SerializeField] private MenuState fromState;
    [SerializeField] private MenuState toState;


    protected override bool ActiveCondition()
    {
        return UIManager.Instance.CurrentState == fromState;
    }

    public override void Execute()
    {
        UIManager.Instance.UpdateState(toState);
    }
}
