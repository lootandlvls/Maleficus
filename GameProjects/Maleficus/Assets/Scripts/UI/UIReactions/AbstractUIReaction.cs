using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusConsts;


/// <summary>
/// UI element that reacts to UI state change.
/// </summary>
public abstract class AbstractUIReaction : MonoBehaviour {

    [SerializeField] protected bool isInitializeOnStart = true;
    [SerializeField] protected EMenuState activeOnState = EMenuState.NONE;

    protected EMenuState[] activeOnStates;

    private void Awake()
    {
        // Add all sub states if selected state is a high hierarchy 
        List<EMenuState> temp = new List<EMenuState>();
        switch (activeOnState)
        {
            case EMenuState.IN_ENTRY_IN_LOGIN:
                temp = new List<EMenuState>(MENU_STATES_IN_LOGIN);
                break;
      
        }
        if (temp.Contains(activeOnState) == false)
        {
            temp.Add(activeOnState);
        }
        activeOnStates = temp.ToArray();
    }

    private void Start()
    {
        EventManager.Instance.UI_MenuStateUpdated.AddListener(On_UI_MenuStateUpdated);

        // Initialization
        if (isInitializeOnStart == true)
        {
            PlayAppropriateReaction(UIManager.Instance.CurrentState);
        }
    }

    protected virtual void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UI_MenuStateUpdated.RemoveListener(On_UI_MenuStateUpdated);
        }
    }

    protected virtual void On_UI_MenuStateUpdated(Event_StateUpdated<EMenuState> eventHandle)
    {
        PlayAppropriateReaction(eventHandle.NewState);
    }

    protected abstract void PlayAppropriateReaction(EMenuState newState);


}
