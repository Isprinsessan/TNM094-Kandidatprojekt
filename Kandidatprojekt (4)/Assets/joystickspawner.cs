using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class joystickspawner : NetworkBehaviour
    {
        //the object to spawn and how many
        public GameObject joystickprefab;
       

        public override void OnStartServer()
        {
                var joystick = (GameObject)Instantiate(joystickprefab);
                NetworkServer.Spawn(joystick); //spawn flag on all clients ("which are ready")

    }
}
