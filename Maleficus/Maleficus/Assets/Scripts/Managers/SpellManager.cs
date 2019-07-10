using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpellManager : AbstractSingletonManager<SpellManager>
{

    [SerializeField] private GameObject FrozenEffect;
    [SerializeField] private float friction;

    private Dictionary<EPlayerID, Player> activePlayers = new Dictionary<EPlayerID, Player>();

    private void Start()
    {
        activePlayers = PlayerManager.Instance.ActivePlayers;

        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
        EventManager.Instance.SPELLS_Teleport += On__SPELLS_Teleport;
    }

    private void On__SPELLS_Teleport(ISpell castedSpell, EPlayerID castingPlayerID)
    {
        Debug.Log("Teleportation spell executed");
        float InputH = Input.GetAxis(activePlayers[castingPlayerID].playerHorizontalInput);
        float InputV = Input.GetAxis(activePlayers[castingPlayerID].playerVerticalInput);
        Debug.Log(InputH + " and " + InputV);
        Vector3 TeleportDirection = new Vector3();
        TeleportDirection.x = InputH * Time.deltaTime * 10;
        TeleportDirection.z = InputV * Time.deltaTime * 10;
        activePlayers[castingPlayerID].transform.position += TeleportDirection * 50;



    }
    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        if (hitInfo.HasPower)
        {
            if (activePlayers[hitInfo.HitPlayerID].PlayerID == hitInfo.HitPlayerID)
            {

                StartCoroutine(PushPlayer(hitInfo));


            }
        }

        foreach (SpellEffects debuffeffect in hitInfo.DebuffEffects)
        {
            ApplyDebuff(debuffeffect, hitInfo.HitPlayerID);
        }

        foreach (SpellEffects buffeffect in hitInfo.DebuffEffects)
        {
            ApplyBuff(buffeffect, hitInfo.HitPlayerID);
        }
    }

    private IEnumerator PushPlayer(SHitInfo hitInfo)
    {   
        
        Rigidbody rgb = activePlayers[hitInfo.HitPlayerID].GetComponent<Rigidbody>();
        Transform playerTransform = activePlayers[hitInfo.HitPlayerID].GetComponent<Transform>();
        rgb.isKinematic = false;
        //  transform.position = Vector3.MoveTowards(activePlayers[hitInfo.HitPlayerID].transform.position, hitInfo.HitVelocity, Time.deltaTime * 2);
        Debug.Log(hitInfo.HitVelocity);
        Vector3 endPushPosition = new Vector3(hitInfo.HitVelocity.x, 0, hitInfo.HitVelocity.z);
        
        endPushPosition = transform.TransformDirection(endPushPosition);
        playerTransform.position = Vector3.Lerp(playerTransform.position, endPushPosition, Time.deltaTime * friction);
        yield return new WaitForSeconds(0.1f);
        rgb.isKinematic = true;
    }

    private void ApplyDebuff(SpellEffects debuff, EPlayerID playerID)
    {

        switch (debuff)
        {
            case SpellEffects.FROZEN:
                StartCoroutine(PlayerFrozen(playerID));
                break;
            case SpellEffects.STUN:
                

                break;

            case SpellEffects.SLOWDOWN:
                Debug.Log("Player SLOWED DOWN");

                break;

            case SpellEffects.CHARM:

                break;

        }




    }
    private void ApplyBuff(SpellEffects buff, EPlayerID playerID)
    {
        Debug.Log("Apply Buff on player " + playerID);
        switch (buff)
        {
            case SpellEffects.INCREACE_SPEED:               
                
                break;

            case SpellEffects.INCREASE_CASTING_SPEED:

                Debug.Log("INCREASE_CASTING_SPEED");
                break;

            case SpellEffects.INCREASE_DAMAGE:
                Debug.Log("IINCREASE_DAMAGE");
                break;

            case SpellEffects.INCREASE_OFFENSIVE_SPELL_SIZE:
                Debug.Log("INCREASE_OFFENSIVE_SPELL_SIZE");
                break;
            case SpellEffects.PROTECT:
                Debug.Log("PROTECT");
                break;
            case SpellEffects.REMOVE_DEBUFF:
                Debug.Log("REMOVE_DEBUF");
                break;


        }




    }

    private IEnumerator PlayerFrozen(EPlayerID playerID)
    {
        activePlayers[playerID].speed = 0;
        GameObject snowman = Instantiate(FrozenEffect, activePlayers[playerID].transform.position, activePlayers[playerID].transform.rotation);
        activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(false);
        snowman.transform.parent = activePlayers[playerID].transform;
        yield return new WaitForSeconds(1.5f);
        activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(true);
        activePlayers[playerID].speed = 75;
    }

    private IEnumerator PlayerStunned(EPlayerID playerID)
    {
        activePlayers[playerID].speed = 0;
        yield return new WaitForSeconds(2.5f);
        activePlayers[playerID].speed = 75;
    }

}
