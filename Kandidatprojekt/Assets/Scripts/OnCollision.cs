using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnCollision : NetworkBehaviour {

    //this script is currently used on flags, implement another if in OnTriggerEnter
    //when entering base to drop flag.

    // When an object collides with ontrigger collider
    
    void OnTriggerEnter(Collider col)
    {
        print("Collision with Flag");
        //if player, make flag object child of player
        if (col.CompareTag("Player") && col.GetComponent<PlayerController>().hasFlag == false)
        {
            //if the flag is already "owned" by another player
            if(gameObject.transform.parent != null)
            {
                //needs some kind of cooldown so that a flag can be stolen smoothly
                //otherwise it gets stolen back an forth many times in a small duration
                gameObject.GetComponentInParent<PlayerController>().hasFlag = false;
            }
             
            gameObject.transform.parent = col.transform;
           // FindObjectOfType<PlayerController>().hasFlag = true;
            col.GetComponent<PlayerController>().hasFlag = true;
            //Destroy(gameObject);
            //gameObject.GetComponent<Renderer>().enabled = false;

            //FindObjectOfType<ScoreManager>().AddScore(score);
        }

        
        //To be implemented on BaseCollision instead, when a base can be properly associated with a player
        if(col.CompareTag("Base"))
        {
         
            
            gameObject.GetComponentInParent<PlayerController>().hasFlag = false;
            gameObject.transform.parent = null;
            gameObject.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, col.transform.position.z);

            //run function for scoring and making sure that the player can pick up another flag
        } 
    }
    /*
    [Command]
    void CmdPickFlag(Collider col)
    {
        print("player picked up flag");

    } */
/*
    public void dropFlag(GameObject obj)
    {
        print("dropflag void called");
        gameObject.transform.parent = null; //this not working
        gameObject.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);

    } */
}
