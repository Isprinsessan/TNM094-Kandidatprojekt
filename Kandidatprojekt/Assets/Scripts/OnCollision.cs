using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 *The purpose of this script is to check if a Player collides with a flag.
 * If that is the case then make the flag a child of Player
 * the script is attached to the flagPrefab.
 * 
 */

public class OnCollision : NetworkBehaviour {

    // When an object collides with ontrigger collider
    void OnTriggerEnter(Collider col)
    {
        print("Collision with Flag");
        //if collide with Player, make flag object child of player
        if (col.CompareTag("Player") && col.GetComponent<PlayerController>().hasFlag == false)
        {
            //if the flag that the player is touching is already "owned" by another player
            //then change the other player's hasFlag bool to false and allow this player to steal it
            if (gameObject.transform.parent != null)
            {
                //needs some kind of cooldown so that a flag can be stolen smoothly
                //otherwise it gets stolen back an forth many times in a small duration
                gameObject.GetComponentInParent<PlayerController>().hasFlag = false;
            }
            
             //might want to improve this part or run it through server.
            gameObject.transform.parent = col.transform;
            col.GetComponent<PlayerController>().hasFlag = true;
            
        }
    }
    

}
