using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HookCoolDown : MonoBehaviour {
    //Variable declaration 
    public Image darkMask;
    public Image myButtonImage;
    public Text coolDownTextDisplay;
    public Button abilityButton;

    //[SerializeField] private Ability ability;
    //[SerializeField] private GameObject weaponHolder;



    public float coolDownDuration;
    private float nextReadyTime;
    private float coolDownTimeLeft;
    
    //Starting the scene 
    private void Start()
    {
        //Initialize(ability, weaponHolder);
        AbilityReady();
    }
    /*
    public void Initialize(Ability selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
        hookButton.GetComponent<Button>().targetGraphic = myButtonImage;
        //abilityScource = GetComponent<AudioSource>();
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        coolDownDuration = ability.aBaseCoolDown;
        ability.Initialize(weaponHolder);
        HookReady();
    }
    */
    void Update()
    {
        //Checking if the coolDown is complete
        bool coolDownComplete = (Time.time > nextReadyTime);
        if (coolDownComplete)
        {

            AbilityReady();
            //Runs when the button is pressed
            abilityButton.onClick.AddListener(ButtonTriggered);
            
        }
        else
        {
            CoolDown();
        }
    }
    //A function that indicates when the hook is ready to be shot again.
    private void AbilityReady()
    {
        //Removing the darkmask and the coolDowntext and activates the button again.
        coolDownTextDisplay.enabled = false;
        darkMask.enabled = false;
        abilityButton.GetComponent<Button>().enabled = true;

    }
    //A function that runs each frame the ability is on cooldown.
    private void CoolDown()
    {
        //Remove the time since last frame
        coolDownTimeLeft -= Time.deltaTime;

        float roundedCd = Mathf.Round(coolDownTimeLeft);
        //Setting the roundCd as a textFile
        coolDownTextDisplay.text = roundedCd.ToString();
        //Dividing coolDownTimeLeft with coolDownDuration to get a value between 0 and 1 which will dictate how far we've come 
        darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    }
    // Runs if the hookButton is pressed
    private void ButtonTriggered()
    {
        //Next readyTime
        nextReadyTime = coolDownDuration + Time.time;
        coolDownTimeLeft = coolDownDuration;
        darkMask.enabled = true;
        coolDownTextDisplay.enabled = true;
        abilityButton.GetComponent<Button>().enabled = false;
        //ability.TriggerAbility();
    }
}
