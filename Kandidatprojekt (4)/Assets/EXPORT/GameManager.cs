using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* This script is attached to GameManager (GameObject) in the Kandidaten scene
 * 
 * The purpose of this script is to restore values to the starting values (score =0, player position, player cooldowns, ...)
 * Normally these values are restored in this script, however due to how unity network work with calling [Command] and [ClientRpc]
 * we have to call a reset function in the playercontroller script.
 * 
 * This script is called when pressed on one of two buttons by host,
 * one button that appear when game over, button calls void in GameOverMenu script which then calls ResetGame() (this script)
 * one that is accessed from in game menu (directly connected to void ResetGame())
 * 
 */

public class GameManager : NetworkBehaviour {

    public PlayerController[] players;

    //this restore the game to 0 score and starting point, should reset cooldowns and remove CC, destroy all flags and spawn one
    public void ResetGame()
    {
        //Calls function to disable OptionsMenu canvas 
        FindObjectOfType<OptionsMenu>().Resume();

        //Find all of the player objects
        players = FindObjectsOfType<PlayerController>();

        //calls RpcResetValue for every player object( calling it more than once should not be necessary but didn't work as well)
        foreach(PlayerController player in players)
        {
            //if (player.gameObject == isServer) player.CmdResetValues();
            player.RpcResetValues();
            //TODO reset player cooldowns

        }
    }
}

