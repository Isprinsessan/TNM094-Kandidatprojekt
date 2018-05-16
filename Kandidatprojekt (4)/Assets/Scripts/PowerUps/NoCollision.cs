using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NoCollision : NetworkBehaviour {

    public float Duration = 5.0f;
    //public GameObject pickupEffect;

    //void OnTriggerEnter(Collider other)
    //{
    //    StartCoroutine(Immortal(other));
    //}

    //IEnumerator Immortal(Collider player)
    //{
    //    //effekt för pickup (inte inlagt)
    //    //instantiate(pickupeffect, transform.position, transform.rotation);

    //    //ändra hastighet
    //    player.getcomponent<playercontroller>().
    //    player.getcomponent<playercontroller>().


    //    //gör objektet osynligt och untriggerable
    //    getcomponent<meshrenderer>().enabled = false;
    //    getcomponent<collider>().enabled = false;

    //    //vänta time sekunder
    //    yield return new waitforseconds(duration);

    //    //ändra tillbaka hastigheten
    //    player.getcomponent<playercontroller>().horizontalmultiplier /= speedmultiplier;
    //    player.getcomponent<playercontroller>().verticalmultiplier /= speedmultiplier;

    //    //ta bort powerupen
    //    destroy(gameobject);

    }
