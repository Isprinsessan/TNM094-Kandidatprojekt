using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

/* This script is attached to OptionsMenu (GameObject)
 * The script is called from a button that is always visible (top right?)
 * The purpose is to show a menu where a player can quit or restart the game (if player is host)
 * 
 * 
 */


public class OptionsMenu : NetworkBehaviour {

    public bool theCanvasIsActive = false;

    //OptionMenu 's canvas
    public GameObject MenuCanvas;
    //the button that only the host can see( to reset game)
    public GameObject resetBtn; 

    //only runs on the server (host player), sets the button that can reset the game active
    [ServerCallback]
    void Start () {
        resetBtn.SetActive(true);
	}

    //Can be called from button on OptionsMenu 's canvas to disable the canvas
    public void Resume()
    {
        MenuCanvas.SetActive(false);
        theCanvasIsActive = false; //unsure if used
    }
    
    //quit to Lobby scene (reached by pressing button on OptionsMenu )
    public void Quit(int scene)
    {
        //all of the code here is trying to find "BackButton" and activate it's onClick
        //the onClick uses some script to load Lobby scene properly
        var parentObj = GameObject.Find("LobbyManager");
        foreach (Transform child in parentObj.transform)
        {
            if (child.name == "TopPanel")
            {
                foreach (Transform child2 in child)
                {
                    if (child2.name == "BackButton")
                    {
                        child2.gameObject.SetActive(true);
                        child2.GetComponent<Button>().onClick.Invoke();
                    }
                }

            }
        }

    }
    //if menu canvas isn't active then activate it
    public void OpenMenu() //is called when option button is pressed. (a button which is always visible (in top right?))
    {
        if(!theCanvasIsActive)
        {
            MenuCanvas.SetActive(true); //show the pause menu
            theCanvasIsActive = true;
        }
    }
}
