using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spinning : MonoBehaviour {
     public float speed = 10f;
    //// Update is called once per frame
    //void Update () {
    //       transform.Rotate( new Vector3(1, 1, 0), speed * Time.deltaTime);		
    //}
    void OnMouseDown()
    {
        transform.Rotate(0, 70, 0);
        //for (int i = 0; i < 1000; ++i)
        //{
        //    transform.Rotate(new Vector3(1, 1, 0), speed * Time.deltaTime);
        //}
    }
    //void Update()
    //{
    //    transform.Rotate(new Vector3(1, 1, 0), speed * Time.deltaTime);
    //}

}
