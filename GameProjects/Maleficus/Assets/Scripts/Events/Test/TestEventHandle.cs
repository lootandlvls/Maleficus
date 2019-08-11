using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventHandle : AbstractEventHandle
{
    public string TestMessage { get; }

    public TestEventHandle(string text)
    {
        TestMessage = text;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        
        return new Net_Test(TestMessage);
    }

    public override string GetDebugMessage()
    {
        return TestMessage;
    }
}

[System.Serializable]
public class Net_Test : AbstractNetMessage
{
    public string TestMessage { get; }

    public Net_Test(string testMessage)
    {
        ID = NetID.Test;
        TestMessage = testMessage;
    }
}

