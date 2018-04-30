using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlyLocalCanvas : NetworkBehaviour
{
    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponentInChildren<Canvas>().enabled = true;
        }
    }
}