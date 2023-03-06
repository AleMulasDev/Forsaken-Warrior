using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager _inputManager;
    private PLayerLocomotion _pLayerLocomotion;

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _pLayerLocomotion = GetComponent<PLayerLocomotion>();
    }

    private void Update()
    {
        _inputManager.HandleAllInput();
    }

    private void FixedUpdate()
    {
        _pLayerLocomotion.HandleAllMovement();
    }
}
