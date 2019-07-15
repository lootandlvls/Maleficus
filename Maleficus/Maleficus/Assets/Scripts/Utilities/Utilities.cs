using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class Utilities : MonoBehaviour
{
    /// <summary>
    /// Gets a random index for a given array
    /// </summary>
    public static int GetRndIndex(int arrayLength)
    {
        return Random.Range(0, arrayLength);
    }

    public static Quaternion GetRndStandRotation()
    {
        return Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
    }
}
