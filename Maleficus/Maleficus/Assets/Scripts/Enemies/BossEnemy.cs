using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Klasse fuer Huendchen Gegner
public class BossEnemy : BasicEnemy
{


    [Header("Attributes")]
    [SerializeField] private int lives = 5;
    [SerializeField] private int subLives = 5; // how many gesture rows?
  


    private int lifeCounter;
    private int subLifeCounter; // how many gesture rows?


    protected override void Start()
    {
        base.Start();
        
        // Init Enemy
        lifeCounter = lives;
        subLifeCounter = subLives;

        EnemyManager.Instance.AllMinionsDead += OnAllMinionsDead;
    }

    private void OnAllMinionsDead()
    {
        myAnimator.SetBool("IsKnocked", false);
        //InitGestureSymbols();
        subLifeCounter = subLives;
        gestureIndex = 0;
        UpdateState(EnemyState.MOVING_TOWARDS_PLAYER);
        
    }

    protected override void Update()
    {
        base.Update();

        switch (myState)
        {
            
            case EnemyState.ROAR:
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    Debug.Log("Back to moving");
                    UpdateState(EnemyState.MOVING_TOWARDS_PLAYER);
                }
                break;

        }
    }



    protected override void UpdateState(EnemyState newState)
    {
        base.UpdateState(newState);

        switch (newState)
        {

            case EnemyState.ROAR:
                myNavAgent.speed = 0;
                myAnimator.SetBool("IsMoving", false);
                myAnimator.SetTrigger("Roar");
                Debug.Log("Roar animation");
                break;


            case EnemyState.KNOCKED:
                myNavAgent.speed = 0;
                myAnimator.SetBool("IsMoving", false);
                myAnimator.SetBool("IsKnocked", true);

                Invoke("SpawnMinions", 1.0f);
                break;
        }
    }


    private void AttackEveryPeriodOfTime()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        while (myState == EnemyState.ATTACKING)
        {
            while ((myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) == false)
            {
                yield return new WaitForEndOfFrame();
            }
            myAnimator.SetTrigger("Attack1");
            InvokeAttackedEvent();
            yield return new WaitForSeconds(attackTimeOut);
        }
    }






    //protected override void OnPlayerPerformedGesture(PlayerID byPlayer, GestureType gestureType)
    //{
    //    if ((AppStateManager.Instance.CurrentState == AppState.PLAYING) 
    //        && (myState != State.KNOCKED) && (myState != State.DEAD))
    //    {
    //        if ((currentGesture.ToPlayerID == byPlayer) && (currentGesture.IsType == gestureType))
    //        {
    //            // Instantiate Lazer from player
    //            PlayersManager.Instance.SpawnLazer(byPlayer, gestureType, transform);
    //            PlayersManager.Instance.SpawnSpellBall(byPlayer, gestureType, transform);

    //            // Instantiate particle effect
    //            if (damageParticleEffect != null)
    //            {
    //                Instantiate(damageParticleEffect, transform.position, Quaternion.identity);
    //            }

    //            // Update gesture and check if gesture row entirely attacked
    //            gestureIndex++;
    //            gestureSymbols[gestureIndex - 1].HideSprite();
    //            if (gestureIndex == gestures.Length)
    //            {
    //                Debug.Log("Row completed");

    //                subLifeCounter--;
    //                // attacked all rows? knock down
    //                if (subLifeCounter == 0)
    //                {
    //                    Debug.Log("All sub lives finished");
    //                    lifeCounter--;
    //                    // Dead ?
    //                    if (lifeCounter == 0)
    //                    {
    //                        Debug.Log("All lives finished");
    //                        UpdateState(State.DEAD);
    //                    }
    //                    // Knocked
    //                    else
    //                    {
    //                        Debug.Log("Knocked");
    //                        UpdateState(State.KNOCKED);
    //                    }
    //                } 
    //                else // Roar and spawn new row
    //                {
    //                    Debug.Log("New row");
    //                    InitGestureSymbols();
    //                    UpdateState(State.ROAR);
    //                }
                    
                    
    //            }
    //            else // just get hit
    //            {
    //                currentGesture = gestures[gestureIndex];
    //                UpdateState(State.GETTING_HIT);

    //            }
    //        }
    //    }

    //}
}
