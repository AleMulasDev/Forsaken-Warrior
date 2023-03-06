using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayerLocomotion : MonoBehaviour
{
    private Vector3 _moveDirection;
    private Transform _cameraTransform;
    private InputManager _inputManager;
    private Rigidbody _rb;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerRotationSpeed;
    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _rb = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        _moveDirection = _cameraTransform.forward * _inputManager.GetVerticalInput();
        _moveDirection += _cameraTransform.right * _inputManager.GetHorizontalInput();
        _moveDirection.Normalize();
        _moveDirection.y = 0;

        //applying the velocity
        _rb.velocity = _moveDirection * playerSpeed;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = _cameraTransform.forward * _inputManager.GetVerticalInput();
        targetDirection += _cameraTransform.right * _inputManager.GetHorizontalInput();
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation =
            Quaternion.Slerp(transform.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }
}