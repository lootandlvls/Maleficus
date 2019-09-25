using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsARAction : AbstractUIAction
{
    private Text myText;

    protected override void Awake()
    {
        base.Awake();

        myText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        if (MotherOfManagers.Instance.IsARGame == true)
        {
            SetIsAR();
        }
        else
        {
            SetIsNotAR();
        }
    }

    public override void Execute()
    {
        base.Execute();

        if (MotherOfManagers.Instance.IsARGame == true)
        {
            MotherOfManagers.Instance.IsARGame = false;
            SetIsNotAR();
        }
        else
        {
            MotherOfManagers.Instance.IsARGame = true;
            SetIsAR();
        }
    }


    public void SetIsAR()
    {
        //GetComponentInChildren<Text>().color = Color.red;
        Color color = Color.green;
        color.a = 0.25f;
        GetComponent<Image>().color = color;
    }

    public void SetIsNotAR()
    {
        //GetComponentInChildren<Text>().color = Color.green;
        Color color = Color.red;
        color.a = 0.25f;
        GetComponent<Image>().color = color;

    }
}
