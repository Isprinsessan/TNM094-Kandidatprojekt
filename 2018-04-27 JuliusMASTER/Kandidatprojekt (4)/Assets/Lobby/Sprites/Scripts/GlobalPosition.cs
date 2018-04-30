using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPosition : MonoBehaviour {

    public GameObject charScreen;
    private float x = 620f;
    private float y = 146;
    private float z = 0f;
	// Use this for initialization
	void Start () {

        charScreen.transform.position = new Vector3(x,y,z);

	}
  

}
