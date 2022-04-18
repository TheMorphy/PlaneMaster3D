using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField]
    float moveSpeed = 3f;
    [SerializeField]
    float moveSmooth = 0.13f, turnSmooth = 0.1f;
    [SerializeField]
    AnimationCurve joistickReplyCurve;

    [Header("Animation")]
    [SerializeField]
    float animationPlaybackspeed = 1f;

    [Header("Serializables")]
    [SerializeField]
    bool touchControls = true;
    [SerializeField]
    FloatingJoystick joistick;
    [SerializeField]
    Animator visual3D;
    [SerializeField]
    Transform cam;
    Transform stableForward;
    Vector3 velocity, lookRotation;
    CharacterController controller;
    float inputX, inputY;
    float currentSpeed;
    [HideInInspector]
    public bool isMoving;
    bool isGrounded;
    float velY;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stableForward = cam.GetChild(0);
    }
    void Update()
    {
        //touch controls
        inputY = joistick.Vertical;
        inputX = joistick.Horizontal;


        if (!touchControls)
        {
            //pc controls for debugging
            inputY = Input.GetAxisRaw("Vertical");
            inputX = Input.GetAxisRaw("Horizontal");
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, moveSmooth);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * joistickReplyCurve.Evaluate(joistick.Direction.magnitude) , moveSmooth);
        }
            if (Mathf.Max(Mathf.Abs(inputY), Mathf.Abs(inputX)) > 0)
            {
                
                Vector3 tempLookRotation = stableForward.forward * inputY + stableForward.right * inputX;
                tempLookRotation *= 180;
                lookRotation = Vector3.Lerp(lookRotation, tempLookRotation, turnSmooth);
                isMoving = true;

            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, moveSmooth);
                if (isMoving)
                {

                    isMoving = false;
                }
            }




            velocity = stableForward.forward * inputY * Time.deltaTime * currentSpeed + stableForward.right * inputX * Time.deltaTime * currentSpeed +stableForward.up * velY;

            transform.rotation = Quaternion.LookRotation(lookRotation);
            controller.Move(velocity);
            visual3D.SetFloat("Speed", currentSpeed);
        
        

    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.3f, 3);
        if(!isGrounded)
        {
            velY += -9.81f * Time.fixedDeltaTime; 
        }
        else
        {
            velY = -2f;
        }
    }
}
