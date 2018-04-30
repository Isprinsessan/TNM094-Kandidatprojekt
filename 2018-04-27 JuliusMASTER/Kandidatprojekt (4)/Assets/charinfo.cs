using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/*
 * Need to find some bool or something before i change the color on the bases and player spawnpoint...
 * 
 * 
 */
public class charinfo : NetworkBehaviour
{
   

   
    public bool hasFlag = false;
    private bool ranOnce = false;
    private bool ranOnce2 = false;
    private bool hasColored = false;
    private bool hasFound = false;
    public bool setPosition = false;
    public float distance = 100f;

    //private float Stunduration = 2.5f;

    [SyncVar]
    Vector3 spawnPoint = Vector3.zero;

    [SyncVar]
    public Color color;
    [SyncVar]
    public string playerName;
    [SyncVar]
    public int charindex;


    //id of base to check if the current base is yours in BaseCollision
    [SyncVar]
    public NetworkInstanceId baseId;
    //id of player
    public NetworkInstanceId playerId;

    //list of teammates.
    //public List<GameObject> myTeam = new List<GameObject>();

    public HashSet<Color> colorList;

    //we sync the score to be able to show it to every other client (just a thought not 100% sure it's implemented like this)
    [SyncVar]
    public int score = 0;

    void Start()
    {
        

        //GetComponent<MeshRenderer>().material.color = color;
        if (isLocalPlayer)
        {
            for (int i = 0; i < 3; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(charindex).gameObject.SetActive(true);

            //
            // myJoyCanvas = transform.GetChild(charindex).gameObject.GetComponentInChildren<Canvas>();
            //transform.GetChild(charindex).GetComponentInChildren<Canvas>();

            //camera setup
            Camera.main.transform.position = transform.position - transform.forward * 10 + transform.up * 3;
            Camera.main.transform.LookAt(transform.position);
            Camera.main.transform.parent = transform;
            //enable canvas element for right player

            //myJoyCanvas = charobject.transform.GetChild(charindex).Find()

            //("VirtualJoystick");

        
            //find("VirtualJoystick");

            //myJoyCanvas.gameObject.SetActive(true);

            //myButtonCanvas = myButtonCanvas.GetComponent<Canvas>();
            //myButtonCanvas.gameObject.SetActive(true);

        }

    }








    
}




