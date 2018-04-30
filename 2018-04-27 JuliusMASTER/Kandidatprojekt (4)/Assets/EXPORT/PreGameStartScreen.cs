using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/* This script manages a canvas that is disabled when all the necessary objects have loaded 
 * This canvas should have a higher sort order than all other canvases
 */


public class PreGameStartScreen : MonoBehaviour {

    public GameObject StartScreenCanvas;
    //text to display who won
    public Text highText;



    //used to update a text to illustrate that game is loading
    private float timer = 0f;
    private int counter = 0;


    private bool GameStarted = false;
	
	// Update is called once per frame
	void Update () {
        if (GameStarted) return;
        

        if (timer <= 0f)
        {
            timer = 0.5f;
            highText.text = "Loading";
            for (int i = 1; i <= counter; i++)
            {
                highText.text += ".";
            }
            if (counter > 2) counter = 0;
            else ++counter;


        }
        else
        {
            timer -= Time.deltaTime;
        }

    }

    //called individually by every player when game is ready to be played, tried with Rpc but -> NullReferenceException occured
    public void StartGame()
    {
        StartScreenCanvas.SetActive(false);
        GameStarted = true;
    }


}
