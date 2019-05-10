using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : Singleton<DebugManager>
{
    private Dictionary<int, DebugText> debugTexts;

    private new void Awake()
    {
        base.Awake();
        debugTexts = new Dictionary<int, DebugText>();
    }


    private void Start()
    {
        DebugText[] foundDebugTexts = FindObjectsOfType<DebugText>();
        foreach (DebugText foundDebugText in foundDebugTexts)
        {
            if (debugTexts.ContainsKey(foundDebugText.DebugID) == false)
            {
                debugTexts.Add(foundDebugText.DebugID, foundDebugText);
            }
            else
            {
                Debug.Log("A DebugText component with ID " + foundDebugText.DebugID + " has already been found in this scene!");
            }
        }
    }

    /// <summary>
    /// Writes a text during this frame on the DebugText in this scene with the given ID. 
    /// Written text is erased on the next frame!
    /// </summary>
    public void Log(int debugID, string debugText)
    {
        if (debugTexts.ContainsKey(debugID) == true)
        {
            debugTexts[debugID].Log(debugText);
        }
        else
        {
            Debug.LogWarning("Debug text with ID " + debugID + " not found in this scene!");
        }
    }
}
