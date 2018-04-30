using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ability_struts : MonoBehaviour {

    float distance;
    public GameObject Mine;

    public Button StrutsButton;

	// Use this for initialization
	void Start () {
        distance = 2f;
        StrutsButton.onClick.AddListener(Fire);

	}
	
	// Update is called once per frame
	void Update () {
       

       
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //Instantiate(Mine, transform.position - transform.forward * distance, transform.rotation);
            /*
             var bullet = (GameObject)Instantiate(
                 Mine,
                 transform.position,
                 transform.rotation);

             // Add velocity to the bullet
             Mine.GetComponent<Rigidbody>().velocity = Mine.transform.forward * 50;

         */

           Fire();
        }
       
		
	}
    void Fire()
    {
       //gör så den släpper bomben högre upp
        Vector3 pos = new Vector3(transform.position.x,transform.position.y+7,transform.position.z);
        // Create the Bullet from the Bullet Prefab
        var Mina = (GameObject)Instantiate(Mine, pos, transform.rotation);

        // Add velocity to the bullet
        Mina.GetComponent<Rigidbody>().velocity = Mina.transform.forward * 40;
        print(Mina.GetComponent<Rigidbody>().velocity);
    }
}
