using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;



public class MaleficusMonoBehaviour : MonoBehaviour
{
    [Separator("Maleficus MonoBehaviour")]
    [SerializeField] bool printDebugToConsole = true;

    protected virtual void Awake()
    {
        InitializeObjecsInScene();

    }

    /// <summary>
    /// Use this to find objects in the scene in order to assign references or bind them to callbacks (actually called in Awake)
    /// </summary>
    protected virtual void InitializeObjecsInScene()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void Start()
    {
        InitializeEventsCallbacks();

        StartCoroutine(LateStartCoroutine());
    }

    /// <summary>
    /// Use this method to listen to events and add their callbacks here (actually called in Start)
    /// </summary>
    protected virtual void InitializeEventsCallbacks()
    {

    }

    protected virtual void LateStart()
    {

    }

    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();

        LateStart();
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    protected void PrintToConsole(string logText)
    {
        if (printDebugToConsole == true)
        {
            Debug.Log(logText);
        }
    }

    protected void PrintToCanvas(string logText, int debugID = 0)
    {
        DebugManager.Instance.Log(debugID, logText);
    }

    /// <summary>
    /// Starts a couritine and store it in the given enumerator. If the enumerator is already running a coroutine, then stop it and start a new one.
    /// </summary>
    /// <param name="enumerator"> where the coroutine reference will be stored (define as a member in your class) </param>
    /// <param name="coroutine"> the coroutine name function to run + () with parameters if defined </param>
    protected void StartNewCoroutine(ref IEnumerator enumerator, IEnumerator coroutine)
    {
        // Stop running coroutine
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }
        // Assign reference
        enumerator = coroutine;
        // Start new coroutine
        StartCoroutine(enumerator);
    }
}
