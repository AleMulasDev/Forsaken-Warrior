using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movementControl;

    [SerializeField]
    private InputActionReference jumpControl;

    [SerializeField] 
    private InputActionReference lightAttackControl;
    
    [SerializeField]
    private float playerSpeed = 10f;

    [SerializeField]
    private float playerJumpHeight = 4f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private float rotationSpeed = 4f;
    
    [SerializeField]
    private Transform playerCamera;

    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 playerVelocity = Vector3.zero;
    private Animator animator;
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsFalling = Animator.StringToHash("isFalling");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        lightAttackControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        lightAttackControl.action.Disable();
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        if (controller.isGrounded)
        {
            animator.SetBool(IsGrounded, true);
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsFalling, false);
        }
        else
        {
            animator.SetBool(IsGrounded, false);
            
            if((animator.GetBool(IsJumping) && playerVelocity.y < 0) || playerVelocity.y < -2)
                animator.SetBool(IsFalling, true);
        }
        
        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = gravity * Time.deltaTime;

        //in questo modo capiamo in quale direzione ci stiamo muovendo
        //equivalente a Input.GetAxis() ma usando gli input actions
        Vector2 input = movementControl.action.ReadValue<Vector2>().normalized;
        
        //applichiamo il movimento e la rotazione solo se e' stato effettuato un input
        if (input.magnitude > 0)
        {
            animator.SetBool(IsMoving, true);
            RotatePlayer(input);
            
            controller.Move(playerSpeed * Time.deltaTime * moveDirection.normalized);
        }
        else
        {
            animator.SetBool(IsMoving, false);
        }
        
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        
        if (controller.isGrounded && jumpControl.action.triggered)
        {
            animator.SetBool(IsJumping, true);
            playerVelocity.y += Mathf.Sqrt(playerJumpHeight * gravity * -3.0f);
        }
        
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        
        if(lightAttackControl.action.triggered)
            animator.SetTrigger(Attack);
    }
    
    /**
     * il seguente metodo permette di ruotare il Player in base alla direzione in cui si sta muovendo, tenendo anche in
     * considerazione la rotazione della camera. In questo modo il player seguirà la direzione in cui la camera sta
     * puntando.
     */
    private void RotatePlayer(Vector2 input)
    {
        var rotationAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        moveDirection = rotation * Vector3.forward;
    }
}
