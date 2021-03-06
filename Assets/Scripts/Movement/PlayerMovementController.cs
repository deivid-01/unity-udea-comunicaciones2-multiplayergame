﻿using UnityEngine;
using Mirror;
public class PlayerMovementController : NetworkBehaviour //Belongs to someone
{

    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Ground Detection ")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float sphereRadius=0.4f; //Minisphere radius to detect collision with ground
    [SerializeField] private bool isGrounded; // If is touching the ground
    [SerializeField] private LayerMask groundMask;
    [Space]
    [SerializeField] private bool isJumping;
   
    #region Constanst Values
    [Header(" Constant values ")]
    [Range(-10,-50)] public float gravity = -9.81f;
    [Range (0.5f,3)] public float jumpHight=3f;
    [Range(4,15)] public float speed = 5f;
    #endregion

    [SerializeField] private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Vector3 currentVelocity;

    #region Controls Variables 
    private Vector2 previousInput; //???

    #endregion 
    public override void OnStartAuthority ()
    {


        enabled = true;

    //Ctx are the values readed by Controls ( KeyBoard Input)
    InputManager.Controls.Player.Move.performed += ctx => SetMovement ( ctx.ReadValue<Vector2> () ); //When performs movememt Calls SetMovement
        InputManager.Controls.Player.Move.canceled += ctx => ResetMovement ();
        InputManager.Controls.Player.Jump.performed += ctx => Jump ();
        InputManager.Controls.Player.Jump.canceled += ctx => ResetJump ();

    }




    [ClientCallback]
    private void Update ()
    {

        isGrounded = Physics.CheckSphere ( groundCheck.position , sphereRadius , groundMask );

        Vector3 direction = new Vector3(previousInput.x,0f,previousInput.y).normalized;
        if ( direction.magnitude >= 0.1f ) //It is moving 
        {
  
            
            Move (direction);
        }


        if ( isGrounded && currentVelocity.y < 0 )
        {
            currentVelocity.y = -2f;

        }

        currentVelocity.y += gravity * Time.deltaTime;

        controller.Move ( currentVelocity * Time.deltaTime );

        
        if ( isJumping && isGrounded )
        {
            currentVelocity.y = Mathf.Sqrt ( jumpHight * gravity * -2 );
        }
        




    }
    [Client]
    private void Jump () => isJumping = true;

    [Client]

    private void ResetJump () => isJumping = false;

    [Client]
    private void SetMovement ( Vector2 movement ) => previousInput = movement;

    [Client]
    private void ResetMovement () => previousInput = Vector2.zero;

    [Client]
    private void Move (Vector3 direction)
    {
        float targetAngle = Mathf.Atan2 ( direction.x , direction.z )* Mathf.Rad2Deg+ Camera.main.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle ( transform.eulerAngles.y , targetAngle , ref turnSmoothVelocity , turnSmoothTime );
        transform.rotation = Quaternion.Euler ( 0f , angle , 0f );


        Vector3  moveDir = Quaternion.Euler ( 0f , targetAngle , 0f )*Vector3.forward;

       controller.Move ( moveDir.normalized * speed * Time.deltaTime );

    }

}

