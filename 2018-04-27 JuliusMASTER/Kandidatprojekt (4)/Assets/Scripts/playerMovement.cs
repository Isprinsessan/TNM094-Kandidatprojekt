
using UnityEngine;
using UnityEngine.Networking;

public class playermovment : NetworkBehaviour {

    [SerializeField] float movementspeed = 5.0f;
    [SerializeField] float turnspeed = 45.0f;
    [SerializeField] float cameradistance = 16f;
    [SerializeField] float cameraright = 16f;

    Rigidbody localRigidbody;
    Transform maincamera;
    Vector3 cameraoffset;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        localRigidbody = GetComponent<Rigidbody>();
        cameraoffset = new Vector3(0f, cameraright, -cameradistance);

        maincamera = Camera.main.transform;
        MoveCamera();

    }
	
	
	void FixedUpdate () {
        float turnAmount = Input.GetAxis("Horizontal");
        float moveAmount = Input.GetAxis("Vertical");

        Vector3 deltaTranslation = transform.position + transform.forward * movementspeed * moveAmount * Time.deltaTime;
        localRigidbody.MovePosition(deltaTranslation);

        Quaternion deltaRotation = Quaternion.Euler(turnspeed * new Vector3(0, turnAmount, 0) * Time.deltaTime);
        localRigidbody.MoveRotation(localRigidbody.rotation * deltaRotation);

    }

    void MoveCamera()
    {
        maincamera.position = transform.position;
        maincamera.rotation = transform.rotation;
        maincamera.Translate(cameraoffset);
        maincamera.LookAt(transform);
    }

}
