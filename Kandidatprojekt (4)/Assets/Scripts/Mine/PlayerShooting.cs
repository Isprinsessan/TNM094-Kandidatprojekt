using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify the different players.
    public Rigidbody m_mine;                   // Prefab of the mine.
    public Transform m_FireTransform;           // A child of the Player where the mines are spawned.
    public Slider m_AimSlider;                  // A child of the Player that displays the current launch force.
    public float m_MinLaunchForce = 15f;        // The force given to the mine if the fire button is not held.
    public float m_MaxLaunchForce = 30f;        // The force given to the mine if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the mine can charge for before it is fired at max force.


    private string m_FireButton;                // The input axis that is used for launching mines.
    private float m_CurrentLaunchForce;         // The force that will be given to the mine when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the mine has been launched with this button press.


    private void OnEnable()
    {
        // When the Player is turned on, reset the launch force and the UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        // The fire axis is based on the player number.
        m_FireButton = "Fire" + m_PlayerNumber;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        // The slider should have a default value of the minimum launch force.
        m_AimSlider.value = m_MinLaunchForce;

        // If the max force has been exceeded and the mine hasn't yet been launched...
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // ... use the max force and launch the mine.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown(m_FireButton))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

        }
        // Otherwise, if the fire button is being held and the mine hasn't been launched yet...
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // Otherwise, if the fire button is released and the mine hasn't been launched yet...
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // ... launch the mine.
            Fire();
        }
    }


    private void Fire()
    {
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        // Create an instance of the mine and store a reference to it's rigidbody.
        Rigidbody mineInstance =
            Instantiate(m_mine, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the mine's velocity to the launch force in the fire position's forward direction.
        mineInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ;

       
        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}