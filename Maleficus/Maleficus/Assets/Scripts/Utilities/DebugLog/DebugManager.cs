using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DebugManager : Singleton<DebugManager>
{
    private Dictionary<int, DebugText> debugTexts;
    private Dictionary<int, bool> reportedDebugTexts;

    private new void Awake()
    {
        base.Awake();
        debugTexts = new Dictionary<int, DebugText>();
        reportedDebugTexts = new Dictionary<int, bool>();
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
            if (reportedDebugTexts.ContainsKey(debugID) == false)
            {
                reportedDebugTexts.Add(debugID, true);
                Debug.LogWarning("Debug text with ID " + debugID + " not found in this scene!");
            }
        }
    }
	
	/// <summary>
    /// Writes a text during this frame on the DebugText in this scene with the default ID 0. 
    /// Written text is erased on the next frame!
    /// </summary>
    public void Log(string debugText)
    {
        if (debugTexts.ContainsKey(0) == true)
        {
            debugTexts[0].Log(debugText);
        }
        else
        {
            if (reportedDebugTexts.ContainsKey(0) == false)
            {
                reportedDebugTexts.Add(0, true);
                Debug.LogWarning("Debug text with default ID " + 0 + " not found in this scene!");
            }
        }
    }
}
