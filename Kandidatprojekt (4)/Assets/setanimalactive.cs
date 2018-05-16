using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class setanimalactive : NetworkBehaviour {

    public GameObject selfitem;
    public void setanimal(int i)
    {
        selfitem = selfitem.transform.GetChild(i).gameObject;
        selfitem.SetActive(true);
    }
   
}
