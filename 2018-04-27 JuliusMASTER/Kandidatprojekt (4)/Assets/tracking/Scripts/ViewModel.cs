using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Vuforia;
using UnityEngine.UI;

public class ViewModel : MonoBehaviour
{
    //Initilize camera
    Camera MainCamera;
    //private Color mClearColor = new Color(0, 0, 0, 0);
    public void toggle_changed(bool cameraActive){
        //Sätter så Maincamera är kameran som används
        MainCamera = Camera.main;
        if(cameraActive){
            //sätter på kamera feed
            MainCamera.clearFlags = CameraClearFlags.Depth;
        }
        else{
            //sätter skybox
            MainCamera.clearFlags = CameraClearFlags.Skybox;  
        }
       
    }
}
