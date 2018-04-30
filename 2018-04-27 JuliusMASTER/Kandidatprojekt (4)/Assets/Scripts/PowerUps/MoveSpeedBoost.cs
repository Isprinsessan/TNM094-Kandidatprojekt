using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoveSpeedBoost : NetworkBehaviour
{
    public GameObject[] PowerUps;

    public float BuffDuration = 5.0f;
    public float RespawnDelay = 5.0f;
    public float SpeedMultiplier = 10.0f;
    //public GameObject pickupEffect;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //print("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            //Ger speed boost
            StartCoroutine(Speed(other));
            print("YOLO" + SpeedMultiplier);
            print(other.GetComponent<PlayerController>().VerticalMultiplier);

            //Spawnar en ny Power Up
            //StartCoroutine(SpawnPowerUps(other));
        }
           
    }

    IEnumerator Speed(Collider player)
    {
        //Effekt för pickup (inte inlagt)
        //Instantiate(pickupEffect, transform.position, transform.rotation);
        print("hastighet innan multiplier");
        print(player.GetComponent<PlayerController>().VerticalMultiplier);
        print(SpeedMultiplier);
        //Ändra hastighet
        player.GetComponent<PlayerController>().HorizontalMultiplier *= SpeedMultiplier;
        player.GetComponent<PlayerController>().VerticalMultiplier *= SpeedMultiplier;
        player.GetComponent<PlayerController>().moveSpeed *= SpeedMultiplier;

        //Gör objektet osynligt och untriggerable
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        print("SpeedMultiplier vid pickup");
        print(SpeedMultiplier);
        print(player.GetComponent<PlayerController>().VerticalMultiplier);

        //Vänta TIME sekunder
        yield return new WaitForSeconds(BuffDuration);

        //Ändra tillbaka hastigheten
        print("INNAN " + SpeedMultiplier);
        player.GetComponent<PlayerController>().HorizontalMultiplier /= SpeedMultiplier;
        player.GetComponent<PlayerController>().VerticalMultiplier /= SpeedMultiplier;
        //print(player.GetComponent<PlayerController>().VerticalMultiplier /= SpeedMultiplier);
        print(player.name);
        //print(player.GetComponent<PlayerController>().VerticalMultiplier *= SpeedMultiplier);
        print(player.GetComponent<PlayerController>().VerticalMultiplier);
        //player.GetComponent<PlayerController>().moveSpeed /= SpeedMultiplier;
        print("Hastighet efter multipliern tagits bort");
        print(SpeedMultiplier);

    }

    IEnumerator SpawnPowerUps(Collider player)
    {
        //Tar plats och rotation från den tidigare PowerUpen
        Vector3 currentPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        Quaternion currentRot = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, 1);
        //Skapar ett nytt objekt som är en kopia av en i PowerUps-arrayen, som randomas
        //print(PowerUps[1]);
        //print(currentPos);
        //print(currentRot); [Random.Range(0,PowerUps.Length)]
        //print("SpeedMultiplier för nya Powerup");
        //print(SpeedMultiplier);
        GameObject newSpawn = Instantiate(PowerUps[1], currentPos, currentRot);
        newSpawn.transform.localScale *= 3;  
        newSpawn.GetComponent<MoveSpeedBoost>().PowerUps = PowerUps;
        print("I newSpawn");
        print(newSpawn.GetComponent<MoveSpeedBoost>().SpeedMultiplier);
        //print(newSpawn.GetComponent<PlayerController>().VerticalMultiplier);
        //newSpawn.GetComponent<MoveSpeedBoost>().SpeedMultiplier /= SpeedMultiplier;
        print(newSpawn.GetComponent<MoveSpeedBoost>().SpeedMultiplier);
        print("Spelarens hastighet i poerupspawn");
        print(player.GetComponent<PlayerController>().VerticalMultiplier);
        //print("SpeedMultiplier för nya Powerup efter den är lika som orginalet");

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
