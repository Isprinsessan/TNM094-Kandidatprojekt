using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseCollision : NetworkBehaviour
{
    /* Currently not working properly
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            print("player in base");
            if(col.GetComponent<PlayerController>().hasFlag)
            {
                print("player in base and has flag");
                //if it's the player's base give score and drop flag ERROR: doesn't work
               
                if(hasAuthority)
                {
                    print("player has authority");
                    col.GetComponentInParent<PlayerController>().hasFlag = false;
                    var aFlag =  col.transform.GetChild(1);
                    aFlag.transform.parent = null;
                    aFlag.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
                    //FindObjectOfType<OnCollision>().dropFlag(gameObject);
                } 
            }

            

            //run function for scoring and making sure that the player can pick up another flag
        }
    }
    */
}
