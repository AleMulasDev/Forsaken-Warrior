using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movementControl;

    [SerializeField]
    private InputActionReference jumpControl;

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
    
    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = gravity * Time.deltaTime;
        }
        
        //in questo modo capiamo in quale direzione ci stiamo muovendo
        //equivalente a Input.GetAxis() ma usando gli input actions
        Vector2 input = movementControl.action.ReadValue<Vector2>().normalized;
        
        //applichiamo il movimento e la rotazione solo se e' stato effettuato un input
        if (input.magnitude > 0)
        {
            RotatePlayer(input);
            
            controller.Move(playerSpeed * Time.deltaTime * moveDirection.normalized);
        }
        
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        
        if (controller.isGrounded && jumpControl.action.triggered)
        {
            playerVelocity.y += Mathf.Sqrt(playerJumpHeight * gravity * -3.0f);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    
    /**
     * il seguente metodo permette di ruotare il player in base alla direzione in cui si sta muovendo, calcolando
     * l'angolo tra i due assi e rendendo la rotazione piu' fluida facendo l'interpolazione lineare tra la rotazione
     * corrente e la nuova rotazione
     */
    private void RotatePlayer(Vector2 input)
    {
        var rotationAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        moveDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
    }
}
