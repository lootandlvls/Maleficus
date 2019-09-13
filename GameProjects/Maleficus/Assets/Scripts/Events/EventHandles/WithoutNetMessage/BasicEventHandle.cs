using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEventHandle<A> : AbstractEventHandle
{
    public A Arg1 { get; }

    public BasicEventHandle(A arg1)
    {
        ID = ENetMessageID.NONE;

        Arg1 = arg1;
    }

    public override string GetDebugMessage()
    {
        return Arg1.ToString();
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_NONE();
    //}
}

public class BasicEventHandle<A, B> : AbstractEventHandle
{
    public A Arg1 { get; }
    public B Arg2 { get; }

    public BasicEventHandle(A arg1, B arg2)
    {
        ID = ENetMessageID.NONE;

        Arg1 = arg1;
        Arg2 = arg2;
    }

    public override string GetDebugMessage()
    {
        return Arg1.ToString() + " | " + Arg2.ToString();
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_NONE();
    //}
}

public class BasicEventHandle<A, B, C> : AbstractEventHandle
{
    public A Arg1 { get; }
    public B Arg2 { get; }
    public C Arg3 { get; }

    public BasicEventHandle(A arg1, B arg2, C arg3)
    {
        ID = ENetMessageID.NONE;

        Arg1 = arg1;
        Arg2 = arg2;
        Arg3 = arg3;
    }

    public override string GetDebugMessage()
    {
        return Arg1.ToString() + " | " + Arg2.ToString() + " | " + Arg3.ToString();
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_NONE();
    //}
}
