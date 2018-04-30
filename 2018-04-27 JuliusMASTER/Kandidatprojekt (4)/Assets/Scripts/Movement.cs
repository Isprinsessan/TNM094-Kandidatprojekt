using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Movement : NetworkBehaviour
{
  
    #region Bullet
    public GameObject bulletPrefab;
    public GameObject playerBasePrefab;
    #endregion
 
    #region Flag and base init
    public bool hasFlag = false;

    private bool ranOnce = false;
    
    //id of base to check if the current base is yours in BaseCollision
    public NetworkInstanceId baseId;

    //we sync the score to be able to show it to every other client (just a thought not 100% sure it's implemented like this)
    [SyncVar]
    public int score = 0;
    #endregion

    #region Player init
    // Reference used to move the tank.
    private Rigidbody m_Rigidbody;
    [SyncVar]
    public Color color;
    [SyncVar]
    public string playerName;
    #endregion
    
    #region movment init
    //Variables for the server to set this tank's speed and turn rate from the TankLibrary and cascade them to clients when spawning the tank.
    [SyncVar]
    private float m_OriginalSpeed = 0f;
    [SyncVar]
    private float m_OriginalTurnRate = 0f;

    [SyncVar]
    // How fast the tank moves forward and back. We sync this stat from server to prevent local cheatery.
    private float m_Speed = 12f;
    [SyncVar]
    // How fast the tank turns in degrees per second. We sync this stat from server to prevent local cheatery.
    private float m_TurnSpeed = 180f;

    //The direction that the player wants to move in.
    private Vector2 m_DesiredDirection;

    //The tank's position last tick.
    private Vector3 m_LastPosition;

    private MovementMode m_CurrentMovementMode;

    #endregion

    // tanksscript likt
    #region Tanksinspo
    //Enum to define how the tank is moving towards its desired direction.
    public enum MovementMode
    {
        Forward = 1,
        Backward = -1
    }

    public float speed
    {
        get
        {
            return m_Speed;
        }
    }


    public Rigidbody Rigidbody
    {
        get
        {
            return m_Rigidbody;
        }
    }

    public MovementMode currentMovementMode
    {
        get
        {
            return m_CurrentMovementMode;
        }
    }

    //Whether the tank was undergoing movement input last tick.
    private bool m_HadMovementInput;

    //The final velocity of the tank.
    public Vector3 velocity
    {
        get;
        protected set;
    }

    //Whether the tank is moving.
    public bool isMoving
    {
        get
        {
            return m_DesiredDirection.sqrMagnitude > 0.01f;
        }
    }

    #endregion

    private int counter = 0;
    
    
    //Accepts a TankManager reference to pull in all necessary data and references.
        public void Init()
        {
            enabled = false;
            //m_TankDisplay = manager.display; //display = tankDisplay.GetComponent<TankDisplay>();
            m_OriginalSpeed = m_OriginalSpeed;//manager.playerTankType.speed;
            m_OriginalTurnRate = m_OriginalTurnRate;//manager.playerTankType.turnRate;

            SetDefaults();
        }
        //Called by the active tank input manager to set the movement direction of the tank.
        public void SetDesiredMovementDirection(Vector2 moveDir)
        {
            m_DesiredDirection = moveDir;
            m_HadMovementInput = true;

            if (m_DesiredDirection.sqrMagnitude > 1)
            {
                m_DesiredDirection.Normalize();
            }
        }

        private void Awake()
        {
            //Get our rigidbody, and init originalconstraints for enable/disable code.
            LazyLoadRigidBody();
            m_OriginalConstrains = m_Rigidbody.constraints;

            m_CurrentMovementMode = MovementMode.Forward;

        }

        private void LazyLoadRigidBody()
        {
            if (m_Rigidbody != null)
            {
                return;
            }

            m_Rigidbody = GetComponent<Rigidbody>();
        }


        // Use this for initialization
        void Start()
            {
                GetComponent<MeshRenderer>().material.color = color;
                if (isLocalPlayer)
                {
                    m_Rigidbody = GetComponent<Rigidbody>();
                    Camera.main.transform.position = transform.position - transform.forward * 10 + transform.up * 3;
                    Camera.main.transform.LookAt(transform.position);
                    Camera.main.transform.parent = transform;

                }
                m_LastPosition = transform.position;


    }

        // Update is called once per frame
        [ClientCallback]
        void Update()
        {
            //this ensures that the player that is client&server updates their base color to the others
            if (isServer && counter <= 100 && ranOnce == true) //counter is temporary solution, didn't work consistently with using a bool
            {
                counter++; //ideally we want to run this if function once. Some reason server is not ready?
                RpcPaint(NetworkServer.FindLocalObject(baseId), color);
            }

            if (!isLocalPlayer)
                return;

            //every client should spawn their own base.
            if (ranOnce == false && ClientScene.ready)
            {
                CmdSpawnBase();
                ranOnce = true;

            }

             if (hasAuthority)
             {
                if (!m_HadMovementInput || !isMoving)
                {
                    m_DesiredDirection = Vector2.zero;
                }
                m_HadMovementInput = false;
             }

            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            


            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Command function is called from the client, but invoked on the server
                CmdFire();
            }

        }


        private void FixedUpdate()
        {
            velocity = transform.position - m_LastPosition;
            m_LastPosition = transform.position;

            if (!hasAuthority)
            {
                return;
            }

            // Adjust the rigidbody's position and orientation in FixedUpdate.
            if (isMoving)
            {
                Turn();
                Move();
            }
        }
        private void Move()
        {
            float moveDistance = m_DesiredDirection.magnitude * m_Speed * Time.deltaTime;

            // Create a movement vector based on the input, speed and the time between frames, in the direction the tank is facing.
            Vector3 movement = m_CurrentMovementMode == MovementMode.Backward ? -transform.forward : transform.forward;
            movement *= moveDistance;

            // Apply this movement to the rigidbody's position.
            // Also immediately move our transform so that attached joints update this frame
            m_Rigidbody.position = m_Rigidbody.position + movement;
            transform.position = m_Rigidbody.position;
        }


        private void Turn()
        {
            // Determine turn direction
            float desiredAngle = 90 - Mathf.Atan2(m_DesiredDirection.y, m_DesiredDirection.x) * Mathf.Rad2Deg;

            // Check whether it's shorter to move backwards here
            Vector2 facing = new Vector2(transform.forward.x, transform.forward.z);
            float facingDot = Vector2.Dot(facing, m_DesiredDirection);

            // Only change if the desired direction is a significant change over our current one
            if (m_CurrentMovementMode == MovementMode.Forward &&
                facingDot < -0.5)
            {
                m_CurrentMovementMode = MovementMode.Backward;
            }
            if (m_CurrentMovementMode == MovementMode.Backward &&
                facingDot > 0.5)
            {
                m_CurrentMovementMode = MovementMode.Forward;
            }
            // currentMovementMode =  >= 0 ? MovementMode.Forward : MovementMode.Backward;

            if (m_CurrentMovementMode == MovementMode.Backward)
            {
                desiredAngle += 180;
            }

            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            float turn = m_TurnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis.
            Quaternion desiredRotation = Quaternion.Euler(0f, desiredAngle, 0f);

            // Approach that direction
            // Also immediately turn our transform so that attached joints update this frame
            m_Rigidbody.rotation = Quaternion.RotateTowards(m_Rigidbody.rotation, desiredRotation, turn);
            transform.rotation = m_Rigidbody.rotation;
        }

        // This function is called at the start of each round to make sure each tank is set up correctly.
        public void SetDefaults()
        {
            enabled = true;
            ResetMovementVariables();
            LazyLoadRigidBody();

            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;

            m_DesiredDirection = Vector2.zero;
            m_CurrentMovementMode = MovementMode.Forward;
        }

        //Disable movement, and also disable our engine noise emitter.
        public void DisableMovement()
        {
            m_Speed = 0;
           
        }

        //Reenable movement, and also the engine noise emitter.
        public void EnableMovement()
        {
            m_Speed = m_OriginalSpeed;
        }

        //We freeze the rigibody when the control is disabled to avoid the tank drifting!
        protected RigidbodyConstraints m_OriginalConstrains;

        //On disable, lock our rigidbody in position.
        void OnDisable()
        {
            m_OriginalConstrains = m_Rigidbody.constraints;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        //On enable, restore our rigidbody's range of movement.
        void OnEnable()
        {
            m_Rigidbody.constraints = m_OriginalConstrains;
        }
        void ResetMovementVariables()
        {
            m_Speed = m_OriginalSpeed;
            m_TurnSpeed = m_OriginalTurnRate;
        }






    //every player should have a base
    [Command]
        void CmdSpawnBase()
        {
            var playerBase = (GameObject)Instantiate(
                 playerBasePrefab,
                 transform.position - transform.forward * 5.5f,
                  Quaternion.Euler(0, transform.rotation.y + 90, 0));


            //NetworkServer.Spawn(playerBase);
            NetworkServer.SpawnWithClientAuthority(playerBase, gameObject);
            //foreach() child change the color, doesn't work over network

            //playerBase.GetComponent<MeshRenderer>().material.color = color; //trying to make base same color
            print(playerBase.GetComponent<NetworkIdentity>().netId);
            baseId = playerBase.GetComponent<NetworkIdentity>().netId;


            //trying to paint 
            RpcPaint(playerBase, color);

            playerBase.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
        }

        [ClientRpc]
        void RpcPaint(GameObject obj, Color col)
        {
            foreach (Transform child in obj.transform)
            {
                if (child.CompareTag("BaseCenter"))
                {
                    child.GetComponent<MeshRenderer>().material.color = col;
                }
            }
            // this is the line that actually makes the change in color happen

        }

        [Command]
        void CmdFire()
        {
            // This [Command] code is run on the server!

            // create the bullet object locally
            var bullet = (GameObject)Instantiate(
                 bulletPrefab,
                 transform.position + transform.forward,
                 Quaternion.identity);

            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 4;

            // spawn the bullet on the clients
            NetworkServer.Spawn(bullet);

            // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
            Destroy(bullet, 2.0f);
        }


}

