using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerController : NetworkBehaviour {

    public GameObject bulletPrefab;
    public GameObject playerBasePrefab;

    protected Rigidbody _rigidbody;

    public bool hasFlag = false;

    private bool ranOnce = false;

    //id of base to check if the current base is yours in BaseCollision
    public NetworkInstanceId baseId;

    //we sync the score to be able to show it to every other client (just a thought not 100% sure it's implemented like this)
    [SyncVar]
    public int score = 0;

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    [Command]
    void CmdFire()
    {
        // This [Command] code is run on the server!

        // create the bullet object locally
        var bullet = (GameObject)Instantiate(
             bulletPrefab,
             transform.position + transform.forward,
             Quaternion.identity);

        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 4;

        // spawn the bullet on the clients
        NetworkServer.Spawn(bullet);

        // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
        Destroy(bullet, 2.0f);
    }
    
    //every player should have a base
    [Command]
    void CmdSpawnBase()
    {
        var playerBase = (GameObject)Instantiate(
             playerBasePrefab,
             transform.position - transform.forward*5.5f,
              Quaternion.Euler(0, transform.rotation.y+90, 0));


        NetworkServer.Spawn(playerBase);
        print(playerBase.GetComponent<NetworkIdentity>().netId);
        baseId = playerBase.GetComponent<NetworkIdentity>().netId; 
        
        
    } 

    // Use this for initialization
    void Start () {
        if(isLocalPlayer)
        {
            
            _rigidbody = GetComponent<Rigidbody>();
            Camera.main.transform.position = transform.position - transform.forward * 10 + transform.up * 3;
            Camera.main.transform.LookAt(transform.position);
            Camera.main.transform.parent = transform;

        }
		
	}
    
    // Update is called once per frame
    [ClientCallback]
    void Update () {
        if (!isLocalPlayer)
            return;

       //every client should spawn their own base.
       if(ranOnce==false && ClientScene.ready )
        {
            CmdSpawnBase();
            ranOnce = true;

        } 


        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical") * 0.5f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z); 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Command function is called from the client, but invoked on the server
            CmdFire();
        }
      
    }
    

}

