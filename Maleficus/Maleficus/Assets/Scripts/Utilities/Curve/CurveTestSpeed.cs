using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTestSpeed : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float heightFactor = 1.0f;

    private bool isStarted = false;

    private float movementDuration;
    private float startTime;

    private float timeProgress;
    private float percentageProgress;


    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            StartMovement();
        }

        if (isStarted == true)
        {
            timeProgress = Time.time - startTime;
            percentageProgress = timeProgress / movementDuration;

            if (percentageProgress >= 1.0f)
            {
                isStarted = false;
                return;
            }

            float curveValue = CurveReader.Instance.EvaluateCurve(3, percentageProgress);
            float heightValue = CurveReader.Instance.EvaluateCurve(1, percentageProgress);

    
            transform.position = Vector3.Lerp(startTransform.position, endTransform.position, curveValue);
            transform.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, curveValue);

            transform.position = new Vector3(transform.position.x, heightValue * heightFactor, transform.position.z);
        }


    }

    private void StartMovement()
    {
        isStarted = true;
        startTime = Time.time;

        float trajectoryDistance = Vector3.Distance(startTransform.position, endTransform.position);
        movementDuration = trajectoryDistance / movementSpeed;

        transform.position = startTransform.position;
        transform.rotation = endTransform.rotation;
    }
}
