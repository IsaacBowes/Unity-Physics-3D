using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPScontrol : MonoBehaviour
{

    CharacterController player;

    public float moveSpeed = 3f;
    public float jumpForce = 4f;
    public float sensitivity = 3f;
    float range = 100f;
    public Camera FPSCamera;
    public GameObject head;

    private bool jumped;

    float moveForwardBackward;
    float moveLeftRight;
    float verticalVel;
    float rotateX;
    float rotateY;

	// Use this for initialization
	void Start ()
    {
        player = GetComponent<CharacterController>();	
	}
	
	// Update is called once per frame
	void Update ()
    {

        movement();

        if(Input.GetButtonDown("Jump"))
        {
            jumped = true;
        }

        ApplyGravity();

        if (Input.GetButtonDown("Fire2"))
        {
            Shoot();
        }	
	}

    private void ApplyGravity()
    {
        if (player.isGrounded == true)
        {
            if (jumped == false)
            {
                verticalVel = Physics.gravity.y;
            }
            else
            {
                verticalVel = jumpForce;
            }
        }
        else
        {
            verticalVel += Physics.gravity.y * Time.deltaTime;
            verticalVel = Mathf.Clamp(verticalVel, -50f, jumpForce);
            jumped = false;
        }
    }


    void movement()
    {
        moveForwardBackward = Input.GetAxis("Vertical") * moveSpeed;
        moveLeftRight = Input.GetAxis("Horizontal") * moveSpeed;

        rotateX += Input.GetAxis("Mouse X") * sensitivity;
        rotateY += Input.GetAxis("Mouse Y") * sensitivity;

        Vector3 move = new Vector3(moveLeftRight, verticalVel, moveForwardBackward);
        transform.eulerAngles = new Vector3(0,rotateX,0);
        head.transform.localEulerAngles = new Vector3(-rotateY, 0, 0);

        move = transform.TransformDirection(move);
        player.Move(move * Time.deltaTime);
    }

    void Shoot ()
    {    
        RaycastHit hit;
        if (Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.Active();
            }
           
        }
    }

}
