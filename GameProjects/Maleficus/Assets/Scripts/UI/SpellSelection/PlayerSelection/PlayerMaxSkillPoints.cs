using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMaxSkillPoints : MonoBehaviour
{
   public int MaxSkillPoints { get { return maxSkillPoints; } }
   [SerializeField] int maxSkillPoints;

    private Text skillPointsText;

    private void Start()
    {
        maxSkillPoints = 10;
        skillPointsText = GetComponent<Text>();
    }

    public void ResetSkillPoints()
    {
        maxSkillPoints = 10;
    }
    public void RemoveSkillPoints(int amount)
    {
        Debug.Log("REMOVED AMOUT IS EQUAL TO = " + amount);
        maxSkillPoints -= amount;
        skillPointsText.text = maxSkillPoints + "";
    }

    public void AddSkillPoints(int amount)
    {
        Debug.Log("ADDED AMOUT IS EQUAL TO = " + amount);
        maxSkillPoints += amount;
        skillPointsText.text = maxSkillPoints + "";
    }
}
