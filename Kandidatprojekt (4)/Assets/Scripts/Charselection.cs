using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Charselection : NetworkBehaviour {



    private GameObject[] characterlist;
    [SyncVar]
    public int charindex;
    public Canvas changecharcanvas;


    private void Start()
    {
        //fill with modells

        //Debug.Log("length is");
        //Debug.Log(characterlist[0].name);

            
            characterlist = new GameObject[transform.childCount];
        
            for (int i = 0; i < transform.childCount; i++)
            {
                characterlist[i] = transform.GetChild(i).gameObject;
                //Debug.Log(characterlist[i].gameObject.name);
            }
           // Debug.Log(characterlist[0].name);


            if (characterlist[charindex])
            {
                characterlist[charindex].SetActive(true);
            }


           // changecharcanvas = changecharcanvas.GetComponent<Canvas>();
//            changecharcanvas.gameObject.SetActive(true);


           // Debug.Log(charindex);
   
    }


    public void nextchar()
    {
        charindex--;
        if (charindex < 0)
            charindex = 3;//MAXSIZE  =  4 st

        PlayerPrefs.SetInt("charindex", charindex);
        Debug.Log(charindex);
    }

    public void ToggleLeft()
    {
        //toggle off current model
        // Debug.Log(charlist.Length);
        
        
        characterlist[charindex].SetActive(false);
        charindex--;
        if (charindex < 0)
            charindex = characterlist.Length-2 ;

        characterlist[charindex].SetActive(true);
    }
}
