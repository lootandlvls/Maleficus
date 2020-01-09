using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using MyBox;

public abstract class MaleficusMonoBehaviour : MonoBehaviour
{
    [Separator("Maleficus MonoBehaviour")]
    [SerializeField] bool showMaleficusMonoBehaviourConfiguration = false;
    [ConditionalField(nameof(showMaleficusMonoBehaviourConfiguration), inverse: false)] [SerializeField] bool debugLogConsoleEnabled = true;
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

    protected virtual void OnValidate()
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
            Debug.Log("<color=gray>[" + name + "]</color> " + logText);
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
            Debug.Log("<color=yellow>Warning! </color>" + "<color=gray>[" + name + "]</color> " + logText);
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
            Debug.Log("<color=red>Error! </color>" + "<color=gray>[" + name + "]</color> " + logText);
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
    #endregion


    #region Checkers

    /// <summary>
    /// Checks if the the given object's reference is null.
    /// Prints a warning in the console if not null.
    /// </summary>
    /// <typeparam name="O"> Type of the object to check</typeparam>
    /// <param name="objectToCheck"> object to check </param>
    /// <returns> True if the given object has a valid reference, otherwise false</returns>
    protected bool IS_NULL<O>(O objectToCheck)
    {
        if (objectToCheck != null)
        {
            LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> isn't null! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the the given object's reference is not null.
    /// Prints a warning in the console if null.
    /// </summary>
    /// <typeparam name="O"> Type of the object to check</typeparam>
    /// <param name="objectToCheck"> object to check </param>
    /// <returns> True if the given object has a valid reference, otherwise false</returns>
    protected bool IS_NOT_NULL<O> (O objectToCheck)
    {
        if (objectToCheck == null)
        {
            LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is null! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given enum has the value NONE. 
    /// Check is performed by a simple string conversion!
    /// Prints a warning in the console if not NONE.
    /// </summary>
    /// <typeparam name="E"> Type of the enum to check </typeparam>
    /// <param name="enumToCheck"> enum to check </param>
    /// <returns></returns>
    protected bool IS_NONE<E> (E enumToCheck) where E : Enum
    {
        if (enumToCheck.ToString() != "NONE")
        {
            LogConsoleWarning("An enum of type <color=cyan>" + typeof(E) + "</color> isn't NONE! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given enum has NOT the value NONE. 
    /// Check is performed by a simple string conversion!
    /// Prints a warning in the console if NONE.
    /// </summary>
    /// <typeparam name="E"> Type of the enum to check </typeparam>
    /// <param name="enumToCheck"> enum to check </param>
    /// <returns></returns>
    protected bool IS_NOT_NONE<E> (E enumToCheck) where E : Enum
    {
        if (enumToCheck.ToString() == "NONE")
        {
                LogConsoleWarning("An enum of type <color=cyan>" + typeof(E) + "</color> is NONE! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the 2 given enums are equal.
    /// Prints a warning in the console if different.
    /// </summary>
    /// <typeparam name="E"> Type of the enum to check </typeparam>
    /// <param name="enumToCheck1"> first enum to check </param>
    /// <param name="enumToCheck2"> second enum to check with </param>
    /// <returns></returns>
    protected bool ARE_EQUAL<E>(E enumToCheck1, E enumToCheck2) where E : Enum
    {
        if (enumToCheck1.ToString().Equals(enumToCheck2.ToString()) == false)
        {
            LogConsoleWarning("Two enums of type <color=cyan>" + typeof(E) + "</color> are different! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the 2 given enums are not equal.
    /// Prints a warning in the console if not different.
    /// </summary>
    /// <typeparam name="E"> Type of the enum to check </typeparam>
    /// <param name="enumToCheck1"> first enum to check </param>
    /// <param name="enumToCheck2"> second enum to check with </param>
    /// <returns></returns>
    protected bool ARE_NOT_EQUAL<E>(E enumToCheck1, E enumToCheck2) where E : Enum
    {
        if (enumToCheck1.ToString().Equals(enumToCheck2.ToString()) == true)
        {
            LogConsoleWarning("Two enums of type <color=cyan>" + typeof(E) + "</color> are NOT different! ");
            return false;
        }
        return true;
    }



    /// <summary>
    /// Checks if the given dictionary contains the given key.
    /// Prints a warning in the console if key is not found.
    /// </summary>
    /// <typeparam name="K"> type of the key </typeparam>
    /// <typeparam name="V"> type of the value </typeparam>
    /// <param name="dictionary"> dictionary to check into </param>
    /// <param name="key"> key to check inside the dictionary </param>
    /// <returns></returns>
    protected bool IS_KEY_CONTAINED<K, V> (Dictionary<K, V> dictionary, K key)
    {
        if (dictionary.ContainsKey(key) == false)
        {
            LogConsoleWarning("A dictionary with keys type <color=cyan>" + typeof(K) + "</color> doens't contain the key '" + key + "' ! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given dictionary doesn't contains the given key.
    /// Prints a warning in the console if key is found.
    /// </summary>
    /// <typeparam name="K"> type of the key </typeparam>
    /// <typeparam name="V"> type of the value </typeparam>
    /// <param name="dictionary"> dictionary to check into </param>
    /// <param name="key"> key to check inside the dictionary </param>
    /// <returns></returns>
    protected bool IS_KEY_NOT_CONTAINED<K, V> (Dictionary<K, V> dictionary, K key)
    {
        if (dictionary.ContainsKey(key) == true)
        {
            LogConsoleWarning("A dictionary with keys type <color=cyan>" + typeof(K) + "</color> does already contain the key '" + key + "' ! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given dictionary contains the given value.
    /// Prints a warning in the console if value is not found.
    /// </summary>
    /// <typeparam name="K"> type of the key </typeparam>
    /// <typeparam name="V"> type of the value </typeparam>
    /// <param name="dictionary"> dictionary to check into </param>
    /// <param name="value"> value to check inside the dictionary </param>
    /// <returns></returns>
    protected bool IS_VALUE_CONTAINED<K, V> (Dictionary<K, V> dictionary, V value)
    {
        if (dictionary.ContainsValue(value) == false)
        {
            LogConsoleWarning("A dictionary with values type <color=cyan>" + typeof(V) + "</color> doens't contain the value '" + value + "' ! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given dictionary doesn't contain the given value.
    /// Prints a warning in the console if value is found.
    /// </summary>
    /// <typeparam name="K"> type of the key </typeparam>
    /// <typeparam name="V"> type of the value </typeparam>
    /// <param name="dictionary"> dictionary to check into </param>
    /// <param name="value"> value to check inside the dictionary </param>
    /// <returns></returns>
    protected bool IS_VALUE_NOT_CONTAINED<K, V> (Dictionary<K, V> dictionary, V value)
    {
        if (dictionary.ContainsValue(value) == true)
        {
            LogConsoleWarning("A dictionary with values type <color=cyan>" + typeof(V) + "</color> does contain already the value '" + value + "' ! ");
            return false;
        }
        return true;
    }



    /// <summary>
    /// Checks if the given list contains the given value.
    /// Prints a warning in the console if value is not found.
    /// </summary>
    /// <typeparam name="V"> type of the value </typeparam>
    /// <param name="list"> list to check into </param>
    /// <param name="value"> value to check inside the list </param>
    /// <returns></returns>
    protected bool IS_VALUE_CONTAINED<V> (List<V> list, V value)
    {
        if (list.Contains(value) == false)
        {
            LogConsoleWarning("A list with valuess type <color=cyan>" + typeof(V) + "</color> doens't contain the value '" + value + "' ! ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given list doesn't contain the given value.
    /// Prints a warning in the console if value is found.
    /// </summary>
    /// <typeparam name="V"> type of the value </typeparam>
    /// <param name="list"> list to check into </param>
    /// <param name="value"> value to check inside the list </param>
    /// <returns></returns>
    protected bool IS_VALUE_NOT_CONTAINED<V> (List<V> list, V value)
    {
        if (list.Contains(value) == true)
        {
            LogConsoleWarning("A list with valuess type <color=cyan>" + typeof(V) + "</color> does contain already the value '" + value + "' ! ");
            return false;
        }
        return true;
    }




    #endregion

    #region Coroutine
    /// <summary>
    /// Starts a couroutine and store it in the given enumerator. If the enumerator is already running a coroutine, then stop it and start a new one.
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
    /// Stops a couroutine if already running.
    /// </summary>
    /// <param name="enumerator"> where the coroutine reference will be stored (define as a member in your class) </param>
    /// <returns> True if the coroutine was running and got stopped, otherwise false </returns>
    protected bool StopCoroutineIfRunning(IEnumerator enumerator)
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
            return true;
        }
        return false;
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
            LogConsoleWarning("Component of type <color=cyan>" + typeof(T) + "</color> not found on '" + name + "'");
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
            LogConsoleWarning("Not any component of type <color=cyan>" + typeof(T) + "</color> not found on '" + name + "'");
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
