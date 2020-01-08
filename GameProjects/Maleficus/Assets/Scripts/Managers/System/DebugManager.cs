using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Multiple channel (IDs) debug messages on every frame
/// IDs:
/// 2  - Joystick 
/// 6  - AI
/// 9  - Player Input Listener
/// 12 - Players join status
/// 50 - UI state
/// 51 - App state 
/// 52 - Scene
/// 60 - InfoText for Players
/// 80 - Enemies
/// 89 - AR Anchor status
/// 92 - Network Player Movement
/// 103 - GM_FFA_Lives - Single lives game mode stats
/// </summary>
public class DebugManager : AbstractSingletonManager<DebugManager>
{
    private Dictionary<int, DebugText> debugTexts = new Dictionary<int, DebugText>();
    private Dictionary<int, bool> reportedDebugTexts = new Dictionary<int, bool>();

    public override void OnSceneStartReinitialize()
    {
        debugTexts.Clear();
        reportedDebugTexts.Clear();

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
