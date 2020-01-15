using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerRespawnGhost : BNJMOBehaviour
{
    public event Action<PlayerRespawnGhost> RespawnAnimationDone;

    public EPlayerID PlayerID { get; set; } = EPlayerID.NONE;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve deviationCurve;
    [SerializeField] private float animationLength_FirstSection = 3.0f;
    [SerializeField] private float animationLength_Secondsection = 2.0f;
    [SerializeField] private float heightFromSpawnPosition = 5.0f;
    [SerializeField] private float deviationFactor = 7.0f;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject chanelingEffect;

    private Vector3 elevatedEndPosition;

    protected override void Awake()
    {
        base.Awake();

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public void DestroyGhost()
    {
        Destroy(gameObject);
    }

    public void StartRespawnAnimation(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation)
    {
        StartCoroutine(PlayFirstSectionAnimationCoroutine(startPosition, startRotation, endPosition, endRotation));
    }

    private IEnumerator PlayFirstSectionAnimationCoroutine(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation)
    {
        elevatedEndPosition = new Vector3
        {
            x = endPosition.x,
            y = endPosition.y + heightFromSpawnPosition,
            z = endPosition.z,
        };

        float startTime = Time.time;
        float progressionPercentage = 0.0f;
        while (progressionPercentage < 1.0f)
        {
            progressionPercentage = (Time.time - startTime) / animationLength_FirstSection;
            progressionPercentage = Mathf.Clamp(progressionPercentage, 0.0f, 1.0f);

            float speedAlpha = speedCurve.Evaluate(progressionPercentage);
            float deviationAlpha = deviationCurve.Evaluate(speedAlpha);


            Vector3 newPosition = Vector3.Lerp(startPosition, elevatedEndPosition, speedAlpha);
            newPosition += Vector3.up * deviationFactor * deviationAlpha;

            transform.position = newPosition;

            transform.rotation = Quaternion.Lerp(startRotation, endRotation, speedAlpha);

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(PlaySecondSectionAnimationCoroutine(transform.position, transform.rotation, endPosition, endRotation));
    }

    private IEnumerator PlaySecondSectionAnimationCoroutine(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation)
    {
        float startTime = Time.time;
        float progressionPercentage = 0.0f;
        while (progressionPercentage < 1.0f)
        {
            progressionPercentage = (Time.time - startTime) / animationLength_Secondsection;
            progressionPercentage = Mathf.Clamp(progressionPercentage, 0.0f, 1.0f);

            float speedAlpha = speedCurve.Evaluate(progressionPercentage);

            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, speedAlpha);
            transform.position = newPosition;

            transform.rotation = Quaternion.Lerp(startRotation, endRotation, speedAlpha);

            yield return new WaitForEndOfFrame();
        }

        InvokeEventIfBound(RespawnAnimationDone, this);
    }

}

[CustomEditor(typeof(PlayerRespawnGhost))]
public class GhostPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerRespawnGhost ghostPlayer = (PlayerRespawnGhost)target;

        if (GUILayout.Button("Play Animation 1"))
        {
            ghostPlayer.StartRespawnAnimation(Vector3.zero, Quaternion.identity, Vector3.zero, Quaternion.identity);
        }
    }
}
