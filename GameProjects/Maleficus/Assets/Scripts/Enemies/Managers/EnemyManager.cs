using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Maleficus.Utils;

public class EnemyManager : AbstractSingletonManager<EnemyManager>
{

    public event Action<int> PlayersHit;
    public event Action AllEnemiesDead;
    public event Action AllMinionsDead;


    [Header("Basic")]
    [SerializeField] BasicEnemy basicEnemyPrefab;
    [SerializeField] private int basicEnemyMaxNumber;
    [Header("Champion")]
    [SerializeField] BasicEnemy championEnemyPrefab;
    [SerializeField] private int championEnemyMaxNumber;
    [Header("Boss")]
    [SerializeField] BasicEnemy bossEnemyPrefab;
    [SerializeField] private int bossEnemyMaxNumber;
    [Header("Minion")]
    [SerializeField] BasicEnemy minionEnemyPrefab;


    [SerializeField] private int numberOfCollectedCoins = 0;
    EnemySpawnPosition[] enemySpawnPositions;


    private int lastChosedIndex;

    private int spawnedBasicEnemyCounter = 0;
    private int spawnedChampionEnemyCounter = 0;
    private int spawnedBossEnemyCounter = 0;
    private int spawnedMinionEnemyCounter = 0;

    private int livingBasicEnemyCounter = 0;
    private int livingChampionEnemyCounter = 0;
    private int livingBossEnemyCounter = 0;
    private int livingMinionEnemyCounter = 0;

    private bool spawnBasicMonsters = false;
    private bool spawnChampionMonsters = false;
    private bool spawnBossMonster = false;

    private int waveCounter = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.APP_AppStateUpdated.AddListener(OnAppStateUpdated);
        EventManager.Instance.PLAYERS_PlayerCollectedCoin += On_PLAYERS_PlayerCollectedCoin;
        EventManager.Instance.ENEMIES_EnemyDied += On_ENEMIES_EnemyDied;
    }

    protected override void OnReinitializeManager()
    {
        base.OnReinitializeManager();

        FindAndBindButtonActions();
        waveCounter = 0;
    }

    protected override void Update()
    {
        base.Update();

        DebugManager.Instance.Log(80,
            "basicEnemyMaxNumber : " + basicEnemyMaxNumber
            + "\nspawnedBasicEnemyCounter : " + spawnedBasicEnemyCounter
            + "\nlivingBasicEnemyCounter : " + livingBasicEnemyCounter
            + "\nspawnBasicMonsters : " + spawnBasicMonsters
            );
    }

    private void SpawnNextWave()
    {
        if (enemySpawnPositions.Length != 0)
        {

            spawnedBasicEnemyCounter = 0;
            spawnedChampionEnemyCounter = 0;
            spawnedBossEnemyCounter = 0;
            spawnedMinionEnemyCounter = 0;

            livingBasicEnemyCounter = 0;
            livingChampionEnemyCounter = 0;
            livingBossEnemyCounter = 0;
            livingMinionEnemyCounter = 0;

            basicEnemyMaxNumber = CoinManager.Instance.NumberOfCoins * 3 - numberOfCollectedCoins ;

            if (numberOfCollectedCoins != 1)
            {
                championEnemyMaxNumber = (int) (numberOfCollectedCoins * 2.5f); 
            }
            else
            {
                championEnemyMaxNumber = 0;
            }


            spawnBasicMonsters = true;
            spawnChampionMonsters = true;
            StartSpawningBasicMonsters();
            StartSpawningChampionMonsters();

            waveCounter++;
        }
    }


    private void StartSpawningBasicMonsters()
    {
        if (basicEnemyMaxNumber == 0)
        {
            spawnBasicMonsters = false;
            return;
        }
        StartCoroutine(BasicMonstersCoroutine());
    }
    private IEnumerator BasicMonstersCoroutine()
    {

        while ((spawnBasicMonsters == true) && (basicEnemyPrefab != null))
        {
            // Make sure the enemy doesn't spawn in the same position of the last spawn enemy
            int randomPositionIndex;
            do
            {
                randomPositionIndex = GetRandomIndex(enemySpawnPositions.Length);
            } while (randomPositionIndex == lastChosedIndex);
            lastChosedIndex = randomPositionIndex;
            
            GameObject enemyObject = Instantiate(basicEnemyPrefab.gameObject, enemySpawnPositions[randomPositionIndex].Position, Quaternion.identity);
            livingBasicEnemyCounter++;
            spawnedBasicEnemyCounter++;
            if (spawnedBasicEnemyCounter == basicEnemyMaxNumber)
            {
                spawnBasicMonsters = false;
            }
            yield return new WaitForSeconds(basicEnemyPrefab.SpawnRate);
        }
    }

    private void StartSpawningChampionMonsters()
    {
        if (championEnemyMaxNumber == 0)
        {
            spawnChampionMonsters = false;
            return;
        }
        StartCoroutine(ChampionMonstersCoroutine());
    }
    private IEnumerator ChampionMonstersCoroutine()
    {
        if (championEnemyPrefab != null)
        {
            yield return new WaitForSeconds(championEnemyPrefab.SpawnRate);
        }
        while ((spawnChampionMonsters == true) && (championEnemyPrefab != null))
        {
            // Make sure the enemy doesn't spawn in the same position of the last spawn enemy
            int randomPositionIndex;
            do
            {
                randomPositionIndex = GetRandomIndex(enemySpawnPositions.Length);
            } while (randomPositionIndex == lastChosedIndex);
            lastChosedIndex = randomPositionIndex;


            GameObject enemyObject = Instantiate(championEnemyPrefab.gameObject, enemySpawnPositions[randomPositionIndex].Position, Quaternion.identity);
            livingChampionEnemyCounter++;
            spawnedChampionEnemyCounter++;
            if (spawnedChampionEnemyCounter == championEnemyMaxNumber)
            {
                spawnChampionMonsters = false;
            }
            yield return new WaitForSeconds(championEnemyPrefab.SpawnRate);
        }
    }

    private void FindAndBindButtonActions()
    {
        enemySpawnPositions = FindObjectsOfType<EnemySpawnPosition>();
    }


    private void On_PLAYERS_PlayerCollectedCoin()
    { 
        numberOfCollectedCoins++;
        SpawnNextWave();
    }

    private void OnEnemyAttacked(IEnemy attackingEnemy)
    {
        if (PlayersHit != null) PlayersHit.Invoke(attackingEnemy.Damage);
    }

    private void On_ENEMIES_EnemyDied(IEnemy deadEnemy)
    {
        switch (deadEnemy.IsType)
        {
            case EEnemyType.BASIC:
                livingBasicEnemyCounter--;
                break;

            case EEnemyType.CHAMPION:
                livingChampionEnemyCounter--;
                break;

            case EEnemyType.BOSS:
                livingBossEnemyCounter--;
                break;

            case EEnemyType.MINION:
                livingMinionEnemyCounter--;
                break;
        }

        if ((livingMinionEnemyCounter == 0) && (spawnedMinionEnemyCounter != 0))
        {
            if (AllMinionsDead != null) AllMinionsDead.Invoke();
            spawnedMinionEnemyCounter = 0;
        }

        // All enemies dead?
        if ((livingBasicEnemyCounter == 0) && (livingBossEnemyCounter == 0) && (livingChampionEnemyCounter == 0) && (livingMinionEnemyCounter == 0))
        {
            EventManager.Instance.Invoke_ENEMIES_WaveCompleted(waveCounter);
        }

        // All basic and champion monsters died? Spawn boss
        //if ((spawnBossMonster == false) && (livingBossEnemyCounter == 0))
        //{
        //    Debug.Log("Game Won!");
        //    if (AllEnemiesDead != null) AllEnemiesDead.Invoke();
        //}
        //else if ((spawnBasicMonsters == false) && (livingBasicEnemyCounter == 0)
        //    && (spawnChampionMonsters == false) && (livingChampionEnemyCounter == 0)
        //    && (livingBossEnemyCounter == 0))
        //{
        //    Debug.Log("Spawn Boss");
        //    if (bossEnemyPrefab != null)
        //    {
        //        SpawnBossMonster();
        //    }
        //    else
        //    {
        //        if (AllEnemiesDead != null) AllEnemiesDead.Invoke();
        //    }

        //}
    }

    

    private void OnAppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        switch (eventHandle.NewState)
        {
            case EAppState.IN_GAME_IN_RUNNING:

                break;

            default:
                StopAllCoroutines();
                break;
        }

    }


    



    //private void SpawnBossMonster()
    //{
    //    Instantiate(bossEnemyPrefab.gameObject, bossSpawnPosition.position, Quaternion.identity);
    //    livingBossEnemyCounter++;
    //    spawnedBossEnemyCounter++;
    //    spawnBossMonster = false;
    //}

}
