using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : BNJMOBehaviour
{
    public float ShrinkingRate { get { return shrinkingRate; } }
    public float ShrinkingDuration { get { return shrinkingDuration; } }
    public float ShrinkingLimit { get { return shrinkingLimit; } }
    public float ArenaRadius { get; private set; }

    [Header("Shrinking")]
    [SerializeField] float shrinkingRate;
    [SerializeField] float shrinkingLimit;
    [SerializeField] float shrinkingDuration;

    private ArenaLimit myArenaLimit;

    private Vector2 currentScale;


    protected override void Awake()
    {
        base.Awake();

        currentScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.GAME_GameTimeUpdated += On_GAME_GameTimeUpdated;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myArenaLimit = GetComponentInChildren<ArenaLimit>();
        if (IS_NOT_NULL(myArenaLimit))
        {
            ArenaRadius = Mathf.Abs((myArenaLimit.Position - transform.position).magnitude);
        }
    }

    /// <summary>
    /// Transforms the position of the given object in local normalized Arena coordinate realtive to its size
    /// </summary>
    public Vector2 GetInArenaPosition(Vector3 position)
    {
        Vector2 player2DPosition = new Vector2(position.x, position.z);
        Vector2 arena2DPosition = new Vector2(transform.position.x, transform.position.z);
        return (player2DPosition - arena2DPosition) / ArenaRadius;
    }

    private void On_GAME_GameTimeUpdated(int countdown)
    {
       // LogConsole("Time Updated");
        if (countdown == 100 || countdown == 50)
        {
          //  LogConsole("Starting to shrink");
            StartToShrink();
        }
    }

    private void StartToShrink()
    {
        StartCoroutine(ShrinkCourotine());
    }

    private IEnumerator ShrinkCourotine()
    {
        float startTime = Time.time;
        while (Time.time - startTime <= ShrinkingDuration)
        {
            if(currentScale.x >= 0 && currentScale.y >= 0)
            {
                currentScale.x -= ShrinkingRate;
                currentScale.y -= ShrinkingRate;
                transform.localScale = new Vector3(currentScale.x, currentScale.y, transform.localScale.z);

            }

            yield return new WaitForEndOfFrame();
        }
      
    }
  
}
