 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour {

    public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hookable")
        {
            print("Colliding box");
            player.GetComponent<GrapplingHook>().hooked = true;
            player.GetComponent<GrapplingHook>().hookedObj = other.gameObject;
        }
    }
}
