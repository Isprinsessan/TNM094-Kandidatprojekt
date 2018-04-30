using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingHook : MonoBehaviour {
    
    public GameObject hook;
    public GameObject hookHolder;

    public AudioSource hookSound;
    public Button button;

    public float hookTravelSpeed = 15f;
    public float playerTravelSpeed = 10f;
    public float maxDistance = 20f;
    public static bool fired;
    public bool hooked;

    public GameObject hookedObj;
    private float currentDistance;

    public void Update()
    {

        //Making the hook object invisible if it is not fired
        if (fired == false)
        {
            hook.GetComponent<Renderer>().enabled = false;
        }
        
        //Firing the hook if it is not already fired and the button is pressed.
        button.onClick.AddListener(StartHook);
  
        // If the hook is fired create a line behind it to its positon.
        if (fired)
        {
            LineRenderer rope = hook.GetComponent<LineRenderer>();
            rope.SetVertexCount(2);
            // Start pos
            rope.SetPosition(0,hookHolder.transform.position);
            // End pos
            rope.SetPosition(1, hook.transform.position);
        }
        // If the hook is fired but has not yet reached an object
        if (fired == true && hooked == false)
        {
            //Translating the hook forward with the hookTravelSpeed
            hook.transform.Translate(Vector3.forward * Time.deltaTime * hookTravelSpeed);
            //The distance the hook has currently traveled
            currentDistance = Vector3.Distance(transform.position, hook.transform.position);
            // if the currentDistance is larger then the maxDistance.
            if(currentDistance >= maxDistance)
            {
                ReturnHook();
            }

        }
        //If the hook has found a target and is fired
        if (hooked == true && fired == true)
        {
            hook.transform.parent = hookedObj.transform;
            hook.GetComponent<Renderer>().enabled = true;
            //Moves the plater towards the hook positon
            transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, Time.deltaTime * playerTravelSpeed);
            // THe distance to the hook
            float distanceToHook = Vector3.Distance(transform.position, hook.transform.position);
            // Makes us not use gravity when the hook has found a traget.
            this.GetComponent<Rigidbody>().useGravity = false;
            //if the distance to the hook is smaller then 1.5 then end the process with return hook.
            if (distanceToHook < 2)
            {
                ReturnHook();
            }
        }
        else
        {
            hook.transform.parent = hookHolder.transform;
            this.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    // A function that reutrns the hook to the hookHolder position.
    void ReturnHook()
    {
        hook.transform.rotation = hookHolder.transform.rotation;
        hook.transform.position = hookHolder.transform.position;
        fired = false;
        hooked = false;

        hook.GetComponent<Renderer>().enabled = false;
        //Removing the rope behind the hook
        LineRenderer rope = hook.GetComponent<LineRenderer>();
        rope.SetVertexCount(0);
    }
    // Starting the hook
    void StartHook()
    {
        if (fired == false)
        {
            fired = true;
            hook.GetComponent<Renderer>().enabled = true;
            hookSound.Play();
        }
    }   
}
