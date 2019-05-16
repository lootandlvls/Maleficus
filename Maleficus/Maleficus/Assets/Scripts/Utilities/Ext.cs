using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ext 
{
    public static bool ContainedIn<T>(this T val, params T[] values) where T : struct
    {
        bool found = false;
        foreach (T t in values)
        {
            if (t.Equals(val))
            {
                found = true;
            }
        }
        return found;
    }


}
