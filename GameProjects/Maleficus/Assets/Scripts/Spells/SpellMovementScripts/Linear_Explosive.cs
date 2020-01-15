using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Explosive : AbstractSpell { 

      private Vector3 movingDirection;
      public Vector3 directionVector;
      private Vector3 forward = Vector3.forward;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Move();
    }


    public void Move()
    {

        movingDirection = forward * Speed * Time.deltaTime;

        directionVector = transform.TransformDirection(movingDirection);
        myRigidBody.velocity = new Vector3(directionVector.x, directionVector.y, directionVector.z);

    }
    private void OnTriggerStay(Collider other)
    {
        Vector3 movingDirection = Vector3.forward;
        direction = transform.TransformDirection(movingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
        Shield shield = other.gameObject.GetComponent<Shield>();
          ;
        if (shield == null)
        {
            if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == Maleficus.Consts.TAG_PLAYER)
            {
                if ((other.transform.position - transform.position )!= Vector3.zero )
                {

                    SetDirection(other.transform.position - transform.position);
                    Debug.Log(other.transform.position - transform.position);
                    

                }
                ProcessHits(otherPlayer, ESpellStatus.STAY);
            }
            else if (otherEnemy != null)
            {
                ProcessHits(otherEnemy);

            }
        }

    }
}
