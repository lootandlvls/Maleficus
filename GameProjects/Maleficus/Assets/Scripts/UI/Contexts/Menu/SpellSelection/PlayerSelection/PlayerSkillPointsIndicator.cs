using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Maleficus;

public class PlayerSkillPointsIndicator : BNJMOBehaviour
{
    public int ReaminingSkillPoints { get; private set; }

    private Dictionary<int, PlayerSkillPointStar> playerSkillPointStars = new Dictionary<int, PlayerSkillPointStar>();

    private Text skillPointsText;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Stars
        foreach (PlayerSkillPointStar playerSkillPointStar in GetComponentsInChildren<PlayerSkillPointStar>())
        {
            if (IS_KEY_NOT_CONTAINED(playerSkillPointStars, playerSkillPointStar.StarID))
            {
                playerSkillPointStars.Add(playerSkillPointStar.StarID, playerSkillPointStar);
            }
        }

       // Text
       skillPointsText = GetComponentInChildren<Text>();
    }

    protected override void Start()
    {
        base.Start();

        ResetSkillPoints();
    }


    public void ResetSkillPoints()
    {
        ReaminingSkillPoints = Consts.SPELL_MAX_SKILL_POINTS;
    }

    public void RemoveSkillPoints(int amount)
    {
        ReaminingSkillPoints -= amount;

        UpdateStars();

        if (IS_NOT_NULL(skillPointsText))
        {
            skillPointsText.text = ReaminingSkillPoints + "";
        }
    }

    public void AddSkillPoints(int amount)
    {
        ReaminingSkillPoints += amount;

        UpdateStars();

        if (IS_NOT_NULL(skillPointsText))
        {
            skillPointsText.text = ReaminingSkillPoints + "";
        }
    }

    public bool CanChoseSpell(AbstractSpell spell)
    {
        if (IS_NOT_NULL(spell))
        {
            return (ReaminingSkillPoints - spell.SkillPoint) >= 0;
        }
        return false;
    }

    public bool CanSwapSpell(AbstractSpell newSpell, AbstractSpell spellSelected)
    {
        if ((IS_NOT_NULL(spellSelected))
            && (IS_NOT_NULL(newSpell)))
        {
            return (ReaminingSkillPoints + spellSelected.SkillPoint - newSpell.SkillPoint) >= 0;
        }
        return false;
    }

    private void UpdateStars()
    {
        for (int starID = 1; starID < 6; starID++)
        {
            if (IS_KEY_CONTAINED(playerSkillPointStars, starID))
            {
                if (starID <= ReaminingSkillPoints)
                {
                    playerSkillPointStars[starID].Show();
                }
                else
                {
                    playerSkillPointStars[starID].Hide();
                }
            }
        }
    }

}
