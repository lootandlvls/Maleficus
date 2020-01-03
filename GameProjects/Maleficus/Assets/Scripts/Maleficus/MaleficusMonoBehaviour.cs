using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using MyBox;

using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;
using System.Linq.Expressions;

public abstract class MaleficusMonoBehaviour : MonoBehaviour
{
    [Separator("Maleficus MonoBehaviour")]
    [SerializeField] bool showMaleficusMonoBehaviourConfiguration = false;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] bool debugLogConsoleEnabled = true;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] bool logWarningsAsErrors = true;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] List<string> logCategoriesToIgnore;


    #region Life Cycle
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
    #endregion

    #region Debug Log
    /// <summary>
    /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
    /// </summary>
    /// <param name="logText"> Log text to print </param>
    /// <param name="category"> Category of the log text </param>
    protected void LogConsole(string logText, string category = "Default")
    {
        if ((debugLogConsoleEnabled == true) && (logCategoriesToIgnore.Contains(category) == false))
        {
            Debug.Log(logText);
        }
    }

    /// <summary>
    /// Prints a warning log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
    /// </summary>
    /// <param name="logText"> Log text to print </param>
    /// <param name="category"> Category of the log text </param>
    protected void LogConsoleWarning(string logText, string category = "Default")
    {
        if ((debugLogConsoleEnabled == true) && (logCategoriesToIgnore.Contains(category) == false))
        {
            Debug.Log("< color = yellow >Warning! </color>" + logText);
        }
    }

      /// <summary>
    /// Prints an error log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
    /// </summary>
    /// <param name="logText"> Log text to print </param>
    /// <param name="category"> Category of the log text </param>
    protected void LogConsoleError(string logText, string category = "Default")
    {
        if ((debugLogConsoleEnabled == true) && (logCategoriesToIgnore.Contains(category) == false))
        {
            Debug.Log("< color = red >Error! </color>" + logText);
        }
    }

    /// <summary>
    /// Prints a log text into a Debug Text component on the canvas in the scene, with a matching debug ID
    /// </summary>
    /// <param name="debugID"> Debug ID of the Debug Text component </param>
    /// <param name="logText"> Log text to print </param>
    protected void LogCanvas(int debugID, string logText)
    {
        DebugManager.Instance.Log(debugID, logText);
    }

    /// <summary>
    /// Checks if the validity of the given object's reference
    /// </summary>
    /// <typeparam name="O"> Type of the object to check</typeparam>
    /// <param name="objectToCheck"> object to check </param>
    /// <returns> True if the given object has a valid reference, otherwise false</returns>
    protected bool IS_VALID<O> (O objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.LogError("An object of type <color=red>" + typeof(O) + "</color> is null! ");
            return false;
        }
        return true;
    }


    protected bool IS_NOT_NONE<E> (E enumToCheck) where E : Enum
    {
        if (enumToCheck.ToString() == "NONE")
        {
            Debug.LogError("An enum of type <color=red>" + typeof(E) + "</color> is NONE! ");
            return false;
        }
        return true;
    }



    #endregion

    #region Coroutine
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
    #endregion

    #region Get Component
    /// <summary>
    /// Looks for component attached to the same gameobject of the given type and prompt an error message if not found.
    /// </summary>
    /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
    /// <param name="promtWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
    /// <returns> Component found otherwise null </returns>
    public T GetComponentWithCheck<T>(bool promtWarningMessageIfNoneFound = true) where T : Component
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
    public T[] GetComponentsWithCheck<T>(bool promtWarningMessageIfNoneFound = true) where T : Component
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
    #endregion

    #region Event Invoke
    /// <summary>
    /// Invoke an event after checking if it is bound (i.e. different from null)
    /// </summary>
    /// <param name="eventToInvoke"> event to invoke </param>
    /// <returns> true if invoked, otherwise false if not bound </returns>
    protected bool InvokeEventIfBound(Action eventToInvoke)
    {
        if (eventToInvoke != null)
        {
            eventToInvoke.Invoke();
            return true;
        }
        else 
        {
            return false;
        }
    }

    /// <summary>
    /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
    /// </summary>
    /// <typeparam name="A"> First generic parameter type of the event </typeparam>
    /// <param name="eventToInvoke"> event to invoke </param>
    /// <param name="arg1" > First generic parameter of the event </param>
    /// <returns> true if invoked, otherwise false if not bound </returns>
    protected bool InvokeEventIfBound<A>(Action<A> eventToInvoke, A arg1)
    {
        if (eventToInvoke != null)
        {
            eventToInvoke.Invoke(arg1);
            return true;
        }
        else 
        {
            return false;
        }
    }

    /// <summary>
    /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
    /// </summary>
    /// <typeparam name="A"> First generic parameter type of the event </typeparam>
    /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
    /// <param name="eventToInvoke"> event to invoke </param>
    /// <param name="arg1" > First generic parameter of the event </param>
    /// <param name="arg2" > Second generic parameter of the event </param>
    /// <returns> true if invoked, otherwise false if not bound </returns>
    protected bool InvokeEventIfBound<A, B>(Action<A, B> eventToInvoke, A arg1, B arg2)
    {
        if (eventToInvoke != null)
        {
            eventToInvoke.Invoke(arg1, arg2);
            return true;
        }
        else 
        {
            return false;
        }
    }

    /// <summary>
    /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
    /// </summary>
    /// <typeparam name="A"> First generic parameter type of the event </typeparam>
    /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
    /// <typeparam name="C"> Third generic parameter type of the event </typeparam>
    /// <param name="eventToInvoke"> event to invoke </param>
    /// <param name="arg1" > First generic parameter of the event </param>
    /// <param name="arg2" > Second generic parameter of the event </param>
    /// <param name="arg3" > Third generic parameter of the event </param>
    /// <returns> true if invoked, otherwise false if not bound </returns>
    protected bool InvokeEventIfBound<A, B, C>(Action<A, B, C> eventToInvoke, A arg1, B arg2, C arg3)
    {
        if (eventToInvoke != null)
        {
            eventToInvoke.Invoke(arg1, arg2, arg3);
            return true;
        }
        else 
        {
            return false;
        }
    }
    #endregion
}
