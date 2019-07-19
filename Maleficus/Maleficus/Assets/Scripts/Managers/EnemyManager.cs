using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : AbstractSingletonManager<EnemyManager>
{

    public event Action<int> PlayersHit;
    public event Action AllEnemiesDead;
    public event Action AllMinionsDead;

    public Action<IEnemy> Action_OnEnemyAttacked;
    public Action<IEnemy> Action_OnEnemyDied;


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


    protected override void Awake()
    {
        base.Awake();

        FindAndBindButtonActions();
    }

    protected override void FindAndBindButtonActions()
    {
        base.FindAndBindButtonActions();

        enemySpawnPositions = FindObjectsOfType<EnemySpawnPosition>();

    }

    private void Start()
    {
        EventManager.Instance.ÁPP_AppStateUpdated += OnAppStateUpdated;

        Action_OnEnemyAttacked = OnEnemyAttacked;
        Action_OnEnemyDied = OnEnemyDied;
    }

    private void OnEnemyAttacked(IEnemy attackingEnemy)
    {
        if (PlayersHit != null) PlayersHit.Invoke(attackingEnemy.Damage);
    }

    private void OnEnemyDied(IEnemy deadEnemy)
    {
        switch (deadEnemy.IsType)
        {
            case EnemyType.BASIC:
                livingBasicEnemyCounter--;
                break;

            case EnemyType.CHAMPION:
                livingChampionEnemyCounter--;
                break;

            case EnemyType.BOSS:
                livingBossEnemyCounter--;
                break;

            case EnemyType.MINION:
                livingMinionEnemyCounter--;
                break;
        }

        if ((livingMinionEnemyCounter == 0) && (spawnedMinionEnemyCounter != 0))
        {
            if (AllMinionsDead != null) AllMinionsDead.Invoke();
            spawnedMinionEnemyCounter = 0;
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

    private void OnAppStateUpdated(EAppState newState, EAppState lastState)
    {
        switch (newState)
        {
            case EAppState.IN_GAME_IN_RUNNING:
                spawnBasicMonsters = true;
                spawnChampionMonsters = true;
                spawnBossMonster = true;
                StartSpawningBasicMonsters();
                StartSpawningChampionMonsters();


                break;

            default:
                StopAllCoroutines();
                break;
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
                randomPositionIndex = Utilities.GetRndIndex(enemySpawnPositions.Length);
            } while (randomPositionIndex == lastChosedIndex);
            lastChosedIndex = randomPositionIndex;

            Instantiate(basicEnemyPrefab.gameObject, enemySpawnPositions[randomPositionIndex].Position, Quaternion.identity);
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
                randomPositionIndex = Utilities.GetRndIndex(enemySpawnPositions.Length);
            } while (randomPositionIndex == lastChosedIndex);
            lastChosedIndex = randomPositionIndex;

             GameObject enemy =  Instantiate(championEnemyPrefab.gameObject, enemySpawnPositions[randomPositionIndex].Position, Quaternion.identity);
            enemy.transform.parent = enemySpawnPositions[0].transform.parent;
            livingChampionEnemyCounter++;
            spawnedChampionEnemyCounter++;
            if (spawnedChampionEnemyCounter == championEnemyMaxNumber)
            {
                spawnChampionMonsters = false;
            }
            yield return new WaitForSeconds(championEnemyPrefab.SpawnRate);
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
