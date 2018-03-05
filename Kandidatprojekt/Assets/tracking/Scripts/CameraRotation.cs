using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;


public class CameraRotation : MonoBehaviour {
    Camera MainCamera;
    public Canvas varningPanel;
    public Text printobj;
    Vector3 pos;
    Vector3 origo;
 
    public Text printVarning;
	// Use this for initialization
	void Start () {
        
        MainCamera = Camera.main;
        printobj = GameObject.Find("ConsolePrint").GetComponent<Text>();
       
        varningPanel = GameObject.Find("CanvasVarning").GetComponent<Canvas>();
        varningPanel.gameObject.SetActive(false);
        origo = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

        //rot = MainCamera.transform.Rotation;
        pos = MainCamera.WorldToScreenPoint(origo);
        printobj.text =   pos.ToString();
        if(pos.z > 600){

            varningPanel.gameObject.SetActive(true);

        }
        else{
            varningPanel.gameObject.SetActive(false);

        }
       

	}
}
