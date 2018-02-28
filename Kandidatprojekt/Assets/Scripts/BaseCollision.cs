using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* the aim of this script is to check collision between a player and his base
 * to see if player has a flag, if so destroy (or deactivate it if the practice of objectpooling 
 * is to be used, :gameObject.GetComponent<Renderer>().enabled = false;)
 * the flag and give player score.
 * Make sure that the player can pick up flags after returning the flag.
 * This script is attached to the basePrefab.
 * 
 * 
 * 
 */

public class BaseCollision : NetworkBehaviour
{
    //is ran when something collide with the Base trigger collider
    void OnTriggerEnter(Collider col) //col is the collider that triggers with base
    {
        //if the collider is a player and if it's the player's base and if that player has a flag, destroy flag and give points
        if (col.CompareTag("Player") && col.GetComponent<PlayerController>().baseId == netId && col.GetComponent<PlayerController>().hasFlag)
        {
            print("player with flag entered his base");
            //this foreach is ran to find and access the child of Player that has tag Flag. 
            foreach (Transform child2 in col.transform)
            {
                if (child2.CompareTag("Flag"))
                {
                    col.GetComponentInParent<PlayerController>().hasFlag = false;
                    print("the flag has been dropped in base");
                    Destroy(child2.gameObject);
                    //call function below to give score...
                    col.GetComponentInParent<PlayerController>().score += 1;
                }
            }
           
        }
    }
    
}
