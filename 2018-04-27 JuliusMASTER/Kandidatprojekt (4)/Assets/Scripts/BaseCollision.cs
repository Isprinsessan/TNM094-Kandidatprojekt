using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* the aim of this script is to check collision between a player and his base
 * to see if player has a flag, if so then place
 * the flag in base and update team score.
 * Make sure that the player can pick up flags after returning the flag.
 * This script is attached to the basePrefab.
 *  
 */

public class BaseCollision : NetworkBehaviour
{
    //is ran when something collide with the Base trigger collider
    void OnTriggerEnter(Collider col) //col is the collider that enters base
    {
        //if the collider is a player and if it's the player's base and if that player has a flag, place flag and update score
        if (col.CompareTag("Player") && col.GetComponent<PlayerController>().baseId == netId && col.GetComponent<PlayerController>().hasFlag)
        {
            //fetch the playercontroller component since it's used below on multiple occasions.
            var player = col.GetComponent<PlayerController>();

            //this foreach is ran to find and access the child of Player that has tag Flag. 
            foreach (Transform child2 in col.transform)
            {
                if (child2.CompareTag("Flag")) //if found the the flag
                {
                    //spawn a new flag if this is the first time the flag is captured (beenCaptured == false)
                    if (!child2.GetComponent<OnCollision>().beenCaptured)
                    {
                        player.spawnFlag(child2.gameObject);

                    }

                    var BFH = gameObject.transform.Find("BaseFlagHolder");

                    //flag position = BaseFlagHolder position
                    child2.transform.position = BFH.position + new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f));
                    //flag becomes child of BaseFlagHolder
                    child2.parent = BFH;

                    //player does not have a flag any longer
                    player.hasFlag = false;

                    //update score to be the amount of children in the BaseFlagHolder (only children in BaseFlagHolder are flags)
                    player.giveScoreAndAutho(player.color, BFH.childCount);
                    //disable the flag image since the flag was returned
                    player.ToggleStatusImages("flagImage", player.hasFlag);



                }
            }


        }
    }


}
