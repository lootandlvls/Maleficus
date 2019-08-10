using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventInvokerAction : AbstractUIAction
{
    private static int counter = 0;

    public override void Execute()
    {
        base.Execute();

        counter++;
        EventManager.Instance.TEST_TestEvent.Invoke(new TestEventHandle("Event invoked : " + counter));
    }
}
