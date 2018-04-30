using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/* This script is attached to GameOverMenu(GameObject)
 * This script is called when the game is won by a team
 * The purpose is to give choice to return to lobby scene or restart game (if host)
 */

public class GameOverMenu : NetworkBehaviour {
    //the canvas that is displayed when game is won
    public GameObject GameOverCanvas;
    //text to display who won
    public Text highText;
    //button to restart the game (only available for host)
    public GameObject resetBtn;

    //only runs on the server (host player), sets the button that can reset the game active
    [ServerCallback]
    void Start()
    {
        resetBtn.SetActive(true);
    }

    //is called when a team won, activates the game over canvas and displays who won (by showing the team color)
    public void ToggleScreen(Color winner) //called from ScoreMNG: void RpcUpdateText(Color col, int flags)
    {
        highText.text = "Team Won";
        highText.color = winner;
        GameOverCanvas.SetActive(true);

    }
    //called by the resetBtn when clicked (host only) calls GameManager to reset everything
    [ClientRpc]
    public void RpcResetGame()
    {
        GameOverCanvas.SetActive(false); //disables the Game over menu
        FindObjectOfType<GameManager>().ResetGame();
    }

    //quit to Lobby scene (reached by pressing button on GameOverMenu )
    public void QuitToMenu(int scene) //same function as found in OptionsMenu script :void Quit(int scene)
    {
        //disable the GameOver screen canvas
        GameOverCanvas.SetActive(false);
        //all of the code here is trying to find "BackButton" and activate it's onClick
        //the onClick uses some script to load Lobby scene properly
        var parentObj = GameObject.Find("LobbyManager");
        foreach(Transform child in parentObj.transform)
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
   
}
