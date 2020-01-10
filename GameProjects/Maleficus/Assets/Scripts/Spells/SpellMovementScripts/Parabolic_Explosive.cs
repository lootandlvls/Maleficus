using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabolic_Explosive : AbstractSpell
{

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve heightCurve;
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

    protected override void Start()
    {
        base.Start();

        endPosition = new Vector3(transform.position.x , transform.position.y , transform.position.z + 8);
        endRotation = endTransform.rotation; 
        startTransform = this.transform;
        startRotation = startTransform.rotation;
        startPosition = startTransform.position;
        Debug.Log(startTransform.position);

    }

    protected override void Update()
    {
        base.Update();

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
                onExplosionEnter(transform.position, 5);
               

                DestroySpell();
                return;
            }

            if ((IS_NOT_NULL(speedCurve))
                && (IS_NOT_NULL(heightCurve)))

            {
                float speedAlpha = speedCurve.Evaluate(percentageProgress);
                float heightAlpha = speedCurve.Evaluate(speedAlpha);

                transform.position = Vector3.Lerp(startPosition, endPosition, speedAlpha);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, speedAlpha);

                transform.position = new Vector3(transform.position.x, heightAlpha * heightFactor, transform.position.z);
            }
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

    void onExplosionEnter(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        List<IPlayer> hitPlayers = new List<IPlayer>();
        while (i < hitColliders.Length)
        {
            if ((hitColliders[i] != null) && hitColliders[i].tag == "Player")
            {
                IPlayer otherPlayer = hitColliders[i].gameObject.GetComponent<IPlayer>();
                AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();

                if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                {

                    hitPlayers.Add(otherPlayer);

                }
            }

            i++;
        }
        ExplosionProcessHits(hitPlayers.ToArray());

    }
}
