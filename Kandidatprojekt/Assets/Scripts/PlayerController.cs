using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerController : NetworkBehaviour {

    public GameObject bulletPrefab;

    public float rotationSpeed = 45.0f;
    public float speed = 2.0f;
    public float maxSpeed = 3.0f;

    protected Rigidbody _rigidbody;

    protected float _rotation = 0;
    protected float _acceleration = 0;

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
        /*
        _rotation = Input.GetAxis("Horizontal");
        //_acceleration = Input.GetAxis("Vertical");

        //stolen code from NetworkSpaceship
        Quaternion rotation = _rigidbody.rotation * Quaternion.Euler(0, _rotation * rotationSpeed * Time.fixedDeltaTime, 0);
        _rigidbody.MoveRotation(rotation);

        //_rigidbody.AddForce((rotation * new Vector3(0,0,1)) * _acceleration * 1000.0f * speed * Time.deltaTime);
        
        if (_rigidbody.velocity.magnitude > maxSpeed * 1000.0f)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed * 1000.0f;
        } */
        //end of stolen code section


        var x = Input.GetAxis("Horizontal") * 0.1f;
        var z = Input.GetAxis("Vertical") * 0.1f;

        transform.Translate(x, 0, z); 

        if (Input.GetKeyDown(KeyCode.Space))
        {// Command function is called from the client, but invoked on the server
            CmdFire();
        }
    }
    
}

