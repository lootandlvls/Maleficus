using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventListener : MonoBehaviour
{

    private void Start()
    {
        EventManager.Instance.TEST_TestEvent.AddListener(On_TEST_TestEvent);

    }

    void On_TEST_TestEvent(TestEventHandle eventHandle)
    {
        Debug.Log("Test event broadcasted: " + eventHandle.TestMessage);
        //Net_Test netTest = (Net_Test)eventHandle.GetNetMessage();
        //Debug.Log("Net ID : " + netTest.ID + " | content : " + netTest.TestMessage);
    }

    [ContextMenu("Do Something")]
    void DoSomething()
    {
        Debug.Log("Perform operation");
    }

}
