using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTestSpeed : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Quaternion endRotation;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float heightFactor = 1.0f;

    private bool isStarted = false;

    private float movementDuration;
    private float startTime;

    private float timeProgress;
    private float percentageProgress;
    private bool animationStarted = true;

    private void Start()
    {
        AbstractSpell abstracSpell = this.GetComponent<AbstractSpell>();
        endTransform = abstracSpell.parabolicSpell_EndPosition;
        endPosition = endTransform.position;
        endRotation = endTransform.rotation;
        startTransform = this.transform;
        startRotation = startTransform.rotation;
        startPosition = startTransform.position;
        Debug.Log(startTransform.position);

    }

    private void Update()
    {
        if (animationStarted)
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
                ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
                destroyEffect.DestroySpell();
                return;
            }
            
            float curveValue = CurveReader.Instance.EvaluateCurve(3, percentageProgress);
            float heightValue = CurveReader.Instance.EvaluateCurve(1, percentageProgress);

    
            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, curveValue);

            transform.position = new Vector3(transform.position.x, heightValue * heightFactor, transform.position.z);
        }


    }

    private void StartMovement()
    {
        animationStarted = false;
        isStarted = true;

        startTime = Time.time;

        float trajectoryDistance = Vector3.Distance(startPosition, endPosition);
        movementDuration = trajectoryDistance / movementSpeed;

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
