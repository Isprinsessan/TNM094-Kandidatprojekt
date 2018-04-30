using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class selectCharacter : NetworkBehaviour
{

    public GameObject charSelect;

    //public GameObject instance = GameObject.FindGameObjectWithTag("Character Selection");
    public Toggle toggle;
    //[SyncVar(hook = "UpdateCharacter()")]
    public NetworkLobbyPlayer playerInfo;


    [Space]
    [Header("Image/Button References")]
    public Sprite bear;
    public Button bearButton;
    [Space]
    public Sprite fox;
    public Button foxBotton;
    [Space]
    public Sprite spider;
    public Button spiderButton;
    [Space]
    public Sprite ostrich;
    public Button ostrichButton;

    [SyncVar]
    public int index = 1;

    public string thisChar = "Fox";
    private string[] characters = { "Bear", "Fox", "Spider", "Ostrich" };


    // Use this for initialization
    void Start()
    {
        toggle.isOn = false;
        charSelect.SetActive(false);
        if (playerInfo.isLocalPlayer)
        {
            toggle.isOn = false;
            charSelect.SetActive(false);
            toggle.onValueChanged.AddListener((bool value) => CharMenuToggle(toggle.isOn));
        }


        Button bearBtn = bearButton.GetComponent<Button>();
        Button foxBtn = foxBotton.GetComponent<Button>();
        Button spiderBtn = spiderButton.GetComponent<Button>();
        Button ostrichBtn = ostrichButton.GetComponent<Button>();

        /*bearBtn.onClick.AddListener(BearClick);
        foxBtn.onClick.AddListener(FoxClick);
        spiderBtn.onClick.AddListener(SpiderClick);
        ostrichBtn.onClick.AddListener(OstrichClick);
        */
    }

    public void CharMenuToggle(bool isOn)
    {

        //Debug.Log(isOn);
        if (charSelect.activeInHierarchy && !isOn)
        {
            toggle.isOn = false;
            charSelect.SetActive(false);
        }
        else if (!charSelect.activeInHierarchy && isOn)
        {
            toggle.isOn = true;
            charSelect.SetActive(true);
        }
    }

    void BearClick()
    {
        index = 0;


        UpdateCharacter(index, bear);
    }
    public void FoxClick()
    {
        index = 1;
        UpdateCharacter(index, fox);
    }
    public void SpiderClick()
    {
        index = 2;
        UpdateCharacter(index, spider);
    }
    public void OstrichClick()
    {
        index = 3;
        UpdateCharacter(index, ostrich);
    }

    public void UpdateCharacter(int index, Sprite avatar)
    {
        toggle.isOn = false;
        thisChar = characters[index];
        Debug.Log(thisChar + " is selected");
        toggle.image.sprite = avatar;
    }

}