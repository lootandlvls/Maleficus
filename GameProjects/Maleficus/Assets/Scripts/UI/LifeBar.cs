using UnityEngine;
using UnityEngine.UI;
using static Maleficus.MaleficusConsts;

public class LifeBar : MonoBehaviour {

	[SerializeField] private Image barImage;
    [SerializeField] private Image contentImage;
    [SerializeField] private float smoothingFactor = 7;

    private float newPercentage = 1.0f;

    public bool IsVisible
    {
        get
        {
            return isVisible;
        }
        set
        {
            barImage.enabled = value;
            contentImage.enabled = value;   
            isVisible = value;
        }
    }
    private bool isVisible;

    private void Start()
    {
        EventManager.Instance.GAME_PlayerStatsUpdated += On_GAME_PlayerStatsUpdated;
    }

    private void On_GAME_PlayerStatsUpdated(AbstractPlayerStats playerStats, EGameMode gameMode)
    {
        Debug.Log("Player stats updated " + gameMode);
        if (gameMode == EGameMode.DUNGEON)
        {

            PlayerStats_Dungeon dungeonStats = (PlayerStats_Dungeon)playerStats;
            newPercentage = (dungeonStats.RemainingLives * 1.0f) / PLAYER_LIVES_IN_DUNGEON_MODE;
        }
    } 

    private void Update()
    {
        contentImage.fillAmount = Mathf.Lerp(contentImage.fillAmount, newPercentage, Time.deltaTime * smoothingFactor);
    }
}
