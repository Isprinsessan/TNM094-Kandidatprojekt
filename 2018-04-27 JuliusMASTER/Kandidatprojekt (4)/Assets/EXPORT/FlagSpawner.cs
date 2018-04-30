using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagSpawner : NetworkBehaviour {
    //the object to spawn and how many
    public GameObject flagPrefab;
    //public int numFlags;
   // public float minRandom;
   // public float maxRandom;

    public override void OnStartServer()
    {
        /*
        var pos = gameObject.transform.position; //the FlagSpawner position
        //rotate the flag around y axis
        var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        //flag is flagPrefab object, with position and rotation as decided above
        var flag = (GameObject)Instantiate(flagPrefab, pos, rotation);
        NetworkServer.Spawn(flag); //spawn flag on all clients ("which are ready")
        */
        spawnFlag();

    }
    
    public void spawnFlag()
    {
        /*for (int i = 0; i < 3; i++)
        {*/


            var pos = gameObject.transform.position; //the FlagSpawner position
                                                     //rotate the flag around y axis
            var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            //flag is flagPrefab object, with position and rotation as decided above
            var flag = (GameObject)Instantiate(flagPrefab, pos, rotation);
            NetworkServer.Spawn(flag); //spawn flag on all clients ("which are ready")
        //}
    }
}
