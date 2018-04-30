using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/*The purpose of this script is to spawn one text object for every team to keep score
 * This script is attached to ScoreManager(GameObject)
 * 
 * 
 */

public class ScoreMNG : NetworkBehaviour {

    //to store the score of a team by color
    public Dictionary<Color, int> scoreList;
    //to store which text belongs to which team
    public Dictionary<Color, Text> TextList;

    //a canvas in ScoreManager (GameObject)
    public Canvas parentCanv;

    //a textprefab that should be attached to the script
    public GameObject textPrefab;
    //reference to the text that displays game time remaining
    public GameObject timerText;

    //used to keep track on how many texts have been spawned (and spawn the texts at different positions)
    private int numText = 0;

    //win criteria number of flags to win the game, changed in inspector currently (later choose in lobby)
    //public int numFlagToWin = 0;

    //win criteria, game duration length in seconds, increased a little when stealing flags from players base(?)
    public float gameTime = 300; //game wont end if there is a tie
    public float extendedGameTime; // initialGameTime + increased time

    //the time the game started, used to compare if currentTime = startTime + gameTime 
    public double startTime = 0;

    //have some bool to choose between which win criteria?
    //public bool flagWinCriteria = false;

    public bool gameOver = false;

    //used in the foreach loops in update
    private bool largestValue = false;

    // Use this for initialization
    void Start () {
        TextList = new Dictionary<Color, Text>();
        scoreList = new Dictionary<Color, int>();
        extendedGameTime = gameTime;
    }
    [ServerCallback]
    private void Update()
    {
        if(startTime != 0)
        {
            //if the timer has reached 0, check if a team has more score than all the others
            if(Network.time >= (startTime + extendedGameTime))
            {
                foreach(Color col in scoreList.Keys)
                {
                    foreach(Color col2 in scoreList.Keys)
                    {

                        //if score of color col is larger than the other scores ->largestValue = true -> end game
                        if (scoreList[col] > scoreList[col2]) largestValue = true;
                        else
                        {
                            if (col != col2)
                            {
                                largestValue = false;
                                break;
                            }
                        }

                    }
                    if (largestValue) RpcWinGame(col); //call function to end the game

                }
                //if some team have equal score and timer ran out -> change timer to suddendeath
                if (!largestValue) RpcUpdateTimerText(0, 1);
            }
            else
            {
                RpcUpdateTimerText((float)((startTime + extendedGameTime) - Network.time), 0);
            }
        }

    }


    /*Called from PlayerController after the players' bases have spawned
     *Each player calls this to initialize a text object where score is displayed for their team 
     */
    [Command]
    public void CmdInitText(Color col)
    {
        //If a teammate has already called this function then don't do anything
        //Otherwise spawn a text object
        if(!TextList.ContainsKey(col))
        {
            var textParent = (GameObject)Instantiate(textPrefab,
                    textPrefab.transform.position,
                    Quaternion.Euler(0, 0, 0));

            NetworkServer.Spawn(textParent); //needs to use Command to spawn or error no active server
            //update textlist for the server (otherwise one text is spawned per player)
            TextList.Add(col, textParent.GetComponentInChildren<Text>());

            //have to call Rpc function to update clients textlist
            RpcAddTextToList(col, textParent);
        }
    }
    //initialize the text and add to textList
    [ClientRpc]
    public void RpcAddTextToList(Color col, GameObject textObj)
    {
        //Adds the newly created text object to TextList (must be done in Rpc otherwise clients' TextList is empty).
        if(!TextList.ContainsKey(col))
        TextList.Add(col, textObj.GetComponentInChildren<Text>());

        //Initialize score to be 0.
        if(!scoreList.ContainsKey(col))
        scoreList.Add(col, 0);

        //Make the text object a child of a specific canvas. (the canvas in ScoreManager(GameObject))
        textObj.transform.SetParent(parentCanv.transform, false);

        //position the score in the canvas, for every existing text, place the next text 15 units below
        //(so the text's don't stack on eachother)
        textObj.transform.position = textObj.transform.parent.position;
        textObj.GetComponentInChildren<Text>().transform.position += new Vector3(0, -15f * numText, 0);
        ++numText;

        //Change the color of the text and update what the text say
        TextList[col].color = col;
        TextList[col].text = "Score: " + scoreList[col];
    }

    //called by client to server wanting to update a team's score 
    //(yours if returning flag, someone elses score if stolen flag from a base)
    [Command]
    public void CmdUpdateText(Color col, int flags)
    {
        //server making sure the texts are updated on the clients
        RpcUpdateText(col, flags);
    }

    [ClientRpc]
    public void RpcUpdateText(Color col, int flags)
    {
        if(flags > scoreList[col])
            //visually show that a team got score
            TextList[col].rectTransform.Find("UpOneScore").GetComponent<TextSlideAndFade>().startSlideAndFade();
        else
            TextList[col].rectTransform.Find("DownOneScore").GetComponent<TextSlideAndFade>().startSlideAndFade();



        //updating scoreList and the text
        scoreList[col] = flags;

        TextList[col].color = col;
        TextList[col].text = "Score: " + scoreList[col];


        //if a team has enough flags to win -> activate game over screen
        /* if (scoreList[col] == numFlagToWin)
         {
             gameOver = true;
             var goM = FindObjectOfType<GameOverMenu>();

             goM.ToggleScreen(col);

         } */


    }


    /*These two are called when the host decides to restart the game
     * (either when game was won or for other reason chose to restart)
     * each player reset their teams score
     */
    [Command]
    public void CmdResetScore(Color col)
    {
        RpcResetScore(col);
    }
    [ClientRpc]
    public void RpcResetScore(Color col)
    { 
        scoreList[col] = 0;
        TextList[col].text = "Score: " + scoreList[col];

    }

    //Server calls this when the game time has ran out and one team has more flags than the others
    [ClientRpc]
    public void RpcWinGame(Color winnerColor)
    {
        gameOver = true;
        var goM = FindObjectOfType<GameOverMenu>();

        goM.ToggleScreen(winnerColor);
    }
    //Called from void update which runs on server, to display the remaining game duration for the clients
    [ClientRpc]
    public void RpcUpdateTimerText(float theTimer, int option) //option is used to differentiate which text should be displayed
    {
        if (option == 0) timerText.GetComponent<Text>().text = "Time: " + theTimer;
        else timerText.GetComponent<Text>().text = "Time: Over time";
    }
}
