using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CurveReader : AbstractSingletonManager<CurveReader>
{
    public Vector3 Curve1;
    public Vector3 Curve2;

    [SerializeField] private AnimationClip animationClip1;


    List<AnimationCurve> animationCurves;

    protected override void Awake()
    {
        base.Awake();

        UpdateCurves();
    }

    public override void OnSceneStartReinitialize()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown("u"))
        {
            UpdateCurves();
        }
    }

    private void UpdateCurves()
    {
        // TODO: Find a solution for building on Android 
#if UNITY_EDITOR
        animationCurves = new List<AnimationCurve>();

        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip1);
        foreach (EditorCurveBinding curveBinding in curveBindings)
        {
            AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(animationClip1, curveBinding);
            animationCurves.Add(newAnimationCurve);
        }

        Debug.Log("Curves updated");
#endif
    }

    public float EvaluateCurve(int curveID, float time)
    {
        return animationCurves[curveID].Evaluate(time);
    }
}
