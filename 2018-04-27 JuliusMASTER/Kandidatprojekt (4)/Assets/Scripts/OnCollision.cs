using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 *The purpose of this script is to check if a Player collides with a flag.
 * If that is the case then make the flag a child of Player
 * the script is attached to the flagPrefab.
 * 
 * The script and BaseCollision might need to be updated to solve bugs regarding picking up and dropping flags
 * Bug that might exist: Picking up more than one flag, Flag not properly being dropped off in base (have to walk in twice)
 */

public class OnCollision : NetworkBehaviour
{
    //bool used to spawn ONE more flag when captured (when placed in a player's base)
    public bool beenCaptured = false;

    // When an object collides with ontrigger collider
    void OnTriggerEnter(Collider col)
    {
        //if collide with a Player that does not have a flag
        if (col.CompareTag("Player") && col.GetComponent<PlayerController>().hasFlag == false)
        {
            //fetch the playercontroller component since it's used below on multiple occasions.
            var player = col.GetComponent<PlayerController>();

            //flag does not have a parent
            if (gameObject.transform.parent == null)
            {
                player.hasFlag = true;

                //position the flag to the player and make the flag a child of the player
                gameObject.transform.parent = col.transform;

                gameObject.transform.position = new Vector3(col.transform.position.x,
                                col.transform.position.y + 4f, col.transform.position.z);

            }
            //When returning a flag to base, OnCollision gets called and makes the player the parent again
            //So making a player unable to pick up flag from their own base fixes this issue
            else if (gameObject.GetComponentInParent<BaseCollision>() != null
                && gameObject.GetComponentInParent<BaseCollision>().netId != player.baseId) //if flag is in a different players base
            {
                player.hasFlag = true;


                //save the flag's parent ( a base)
                var tempParent = gameObject.transform.parent;
                //find the ID of the base (that the flag is in)
                var baseID = gameObject.GetComponentInParent<BaseCollision>().netId;

                //position the flag to the player and make the flag a child of the player
                gameObject.transform.parent = col.transform;

                gameObject.transform.position = new Vector3(col.transform.position.x,
                                col.transform.position.y + 4f, col.transform.position.z);


                //a flag has been stolen from a base -> find what color the base had -> update score
                var baseSpawnerIds = FindObjectOfType<BaseSpawner>().baseIds;
                foreach (Color baseColor in baseSpawnerIds.Keys)
                {
                    if (baseSpawnerIds[baseColor] == baseID)
                    {
                        player.giveScoreAndAutho(baseColor, tempParent.childCount);
                    }

                }
                //go to playercontroller to use network command to increase game time
                player.addTime(10);
            }
            //if flag was picked up, toggle status
            player.ToggleStatusImages("flagImage", player.hasFlag);

        }
    }


}
