using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* This script is attached to a child collider of the bear. 
 * 
 * The purpose of this script is to stun&knockback all other players in the collider range
 * Also if another player has a flag, the flag is dropped
 */

public class BearAbilityCollider : MonoBehaviour
{
    //Here we decide how long the bear roar stun is
    private float stunDuration = 1.5f;


    private float timer = 0f;
    //making sure that the while loop below doesn't go on forever
    private float maxDur = 0.5f;

    PlayerController enemyPlayer;

    private bool inCol = false;

    private Vector3 tempEnemyPos = Vector3.zero;

    private void Update()
    {

        //ran when player enters collider area with players -> push player away 
        if (inCol)
        {
            //position the players caught in the cc away from the player cc:ing them (push them away)
            if (timer < maxDur)
            {
                timer += Time.deltaTime;
                enemyPlayer.transform.position += gameObject.transform.parent.forward * 20 * Time.deltaTime;
            }
            else
            {
                timer = 0f;
                inCol = false;

                //if the player had a flag it should have been dropped in OnTriggerEnter already so set bool to false
                enemyPlayer.hasFlag = false;
                //disable the has flag icon
                enemyPlayer.ToggleStatusImages("flagImage", enemyPlayer.hasFlag);

            }

        }
    }


    //when some object enters this collider
    void OnTriggerEnter(Collider col)
    {

        //if a player is in the collider apply crowd control (stun..)
        if (col.CompareTag("Player"))
        {
            inCol = true;
            enemyPlayer = col.GetComponent<PlayerController>();

            //Stun the player(s) that are in range (does not take in consideration if ally or not) 
            enemyPlayer.GetStunned(col.gameObject, stunDuration);

            //store the enemy players position
            tempEnemyPos = enemyPlayer.transform.position;

            //making the enemy fly up a little bit (to avoid the player from falling 
            //through the floor when pushing the enemy away in the while below (due to height differences in map))
            enemyPlayer.transform.position += new Vector3(0f, 4f, 0f);


            //if one of the players in range has flag
            if (enemyPlayer.hasFlag == true)
            {
                //find the flag among the children of the player object and drop it
                foreach (Transform child in col.transform)
                {
                    if (child.CompareTag("Flag"))
                    {
                        child.transform.position = tempEnemyPos + new Vector3(0f, 2f, 0f);

                        child.parent = null;
                    }
                }
            }

        }
    }
}
