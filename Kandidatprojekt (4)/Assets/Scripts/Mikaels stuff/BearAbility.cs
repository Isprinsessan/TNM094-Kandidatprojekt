using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/*
 * this script inherits PlayerAttack and you need to attach TagAbilityCollider and
 * BearAbilityCollider to the bear player as children as well as reference them to this script.
 * 
 * 
 */

public class BearAbility : PlayerAttack
{

    protected bool roaring = false;
    
    protected float BearAbilityTimer = 0f;
    protected float RoarDurationTimer = 0f;

    public Button roarButton;
    public AudioSource roarSound;

    protected float RoarCd = 10f; //Cooldown for the attack
    protected float RoarDuration = 1f; //The attack's duration, amount of time that the BearAbilityCollider is enabled

    public Collider BearAbilityTrigger;

    void Start()
    {
        thePlayer = gameObject.GetComponent<PlayerController>();
        attackTrigger.enabled = false; //we start by having the attack disabled until we attack
        BearAbilityTrigger.enabled = false;

        roarButton.onClick.AddListener(StartRoar);
        
        //You may only attack if you are not already attacking and you are not stunned
        tagButton.onClick.AddListener(StartTag);
    }

    //Unsure if it is necessary to have the tag [ClientCallback] here.
    void Update()
    {
        if (!isLocalPlayer)
            return;

        CheckAttack();
        
        
        /*
        //You may only attack if you are not already attacking and you are not stunned
        if (Input.GetKeyDown(KeyCode.B) && !roaring && !thePlayer.stunned)//!globalCD)
        {
            CmdBearAbility(); //Send Command to server where server then tells the clients about the attack
        }
        */


        if (roaring)
        {
            if (BearAbilityTimer > 0f) //control the attack cooldown
            {
                BearAbilityTimer -= Time.deltaTime;
            }
            else //make it so we can attack when the timer reach 0;
            {
                //btn.interactable = true;
                roaring = false;
            }

            if (RoarDurationTimer > 0f) //control the attack duration
            {
                RoarDurationTimer -= Time.deltaTime;
            }
            else //disable the attack collider, TagAbilityCollider
            {
                CmdDisableBearAbility();

            }
        }



    }

    void StartTag()
    {
        print("StartTag");
        if (!attacking && !thePlayer.stunned)
        {
            CmdAttack();
        }
    }

    void StartRoar()
    {
        if (!roaring && !thePlayer.stunned)
        {
            CmdBearAbility();
        }
    }
    [Command] //runs on server
    protected void CmdBearAbility()
    {
        print("Bear Attack call recieved!");
        roarSound.Play();
        RpcBearAbility();
    }
    [ClientRpc] //runs on clients
    protected void RpcBearAbility() //attackanimation somewhere here (to add)
    {
        roaring = true;
        BearAbilityTimer = RoarCd;
        BearAbilityTrigger.enabled = true;
        RoarDurationTimer = RoarDuration;

    }
    [Command]
    protected void CmdDisableBearAbility()
    {
        RpcDisableBearAbility();
    }
    [ClientRpc]
    protected void RpcDisableBearAbility()
    {
        BearAbilityTrigger.enabled = false;
    }


}
