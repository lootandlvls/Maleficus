using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalSprite : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void ShowSprite()
    {
        mySpriteRenderer.enabled = true;
    }

    public virtual void HideSprite()
    {
        mySpriteRenderer.enabled = false;
    }


}
