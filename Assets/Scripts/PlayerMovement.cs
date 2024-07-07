using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    

    public static PlayerMovement Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 movement = gameInput.GetMovementVectorNormalized();

        float x = movement.x;
        float z = movement.y;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (gameInput.PlayerJumpedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        velocity.y += gravityValue * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }




}
