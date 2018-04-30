using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/*
* This script should be attached to every character since it is the basic attack.
* Or rather, this script should be the base script of abilities and then other abilities inherit this script
* and you attach that script to the character!
* 
* This script enables and disables the collider for the 'TagAbilityCollider' GameObject through
* command calls to the server as well as it decides cooldown of the ablity and it's attack duration 
* 
*/

public class PlayerAttack : NetworkBehaviour
{

    public Button tagButton;
    public AudioSource tagSound;
    public bool attacking = false;

    public float attackTimer = 0f;
    public float attackDurationTimer = 0f;

    protected float attackCd = 5f; //Cooldown for the attack
    protected float duration = 1f; //The attack's duration, amount of time that the TagAbilityCollider is enabled

    public Collider attackTrigger; //Here we connect the TagAbilityCollider, attach the gameObject to the script

    protected PlayerController thePlayer; //This is used to be able to see if the player is stunned.

    //public Animator anim; // for animation

    // public Button btn;

    void Start()
    {
        thePlayer = gameObject.GetComponent<PlayerController>();
        attackTrigger.enabled = false; //we start by having the attack disabled until we attack

        //You may only attack if you are not already attacking and you are not stunned
        tagButton.onClick.AddListener(StartTag);
    }

    //Unsure if it is necessary to have the tag [ClientCallback] here.
    void Update()
    {
        if (!isLocalPlayer)
            return;

        CheckAttack();
    }

    void StartTag()
    {
        print("StartTag");
        if (!attacking && !thePlayer.stunned)
        {
            CmdAttack();
        }
    }

    [Command] //runs on server
    protected void CmdAttack()
    {
        print("Attack call recieved!");
        tagSound.Play();
        RpcAttack();
    }
    [ClientRpc] //runs on clients
    public void RpcAttack() //attackanimation somewhere here (to add)
    {
        
        attacking = true;
        attackTimer = attackCd;
        attackTrigger.enabled = true;
        attackDurationTimer = duration;
       
        //anim.SetBool("TagAttacking", attackTrigger.enabled);

    }
    [Command]
    protected void CmdDisableAttack()
    {
        RpcDisableAttack();
    }
    [ClientRpc]
    protected void RpcDisableAttack()
    {
        attackTrigger.enabled = false;
    }

    //the purpose of having this void is to reduce duplicated code, this is a base class and is inherited by Bear
    protected void CheckAttack()
    {


        //You may only attack if you are not already attacking and you are not stunned
        /*if (Input.GetKeyDown(KeyCode.A) && !attacking && !thePlayer.stunned)
        {
            CmdAttack(); //Send Command to server where server then tells the clients about the attack
        }*/
        //tagButton.onClick.AddListener(StartTag);

        if (attacking)
        {
            
            if (attackTimer > 0f) //control the attack cooldown
            {
                attackTimer -= Time.deltaTime;
            }
            else //make it so we can attack when the timer reach 0;
            {
                //btn.interactable = true;
                attacking = false;
            }

            if (attackDurationTimer > 0f) //control the attack duration
            {
                attackDurationTimer -= Time.deltaTime;
            }
            else //disable the attack collider, TagAbilityCollider
            {
                CmdDisableAttack();
            }
        }
    }


}
