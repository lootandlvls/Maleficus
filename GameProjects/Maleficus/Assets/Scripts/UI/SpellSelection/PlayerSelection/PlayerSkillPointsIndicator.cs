using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillPointsIndicator : MaleficusMonoBehaviour
{
    public int ReaminingSkillPoints { get; private set; }

    private Text skillPointsText;

    protected override void Start()
    {
        base.Start();

        ReaminingSkillPoints = Maleficus.Consts.SPELL_MAX_SKILL_POINTS;
        skillPointsText = GetComponentInChildren<Text>();
    }

    public void ResetSkillPoints()
    {
        ReaminingSkillPoints = 10;
    }

    public void RemoveSkillPoints(int amount)
    {
        ReaminingSkillPoints -= amount;
        if (IS_NOT_NULL(skillPointsText))
        {
            skillPointsText.text = ReaminingSkillPoints + "";
        }
    }

    public void AddSkillPoints(int amount)
    {
        ReaminingSkillPoints += amount;
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
}
