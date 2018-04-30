using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ability_rav : MonoBehaviour {
    float speed;
    bool leap;
    // Use this for initialization
    Vector3 currentPosition;
    Vector3 targetPosition;
    int counter;
    public Button dashButton;
    public AudioSource dashSound;


	void Start () {
        speed = 10f;
        leap = false;
        counter = 0;
        currentPosition = Vector3.zero;

        dashButton.onClick.AddListener(StartDash);
    }
	
	// Update is called once per frame
	void Update () {

        if(leap){
            
            transform.Translate(0f, 0f, 1f);
            counter++;
        }
        if (counter == 5)
        {
            //anim.Play("Dash", 1, 0f);
            leap = false;
        }
         
		
	}

    void StartDash()
    {
        
        if (leap == false)
        {
            leap = true;
            counter = 0;
            dashSound.Play();

        }

    }
}
