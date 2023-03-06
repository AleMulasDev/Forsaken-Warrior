using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PLayerControls _playerInput;
    private AnimatorManager _animatorManager;
    
    private Vector2 _movementInput;
    private float moveAmount;
    private float _verticalInput;
    private float _horizontalInput;

    private void Awake()
    {
        _animatorManager = GetComponent<AnimatorManager>();
    }

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PLayerControls();

            _playerInput.PlayerMovement.Movement.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
        }
        
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    public void HandleAllInput()
    {
        HandleMovementInput();
    }
    
    private void HandleMovementInput()
    {
        _verticalInput = _movementInput.y;
        _horizontalInput = _movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(_horizontalInput) + Mathf.Abs(_verticalInput));
        
        _animatorManager.UpdateAnimatorValues(0, moveAmount);
    }
    
    public float GetVerticalInput()
    {
        return _verticalInput;
    }

    public float GetHorizontalInput()
    {
        return _horizontalInput;
    }
}
