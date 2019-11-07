using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerMovementRepresentation : MaleficusMonoBehaviour
{
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }

    private TrailRenderer myTrailRenderer;


    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myTrailRenderer = FindComponent<TrailRenderer>();
    }

    public void ClearTrailRenderer()
    {
        myTrailRenderer.Clear();
    }

}
