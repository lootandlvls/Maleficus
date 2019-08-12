using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotProductTest : MonoBehaviour
{
    [SerializeField] private Transform other;

    private void Update()
    {
        Vector2 coordinateForward = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 coordinateRight = new Vector2(transform.right.x, transform.right.z);
        Vector2 objectForward = new Vector2(other.transform.forward.x, other.transform.forward.z);
        float dotWithRight = Vector2.Dot(coordinateRight, objectForward);
        int sign;
        if (dotWithRight > 0.0f)
        {
            sign = 1; 
        }
        else if (dotWithRight < 0.0f)
        {
            sign = -1;
        }
        else
        {
            sign = 0;
        }
        float angle = Mathf.Acos(Vector2.Dot(coordinateForward, objectForward)) * sign;
        DebugManager.Instance.Log(69, "Dot : " + Mathf.Cos(angle));
    }
}
