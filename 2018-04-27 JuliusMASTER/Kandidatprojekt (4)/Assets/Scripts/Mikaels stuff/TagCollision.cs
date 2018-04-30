using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/*
* This script is attached to TagAbilityCollider gameprefab which should be a child of every playable characters
* 
*/

public class TagCollision : MonoBehaviour
{
    private float stunDuration = 2.5f;
    //unsure if using only OnCollisionEnter might cause buggs or not otherwise use OnCollisionStay
    void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Player"))
        {
            var enemyTarget = col.GetComponent<PlayerController>();

            //Stun the player(s) that are in range (does not yet take in consideration if ally or not) 
            enemyTarget.GetStunned(col.gameObject, stunDuration);

            //if the player (or one of the players in range) has flag
            if (enemyTarget.hasFlag == true && gameObject.transform.parent.GetComponent<PlayerController>().hasFlag == false)
            {
                //find the flag among the children of the player object and steal it
                foreach (Transform child in col.transform)
                {
                    if (child.CompareTag("Flag"))
                    {

                        //position the flag to the player who attacked
                        child.transform.position = new Vector3(gameObject.transform.parent.position.x,
                            gameObject.transform.parent.position.y + 4f, gameObject.transform.parent.position.z);


                        //Set the flags parent to be the player who attacked
                        child.parent = gameObject.transform.parent;

                        //the player who attacked stole the flag and so that player has a flag
                        gameObject.transform.parent.GetComponent<PlayerController>().hasFlag = true;
                        //enable the has flag icon on player who stole flag
                        gameObject.transform.parent.GetComponent<PlayerController>().ToggleStatusImages("flagImage", true);


                        //other player got their flag stolen from them
                        enemyTarget.hasFlag = false;
                        //disable the has flag icon on enemy
                        enemyTarget.ToggleStatusImages("flagImage", enemyTarget.hasFlag);

                    }
                }
            }
        }
    }
}
