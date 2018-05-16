using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : MonoBehaviour {

    public GameObject[] PowerUps;
    private GameObject[] teleportPositions;
    private Vector3 teleportedPos;

    public float RespawnDelay = 5.0f;


    void OnTriggerEnter(Collider other)
    {
        PickUp(other);
        print(PowerUps.Length);
        StartCoroutine(SpawnPowerUps());
    }

    void PickUp(Collider player)
    {
        //Effect

        //Apply to player
        teleportPositions = GameObject.FindGameObjectsWithTag("TeleportPoints");
        //teleportedPos = teleportPositions[Random.Range(0, teleportPositions.Length-1)].transform.position;
        player.transform.position = teleportedPos;
       

        if(player.transform.position == teleportedPos)
        {
            Destroy(gameObject);
        }
        
    }
    IEnumerator SpawnPowerUps()
    {
        //Tar plats och rotation från den tidigare PowerUpen
        Vector3 currentPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        Quaternion currentRot = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, 1);

        //Skapar ett nytt objekt som är en kopia av en i PowerUps-arrayen, som randomas
        //print(PowerUps[1]);
        //print(currentPos);
        //print(currentRot);
        GameObject newSpawn = Instantiate(PowerUps[Random.Range(0, PowerUps.Length)], currentPos, currentRot);
        newSpawn.GetComponent<PowerUp>().PowerUps = PowerUps;
        //Gör objektet osynligt och untriggerable
        newSpawn.GetComponent<MeshRenderer>().enabled = false;
        newSpawn.GetComponent<Collider>().enabled = false;

        //Vänta RespawnDelay sekunder innan den görs synlig och interactable
        yield return new WaitForSeconds(RespawnDelay);

        newSpawn.GetComponent<MeshRenderer>().enabled = true;
        newSpawn.GetComponent<Collider>().enabled = true;

        //Ta bort det gamla objektet
        //Destroy(gameObject);

    }

}
