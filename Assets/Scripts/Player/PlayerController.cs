using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _controller;
    private Animator _animator;
    private ECharacterStates _characterState = ECharacterStates.ECS_Inoccupied;
    
    [Header("Player parameters")] 
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float attackRotationSpeed;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerJumpHeight;
    [SerializeField] private float heavyAttackMaxDuration;
    
    private float _heavyAttackCurrentDuration;
    
    private Vector2 _movementInput;
    private Vector3 _movement;
    private Vector3 _cameraBasedMovement;
    private Coroutine _heavyAttackCoroutine;
    
    private bool _isMoving;
    private bool _isJumping;
    
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsFalling = Animator.StringToHash("isFalling");
    private static readonly int IsLanded = Animator.StringToHash("isLanded");
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
    private static readonly int LightAttackInput = Animator.StringToHash("lightAttackInput");
    private static readonly int HeavyAttackInput = Animator.StringToHash("heavyAttackInput");

    private const float Gravity = -9.81F;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        
        _playerInput.PlayerControls.Move.started += OnMovementInput;
        _playerInput.PlayerControls.Move.performed += OnMovementInput;
        _playerInput.PlayerControls.Move.canceled += OnMovementInput;
        _playerInput.PlayerControls.LightAttack.started += OnLightAttackStarted;
        _playerInput.PlayerControls.LightAttack.canceled += OnLightAttackEnded;

        _playerInput.PlayerControls.HeavyAttack.canceled += OnHeavyAttackEnded;
        _playerInput.PlayerControls.HeavyAttack.started += OnHeavyAttackStarted;

    }

    #region inputFunctions

    private void OnHeavyAttackStarted(InputAction.CallbackContext ctx)
    {
        _heavyAttackCoroutine = StartCoroutine(HeavyAttackCoroutine());
    }

    private void OnHeavyAttackEnded(InputAction.CallbackContext ctx)
    {
        if(_heavyAttackCoroutine != null)
            StopCoroutine(_heavyAttackCoroutine);

        ResetHeavyAttack();
    }
    
    private void OnLightAttackStarted(InputAction.CallbackContext ctx)
    {
        _animator.SetBool(LightAttackInput, true);
        _characterState = ECharacterStates.ECS_LightAttack;
    }

    private void OnLightAttackEnded(InputAction.CallbackContext ctx)
    {
        _animator.SetBool(LightAttackInput, false);
    }
    
    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        _movementInput = ctx.ReadValue<Vector2>();

        _movement.x = _movementInput.x;
        _movement.z = _movementInput.y;
        _isMoving = _movementInput != Vector2.zero;

        if (_characterState == ECharacterStates.ECS_LightAttack)
        {
            transform.Rotate(0, _movementInput.x * attackRotationSpeed, 0);
        }        
    }
    #endregion
    private IEnumerator HeavyAttackCoroutine()
    {
        float heavyAttackTimer = 0.0f;

        _animator.SetBool(HeavyAttackInput, true);
        _characterState = ECharacterStates.ECS_HeavyAttack;

        while (heavyAttackTimer < 3.0f)
        {
            heavyAttackTimer += Time.deltaTime;
            yield return null;
        }

        ResetHeavyAttack();
    }

    private void ResetHeavyAttack()
    {
        _animator.SetBool(HeavyAttackInput, false);
        _characterState = ECharacterStates.ECS_Inoccupied;
    }

    private void HandleRotation()
    {
        Vector3 direction;

        direction.x = _cameraBasedMovement.x;
        direction.y = 0;
        direction.z = _cameraBasedMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMoving)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(currentRotation, newRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        bool isFalling = _movement.y <= -1.0f;
        float fallMultiplier = 2.0f;

        if (_controller.isGrounded)
        {
            _animator.SetBool(IsFalling, false);
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsLanded, true);
            _movement.y = Gravity * Time.deltaTime;
        }
        else if (isFalling)
        {
            print(_movement.y);
            _animator.SetBool(IsFalling, true);
            _animator.SetBool(IsLanded, false);
            
            float oldYVel = _movement.y;
            float newYVel = _movement.y + (Gravity * fallMultiplier * Time.deltaTime);
            float nextYVel = Mathf.Max((oldYVel + newYVel) / 2, -20.0f);
            _movement.y = nextYVel;
        }
        else
        {
            float oldYVel = _movement.y;
            float newYVel = _movement.y + (Gravity * Time.deltaTime);
            float nextYVel = (oldYVel + newYVel) / 2;
            _movement.y = nextYVel;
        }
    }

    private void HandleJump()
    {
        if (_controller.isGrounded && _playerInput.PlayerControls.Jump.triggered && !_isJumping)
        {
            _animator.SetBool(IsJumping, true);
            _animator.SetBool(IsLanded, false);
            
            _isJumping = true;
            float oldYVel = _movement.y;
            float newYVel = _movement.y + Mathf.Sqrt(playerJumpHeight * Gravity * -3.0f);
            float nextYVel = (oldYVel + newYVel) / 2;
            _movement.y = nextYVel;
        }else if (_controller.isGrounded && _isJumping)
            _isJumping = false;
        
    }

    private Vector3 ConvertToCameraSpace(Vector3 playerMovement)
    {
        Transform camera = Camera.main.transform;

        float currentY = playerMovement.y;
        
        Vector3 forward = camera.forward.normalized;
        Vector3 right = camera.right.normalized;

        forward.y = 0;
        right.y = 0;

        Vector3 rotatedX = playerMovement.x * right;
        Vector3 rotatedZ = playerMovement.z * forward;

        Vector3 res = rotatedZ + rotatedX;
        res.y = currentY;
        
        return res;
    }
    
    private void Update()
    {
        HandleRotation();

        _cameraBasedMovement = ConvertToCameraSpace(_movement);

        if (CanMove() || _isJumping)
        {
            _animator.SetBool(IsMoving, _isMoving);
            _controller.Move(playerSpeed * Time.deltaTime * _cameraBasedMovement);
        } else
        {
            _animator.SetBool(IsMoving, false);
        }

        HandleGravity();
        HandleJump();
    }

    bool CanMove()
    {
        return _characterState == ECharacterStates.ECS_Inoccupied && _isMoving
            || _characterState == ECharacterStates.ECS_HeavyAttack;
    }

    private void CheckAnimationState()
    {
        if (_characterState == ECharacterStates.ECS_LightAttack)
        {
            _playerInput.PlayerControls.Move.Disable();
            _playerInput.PlayerControls.Jump.Disable();
        }else if (_characterState == ECharacterStates.ECS_HeavyAttack)
        {
            if(_playerInput.PlayerControls.Jump.enabled)
                _playerInput.PlayerControls.Jump.Disable();

            _heavyAttackCurrentDuration += Time.deltaTime;
            
            if(_heavyAttackCurrentDuration > heavyAttackMaxDuration)
                _playerInput.PlayerControls.HeavyAttack.Disable();
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy-Attack-End"))
        {
            _playerInput.PlayerControls.Move.Disable();
            
            if(_playerInput.PlayerControls.Jump.enabled)
                _playerInput.PlayerControls.Jump.Disable();
        }
        else
        {
            _playerInput.PlayerControls.Move.Enable();
            _playerInput.PlayerControls.Jump.Enable();
            _playerInput.PlayerControls.HeavyAttack.Enable();
            _heavyAttackCurrentDuration = 0f;
        }   
    }
    
    private void EnableCombo()
    {
        _animator.SetBool(CanDoCombo, true);
    }
    
    private void DisableCombo()
    {
        _animator.SetBool(CanDoCombo, false);
    }

    private void ResetState()
    {
        _characterState = ECharacterStates.ECS_Inoccupied;
    }
    
    private void OnEnable()
    {
        _playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.PlayerControls.Disable();
    }
}
