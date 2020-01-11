using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public float ShrinkingRate { get { return shrinkingRate; } }
    public float ShrinkingDuration { get { return shrinkingDuration; } }
    public float MaxLimit { get { return maxLimit; } }

    [SerializeField] float shrinkingRate;

    [SerializeField] float maxLimit;
    [SerializeField] float shrinkingDuration;

    private Vector2 currentSize;
    private Transform ArenaTransform;
    private bool IsReadyToshrink;

    // Start is called before the first frame update
    void Start()
    {
        ArenaTransform = GetComponent<Transform>();
        currentSize = new Vector2(transform.localScale.x, transform.localScale.y);


    }


    public void StartToShrink()
    {
        StartCoroutine(ShrinkCourotine());
    }


    private IEnumerator ShrinkCourotine()
    {
        float startTime = Time.time;
        while (Time.time - startTime <= ShrinkingDuration)
        {
            if(currentSize.x >= 0 && currentSize.y >= 0)
            {
                currentSize.x -= ShrinkingRate;
                currentSize.y -= ShrinkingRate;
                ArenaTransform.localScale = new Vector3(currentSize.x, currentSize.y, transform.localScale.z);

            }

            yield return new WaitForEndOfFrame();
        }
      
    }
  
}
