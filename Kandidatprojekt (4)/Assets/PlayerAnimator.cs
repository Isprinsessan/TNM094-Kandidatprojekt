using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    public Animator anim;
    private float inputH;
    private float inputV;

    public float moveSpeed = 30.0f;
    public VirtualJoystick joyStick;
    public Vector3 MoveVector { set; get; }
    private Rigidbody rbody;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
      rbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        MoveVector = PoolInput();
        Move();

        inputH = MoveVector.x ;
        inputV = MoveVector.z;

        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);

	}

    private Vector3 PoolInput()
    {
        Vector3 dir = Vector3.zero;

        dir.x = joyStick.Horizontal();
        dir.z = joyStick.Vertical();



        if (dir.magnitude > 1)
            dir.Normalize();

        return dir;

    }
    private void Move()
    {
        rbody.AddForce((MoveVector * moveSpeed));
        rbody.transform.eulerAngles = new Vector3(rbody.transform.eulerAngles.x, Mathf.Atan2(joyStick.Horizontal(), joyStick.Vertical()) * Mathf.Rad2Deg, rbody.transform.eulerAngles.z);
    }

}
