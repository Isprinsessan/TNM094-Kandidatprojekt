using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/* This script is the main player script used to contact the server for updating score, 
 * telling bases to spawn, store baseID and team color, ...
 * 
 * Script is attached to player controlled object
 * 
 */
//[NetworkSettings(channel = 2, sendInterval = 0.2f)]
public class PlayerController : NetworkBehaviour
{
    
    public GameObject statusCanvas; //Can be used to display that a player has buffs from powerups, has flag, is cc'd
    public GameObject charobject;
    public GameObject bulletPrefab;
    private BaseSpawner BS;
    protected Rigidbody _rigidbody;
    public float moveSpeed = 10.0f;
    public Vector2 MoveVector { set; get; }
    public Joystick joystick;
    public Animator anim;
    public Canvas myJoyCanvas;
    public Canvas myButtonCanvas;
    public float VerticalMultiplier;
    public float HorizontalMultiplier;

    public AudioListener AL;


    private float inputH;
    private float inputV;

    public bool hasFlag = false;
    private bool ranOnce = false;
    private bool ranOnce2 = false;
    private bool gameStarted = false;
    private bool hasColored = false;
    private bool hasFound = false;
    public bool setPosition = false;
    public float distance = 100f;

    public bool stunned = false;
    private float StunTimer = 0f;
    //private float Stunduration = 2.5f;

    [SyncVar]
    Vector3 spawnPoint = Vector3.zero;

    [SyncVar]
    public Color color;
    //TODO, have the name above player (synced from lobby)
    [SyncVar]
    public string playerName;
    [SyncVar]
    public int charindex;


    //id of base to check if the current base is yours in BaseCollision
    [SyncVar]
    public NetworkInstanceId baseId;

    public HashSet<Color> colorList;

    void Start()
    {
        Debug.Log("entered start");
        //GetComponent<MeshRenderer>().material.color = color;
        if (isLocalPlayer)
        {

            /*   Debug.Log("entered islocalplayer");
               for (int i = 0; i < 3; i++)
               {
                   transform.GetChild(i).gameObject.SetActive(false);
               }
               transform.GetChild(charindex).gameObject.SetActive(true);

               charobject = charobject.transform.GetChild(charindex).gameObject;*/
            // Debug.Log("name");
            //Debug.Log(charobject.name); // blir rätt djur atm


            /*joystick = charobject.GetComponentInChildren<PlayerController>().joystick;
            _rigidbody = charobject.GetComponentInChildren<PlayerController>()._rigidbody;
            anim = charobject.GetComponentInChildren<PlayerController>().anim;
            myJoyCanvas = charobject.GetComponentInChildren<PlayerController>().myJoyCanvas;
            myButtonCanvas = charobject.GetComponentInChildren<PlayerController>().myButtonCanvas;
            AL = charobject.GetComponentInChildren<PlayerController>().AL;
            BS = charobject.GetComponentInChildren<PlayerController>().BS;*/

            joystick = joystick.GetComponent<Joystick>();
            //_rigidbody = _rigidbody.GetComponent<Rigidbody>();
            anim = anim.GetComponent<Animator>();
            myJoyCanvas = myJoyCanvas.GetComponent<Canvas>();
            myButtonCanvas = myButtonCanvas.GetComponent<Canvas>();
            AL = AL.GetComponent<AudioListener>();
            

            //AL = charobject.GetComponent<AudioListener>();
            AL.enabled = true;
            Debug.Log("joyname");
            Debug.Log(myJoyCanvas.gameObject.name); // blir rätt djur atm

            myJoyCanvas.gameObject.SetActive(true);
            myButtonCanvas.gameObject.SetActive(true);
            /*
            //camera setup
            Camera.main.transform.position = transform.position - transform.forward * 10 + transform.up * 3;
            Camera.main.transform.LookAt(transform.position);
            Camera.main.transform.parent = transform;*/
            //enable canvas element for right player            

        }

    }


    // Update is called once per frame



    void Update()
    {


        //these scripts can't be found at void Start() in this case so: if found && has not ran && is host player
        if (FindObjectOfType<BaseSpawner>() != null /*&& FindObjectOfType<worldGenerator>() != null*/ && !ranOnce && isServer)
        {
            //if the world has been spawned then spawn bases 
            /*if (FindObjectOfType<worldGenerator>().hasSpawnedWorld)
            {*/

            ranOnce = true;
            //send the player object to BaseSpawner to fetch the color list and then spawn a base for every color in the list
            FindObjectOfType<BaseSpawner>().findControllerObject(gameObject);

            //}
        }


        //when the script finds the basespawner and bases have been spawned then spawn texts which keep track of score
        if (FindObjectOfType<BaseSpawner>() != null && !hasFound && ClientScene.ready)
        {
            BS = FindObjectOfType<BaseSpawner>();

            if (BS.hasSpawnedBases && FindObjectOfType<ScoreMNG>() != null)
            {
                //spawn text to track score
                FindObjectOfType<ScoreMNG>().CmdInitText(color);
                hasFound = true;

                //fetch the spawn point for the player( to spawn at your base) and connect player with baseId
                CmdFetchSpawnPoint();
            }
        }

        // "Unique" operations that are different for different clients, therefore only the localplayer can go further than this line
        if (!isLocalPlayer)
            return;
        //all below this will only be done if is local player 
        //<---------------------------------------------------------------------------------------------------------->

        //Conditions that need to be met before letting the players play the game.
        if (!setPosition && spawnPoint != Vector3.zero) //if the spawnPoint has been fetched
        {
            //TODO spawn players on same team next to eacher (right now they get same position -> one of them flies away at spawn)

            transform.position = spawnPoint + new Vector3(0f, 10f, 0f);
            setPosition = true; //position has been set, don't run this if again
        }


        //past this point is game mechanics; moving

        if (stunned)
        {
            if (StunTimer > 0f) //control the stun duration
            {
                StunTimer -= Time.deltaTime;
            }
            else //timer reached 0, player should no longer be stunned;
            {
                //btn.interactable = true;
                stunned = false;
                ToggleStatusImages("stunnedImage", stunned);

            }

        }
        else //if not stunned then you are allowed to move
        {
            //keyboard input for movement and rotation
            Move(); // no animation and slower movement # just use joystick....

            //Joystick inputd
            // creates the movement vector for joystick
            MoveVector = PoolInput();
            //Joystick movement
            Movejoy();
            //Debug.Log(MoveVector); // display joystick values
            Animate();

            // stay close to ground
            RaycastHit hit;
            // check and adjust
            if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
            {
                toground(hit.point);
            }

        }

    }



    #region  Command functions
    //finding where to spawn this player based on which base it belong to (based on it's color) and find the baseId related to the players color
    [Command]
    void CmdFetchSpawnPoint()
    {
        //set the game startTime to be current time
        FindObjectOfType<ScoreMNG>().startTime = Network.time;

        if (BS.spawns.ContainsKey(color))
        {
            spawnPoint = BS.spawns[color].transform.position;
            baseId = BS.baseIds[color];
        }
        //at this point, the game should be loaded and therefore let the players play
        RpcStartGame();
    }
    //every player/client manually disables their "Loading..." canvas (easy solution but might not be optimal)
    //Might lead to some players being able to play before others
    [ClientRpc]
    void RpcStartGame()
    {
        FindObjectOfType<PreGameStartScreen>().StartGame();
    }

    //Stun a specific client for a duration
    [TargetRpc]
    public void TargetGetStunned(NetworkConnection target, float dur)
    {
        stunned = true;
        //if stunned while stunned update the timer if new stun is longer
        if (StunTimer < dur) //stuns wont stack but they will refresh
            StunTimer = dur;

        ToggleStatusImages("stunnedImage", stunned);
    }
    [Command]
    public void CmdGetStunned(GameObject targetObj, float dur) //Command calls Rpc because Rpc can only be called from server
    {
        TargetGetStunned(targetObj.GetComponent<NetworkIdentity>().connectionToClient, dur);
    }
    public void GetStunned(GameObject targetObj, float dur) //called from script on bear/tag collider to stun player
    {
        if (!isServer)
        {
            if (!isLocalPlayer) return;
            //CmdGiveAutho(targetObj);
            CmdGetStunned(targetObj, dur);
            //CmdRemoveAutho(targetObj);
        }
        else TargetGetStunned(targetObj.GetComponent<NetworkIdentity>().connectionToClient, dur);
        //the host does not need to go through Cmd (call to server), it goes directly to TargetRpc 
    }


    //stuff to reset when restarting game 
    [ClientRpc]
    public void RpcResetValues()
    {
        //a new game is started
        FindObjectOfType<ScoreMNG>().gameOver = false;
        //deactivate the game over canvas if it is active
        FindObjectOfType<GameOverMenu>().GameOverCanvas.SetActive(false);


        //destroy all the flags 
        GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");
        foreach (GameObject fl in flags)
        {
            Destroy(fl);
        }

        //the server spawns the starting flag and makes current time the start time
        if (isServer)
        {
            var scoreMNGref = FindObjectOfType<ScoreMNG>();
            scoreMNGref.startTime = Network.time;
            scoreMNGref.extendedGameTime = scoreMNGref.gameTime;

            FindObjectOfType<FlagSpawner>().spawnFlag();
        }

        //place player back on the spawn position and reset other values
        hasFlag = false;
        gameObject.transform.position = spawnPoint;
        stunned = false;
        StunTimer = 0;
        DisableAllStatusImageOnReset();

        //reset score (gives a warning Trying to send command for object without authority)
        if (!isServer)
        {
            if (!isLocalPlayer) return;

            var scoreManagerObj = FindObjectOfType<ScoreMNG>().gameObject;

            CmdGiveAutho(scoreManagerObj);
            FindObjectOfType<ScoreMNG>().CmdResetScore(color);
            CmdRemoveAutho(scoreManagerObj);


        }
        else FindObjectOfType<ScoreMNG>().RpcResetScore(color);
    }

    //give authority over the ScoreManager gameobject and update score
    [Command]
    public void CmdgiveScoreAndAutho(Color baseCol, int flags)
    {
        var scoreManagerObj = FindObjectOfType<ScoreMNG>();
        //same as CmdGiveAutho
        scoreManagerObj.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

        FindObjectOfType<ScoreMNG>().RpcUpdateText(baseCol, flags);

        //same as CmdRemoveAutho
        scoreManagerObj.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }
    public void giveScoreAndAutho(Color baseCol, int flags) //called everytime score should be changed; stealing someones flag, returning a flag
    {
        if (isServer) FindObjectOfType<ScoreMNG>().RpcUpdateText(baseCol, flags);
        else
        {
            if (!isLocalPlayer)
                return;
            CmdgiveScoreAndAutho(baseCol, flags);
        }
    }


    //give authority over an object to a player
    [Command]
    public void CmdGiveAutho(GameObject authoObject)
    {
        authoObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

    }
    //remove authority over an object from a player
    [Command]
    public void CmdRemoveAutho(GameObject authoObject)
    {
        authoObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }

    //called when returning a flag for the first time, give authority (unsure if necessary) -> spawn flag -> remove authority
    public void spawnFlag(GameObject flag)
    {
        if (!isLocalPlayer) return;
        CmdGiveAutho(flag);
        CmdSetCaptured(flag);
        CmdRemoveAutho(flag);
    }

    //a flag has been returned to a base for the first time -> spawn a new flag
    [Command]
    void CmdSetCaptured(GameObject obj)
    {
        RpcSetCaptured(obj);
        FindObjectOfType<FlagSpawner>().spawnFlag();
    }
    [ClientRpc]
    void RpcSetCaptured(GameObject obj)
    {
        obj.GetComponent<OnCollision>().beenCaptured = true;
    }

    //called to increase game duration
    public void addTime(float timeToAdd)
    {
        if (isServer) FindObjectOfType<ScoreMNG>().extendedGameTime += timeToAdd;

    }

    //used to activate or deactivate status icons (cc'd, has flag, has powerup)
    public void ToggleStatusImages(string imageName, bool toggle)
    {
        if (!isLocalPlayer)
            return;

        foreach (Transform child in statusCanvas.transform)
        {
            if (child.name == imageName)
                child.gameObject.SetActive(toggle);
        }

    }
    // When the game is reset, disable all the player status images
    public void DisableAllStatusImageOnReset()
    {
        foreach (Transform child in statusCanvas.transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    #endregion

    #region Movementfunctions
    // keyboard input handle
    void Move()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f * 5f;
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

    }

    // stay on ground function

    void toground(Vector3 hitpoint)
    {
        Vector3 targetLocation = hitpoint;
        targetLocation += new Vector3(0, transform.localScale.y / 2, 0);
        transform.position = targetLocation;

    }
    // creates the movement vector for joystick
    public Vector2 PoolInput()
    {
        Vector2 dir = Vector2.zero;

        //Får ut x och z från Joystick
        dir.x = joystick.Horizontal;
        dir.y = joystick.Vertical;


        //Normaliserar vektorn för att ha värden mellan -1 och 1
        if (dir.magnitude > 1)
            dir.Normalize();

        return dir;

    }
    // what creates movement for character  from joystick
    public void Movejoy()
    {
        if (MoveVector != Vector2.zero)
        {
            // Vector3 translateVector = (transform.right * -joystick.Horizontal + transform.forward * joystick.Vertical);
            //transform.Translate(translateVector * moveSpeed * Time.deltaTime);

            //styrning  fram,back, rot
            //transform.Translate(0, 0, MoveVector.y);

            //tar in kamerans värde
            var camera1 = Camera.main;

            //tar camerans x och y värden
            var forward = camera1.transform.forward;
            var right = camera1.transform.right;

            //sätter y till 0 på båda, så den inte flyger
            forward.y = 0;
            right.y = 0;
            //normaliserar
            forward.Normalize();
            right.Normalize();
            //sätter riktningen vi vill åt
            var desiredMoveDirection = forward * MoveVector.y + right * MoveVector.x;
            //flyttar gubben till rätt ställe
            print(desiredMoveDirection);
            //roterar gubben i rätt riktning
            transform.eulerAngles = new Vector3(0, Mathf.Atan2(desiredMoveDirection.x, desiredMoveDirection.z) * Mathf.Rad2Deg, 0);

            //flyttar gubben åt rätt håll
            transform.Translate(0, 0, Mathf.Abs(MoveVector.y / 2) + Mathf.Abs(MoveVector.x)/2 );
            //transform.Translate(desiredMoveDirection * moveSpeed * Time.deltaTime);

        }

    }



    // animate player depending on input from joystick
    public void Animate()
    {
        if (MoveVector == Vector2.zero)
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
        }
        else if (MoveVector.x < -0.5 || MoveVector.y < -0.5 || MoveVector.x > 0.5 || MoveVector.y > 0.5)
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", false);
        }


    }

    #endregion
}
