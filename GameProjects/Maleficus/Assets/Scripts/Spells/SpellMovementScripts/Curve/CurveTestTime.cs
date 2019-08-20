using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTestTime : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private float movementDuratoin;
    [SerializeField] private float heightFactor = 1;

    private float startTime;
    private bool isStarted = false;

    private float timeProgress;
    private float percentageProgress;


    private void Start()
    {
        startTransform.position = transform.position ;
        AbstractSpell abstracSpell = this.GetComponent<AbstractSpell>();
        //endTransform = abstracSpell.parabolicSpell_EndPosition;
    }
    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            StartMovement();
        }

        if (isStarted == true)
        {
            timeProgress = Time.time - startTime;
            percentageProgress = timeProgress / movementDuratoin;

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

        transform.position = startTransform.position;
        transform.rotation = endTransform.rotation;
    }
}
