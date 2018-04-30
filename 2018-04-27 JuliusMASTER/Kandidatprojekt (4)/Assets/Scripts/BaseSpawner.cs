using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseSpawner : NetworkBehaviour
{
    //the base
    public GameObject playerBasePrefab;

    //dictionaries which store baseID and base spawn position by color
    public Dictionary<Color, NetworkInstanceId> baseIds = new Dictionary<Color, NetworkInstanceId>();
    public Dictionary<Color, NetworkStartPosition> spawns = new Dictionary<Color, NetworkStartPosition>();

    //store the base spawn positions
    public NetworkStartPosition[] spawnPoints;

    //initialize spawnPoint for bases as zero
    Vector3 spawnPoint = Vector3.zero;
    //counting how many bases have been spawned (used to give different spawn point to different bases spawnPoints[counter])
    private int counter = 0;

    //might be outdated counter used to make sure that all of the bases are colored for every client
    public int counter2 = 5;

    public bool hasSpawnedBases = false;

    private HashSet<Color> theColorList;

    private bool found = false;

    // Use this for initialization
    void Start()
    {
        //fetch all of the spawn position found in the in-game scene
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();

        if (!isServer)
            return;
    }

    [ClientCallback]
    private void Update()
    {
        //we are waiting for a playerobject to spawn, then the playerobject will call a void that changes this bool to true
        if (!found)
            return;

        //not very effective but running it once doesn't properly update the clients base colors (might be outdated)
        if (hasSpawnedBases && counter2 <= 10)
        {
            counter2++;
            foreach (Color col in baseIds.Keys) //find the base objects that were spawned and color them
            {
                var obj = (GameObject)NetworkServer.FindLocalObject(baseIds[col]);
                //calling command to color the bases, command then calls Rpc so the clients color the base
                CmdRunPaint(obj, col);

            }
        }
    }
    //command to server, server tells clients by calling rpc to color a base
    [Command]
    private void CmdRunPaint(GameObject obj, Color color)
    {
        RpcPaint(obj, color);
    }
    //color a base a certain color
    [ClientRpc]
    private void RpcPaint(GameObject obj, Color color)
    {
        foreach (Transform child in obj.transform)
        {
            if (child.CompareTag("BaseCenter"))
            {

                child.GetComponent<MeshRenderer>().material.color = color;

            }
        }
    }
    //spawn a base for every color in the color list
    [Command]
    private void CmdSpawnBase()
    {
        foreach (Color playerColor in theColorList)
        {
            //this if is used to make sure that one base is spawned per color
            if (!spawns.ContainsKey(playerColor))
            {
                //checking that the script has found spawn points and that spawnPoints array isn't accessed out of bounds
                if (spawnPoints != null && counter < spawnPoints.Length)
                {
                    //fetching a spawnpoint
                    spawnPoint = spawnPoints[counter].transform.position;
                    //add which color got this spawn to the dictionary
                    spawns.Add(playerColor, spawnPoints[counter]);
                    counter++; //increment to give different spawn to the next base 
                }

                //spawn the base on all connected clients
                var playerBase = (GameObject)Instantiate(
                        playerBasePrefab,
                        spawnPoint,
                         Quaternion.Euler(0, 0, 0));

                NetworkServer.Spawn(playerBase);


                //assign a base to a color
                if (!baseIds.ContainsKey(playerColor))
                {
                    //adding the newly spawned base in a dictionary connected by color
                    baseIds.Add(playerColor, playerBase.GetComponent<NetworkIdentity>().netId);
                    //doing the same thing as above except making so the list is updated for the clients as well (above only host)
                    RpcAddBaseId(playerColor, playerBase.GetComponent<NetworkIdentity>().netId);
                }
            }

        }
        //all of the bases has spawned 
        hasSpawnedBases = true;
    }
    //updating the baseID list for the clients as well
    //(so that it can be used later when stealing a flag from a base and updating score of affected base)
    [ClientRpc]
    public void RpcAddBaseId(Color playerColor, NetworkInstanceId baseId)
    {
        if (!baseIds.ContainsKey(playerColor))
        {
            baseIds.Add(playerColor, baseId);

        }

    }
    //is called once from the host player
    public void findControllerObject(GameObject obj)
    {
        found = true;
        //get the list of all the colors (teams)
        theColorList = obj.GetComponent<PlayerController>().colorList;
        //start spawning bases 
        CmdSpawnBase();
    }
}
