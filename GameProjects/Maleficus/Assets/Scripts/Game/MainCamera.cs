using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : BNJMOBehaviour
{
    

    [SerializeField] float IntroDuration;
  
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(IntroCoroutine(IntroDuration));
    }

    private IEnumerator IntroCoroutine(float introDuration)
    {
        yield return new WaitForSeconds(introDuration);
        EventManager.Instance.Invoke_GAME_IntroFinished(true);
    }
}
