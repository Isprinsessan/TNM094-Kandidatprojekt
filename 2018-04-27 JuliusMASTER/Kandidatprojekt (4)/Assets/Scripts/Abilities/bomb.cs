using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour {
                  
    public GameObject explosionPrefab;
    public AudioSource explosionSound;
	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //När något rör bomben
    void OnTriggerEnter(Collider col){
        //ifall du vill ha endast spelare, checka tag, if-sats

        //slutar rendera bollen
        GetComponent<Renderer>().enabled = false;
        //sätter på bomben
        explosionPrefab.SetActive(true);

        //Playing bomb sound when the bomb explodes
        explosionSound.Play();
            
        //kallas på att förstöra bomben
        Invoke("DestroyMine", 1);

    }
    void DestroyMine(){
        //bomben försvinner
        Destroy(gameObject);

    }
}
