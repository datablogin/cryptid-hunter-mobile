using UnityEngine;
using UnityEngine.InputSystem;

namespace CryptidHunter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float lookSensitivity = 0.1f;
        
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        
        private CharacterController controller;
        private Transform cameraTransform;
        private Vector3 velocity;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool isGrounded;
        private bool isRunning;
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraTransform = Camera.main.transform;
        }
        
        private void Update()
        {
            HandleMovement();
            HandleLook();
        }
        
        private void HandleMovement()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            controller.Move(move * currentSpeed * Time.deltaTime);
            
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        
        private void HandleLook()
        {
            if (lookInput.sqrMagnitude < 0.01f) return;
            
            float horizontal = lookInput.x * lookSensitivity;
            float vertical = -lookInput.y * lookSensitivity;
            
            transform.Rotate(Vector3.up * horizontal);
            
            float currentXRotation = cameraTransform.localEulerAngles.x;
            float newXRotation = currentXRotation + vertical;
            
            if (newXRotation > 180f) newXRotation -= 360f;
            newXRotation = Mathf.Clamp(newXRotation, -80f, 80f);
            
            cameraTransform.localEulerAngles = new Vector3(newXRotation, 0f, 0f);
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        
        public void OnRun(InputAction.CallbackContext context)
        {
            isRunning = context.performed;
        }
    }
}