using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CurveTest : BNJMOBehaviour
{
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve hightCurve;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private float animationLength = 2;
    [SerializeField] private float hightFactor = 2;

    public void PlayAnimation1()
    {
        StartCoroutine(Animation1Coroutine());
    }

    private IEnumerator Animation1Coroutine()
    {
        float startTime = Time.time;
        float progressionPercentage = 0.0f;
        while (progressionPercentage < 1.0f)
        {
            progressionPercentage = (Time.time - startTime) / animationLength;
            progressionPercentage = Mathf.Clamp(progressionPercentage, 0.0f, 1.0f);

            float speedAlpha = speedCurve.Evaluate(progressionPercentage);
            float hightAlpha = hightCurve.Evaluate(progressionPercentage);

            Vector3 newPosition = Vector3.Lerp(startTransform.position, endTransform.position, speedAlpha);
            newPosition = new Vector3
            {
                x = newPosition.x,
                y = newPosition.y + hightAlpha * hightFactor,
                z = newPosition.z
            };
            transform.position = newPosition;

            transform.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, speedAlpha);

            yield return new WaitForEndOfFrame();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CurveTest))]
public class CurveTestEditr : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CurveTest curveTest = (CurveTest)target;

        if (GUILayout.Button("Play Animation 1"))
        {
            curveTest.PlayAnimation1();
        }
    }
}
#endif