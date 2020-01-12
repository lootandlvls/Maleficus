using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Check DebugManager for DebugIDs
/// </summary>
public class DebugText : MonoBehaviour
{
    private Text myUIText;
    private string myText;

    public int DebugID;

    private void Awake()
    {
        myUIText = GetComponent<Text>();
    }

    private void Start()
    {
        myText = "";
    }

    private void LateUpdate()
    {
        myUIText.text = myText;
        myText = "";
    }

    public void Log(string newText)
    {
        myText += newText + "\n";
    }
}
