using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;



public abstract class MaleficusMonoBehaviour : MonoBehaviour
{
    [Separator("Maleficus MonoBehaviour")]
    [SerializeField] bool showMaleficusMonoBehaviourConfiguration = false;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] bool debugLogConsoleEnabled = true;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] bool logWarningsAsErrors = true;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] List<string> logCategoriesToIgnore;

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeObjecsInScene();

    }

    /// <summary>
    /// Use this to find components attached to the same gameobject and in order to assign references or bind them to callbacks (actually called in Awake)
    /// </summary>
    protected virtual void InitializeComponents()
    {

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

    /// <summary>
    /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
    /// </summary>
    /// <param name="logText"> Log text to print </param>
    /// <param name="category"> Category of the log text </param>
    protected void DebugLog(string logText, string category = "Default")
    {
        if ((debugLogConsoleEnabled == true) && (logCategoriesToIgnore.Contains(category) == false))
        {
            Debug.Log(logText);
        }
    }

    /// <summary>
    /// Prints a log text into a Debug Text component on the canvas in the scene, with a matching debug ID
    /// </summary>
    /// <param name="logText"> Log text to print </param>
    /// <param name="debugID"> Debug ID of the Debug Text component </param>
    protected void DebugLogOnCanvas(string logText, int debugID = 0)
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


    /// <summary>
    /// Looks for component attached to the same gameobject of the given type and prompt an error message if not found.
    /// </summary>
    /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
    /// <param name="promtWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
    /// <returns> Component found otherwise null </returns>
    public T FindComponent<T>(bool promtWarningMessageIfNoneFound = true) where T : Component
    {
        T result = GetComponent<T>();
        if ((promtWarningMessageIfNoneFound == true) && (result == null))
        {
            if (logWarningsAsErrors == true)
            {
                Debug.LogError("[WARNING] Component of type '" + result.GetType() + "' not found on '" + name + "'");
            }
            else
            {
                Debug.Log("[WARNING] Component of type '" + result.GetType() + "' not found on '" + name + "'");
            }
        }
        return result;
    }

    /// <summary>
    /// Looks for all components attached to the same gameobject of the given type and prompt an error message if none found.
    /// </summary>
    /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
    /// <param name="promtWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
    /// <returns> Component found otherwise null </returns>
    public T[] FindComponents<T>(bool promtWarningMessageIfNoneFound = true) where T : Component
    {
        T[] result = GetComponents<T>();
        if ((promtWarningMessageIfNoneFound == true) && (result.Length == 0))
        {
            if (logWarningsAsErrors == true)
            {
                Debug.LogError("[WARNING] Not any component of type '" + result.GetType() + "' not found on '" + name + "'");
            }
            else
            {
                Debug.Log("[WARNING] Not any component of type '" + result.GetType() + "' not found on '" + name + "'");
            }
        }
        return result;
    }
}
