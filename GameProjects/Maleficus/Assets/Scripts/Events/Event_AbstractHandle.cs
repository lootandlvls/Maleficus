using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract event handle with 1 parameter
/// </summary>
public class Event_AbstractHandle<A> : AbstractEventHandle
{
    public A Arg1 { get; }

    public Event_AbstractHandle(A arg1)
    {
        ID = ENetMessageID.NONE;

        Arg1 = arg1;
    }

    public override string GetDebugMessage()
    {
        return Arg1.ToString();
    }
}

/// <summary>
/// An abstract event handle with 2 parameters
/// </summary>
public class Event_AbstractHandle<A, B> : AbstractEventHandle
{
    public A Arg1 { get; }
    public B Arg2 { get; }

    public Event_AbstractHandle(A arg1, B arg2)
    {
        ID = ENetMessageID.NONE;

        Arg1 = arg1;
        Arg2 = arg2;
    }

    public override string GetDebugMessage()
    {
        return Arg1.ToString() + " | " + Arg2.ToString();
    }
}

/// <summary>
/// An abstract event handle with 3 parameters
/// </summary>
public class Event_AbstractHandle<A, B, C> : AbstractEventHandle
{
    public A Arg1 { get; }
    public B Arg2 { get; }
    public C Arg3 { get; }

    public Event_AbstractHandle(A arg1, B arg2, C arg3)
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
}

/// <summary>
/// An abstract event handle with 4 parameters
/// </summary>
public class Event_AbstractHandle<A, B, C, D> : AbstractEventHandle
{
    public A Arg1 { get; }
    public B Arg2 { get; }
    public C Arg3 { get; }
    public D Arg4 { get; }

    public Event_AbstractHandle(A arg1, B arg2, C arg3, D arg4)
    {
        ID = ENetMessageID.NONE;

        Arg1 = arg1;
        Arg2 = arg2;
        Arg3 = arg3;
        Arg4 = arg4;
    }

    public override string GetDebugMessage()
    {
        return Arg1.ToString() + " | " + Arg2.ToString() + " | " + Arg3.ToString() + " | " + Arg4.ToString();
    }
}
