using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerInput _playerInput;
    private CharacterController _controller;
    private Animator _animator;
    private MeleeWeaponTrail _weaponTrail;
    private PowerupManager _powerupManager;
    private Weapon _weapon;
    private ECharacterStates _characterState = ECharacterStates.ECS_Inoccupied;

    [Header("Player parameters")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float attackRotationSpeed;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float jumpBackwardSpeed;
    [SerializeField] private float playerJumpHeight;
    [SerializeField] private float heavyAttackMaxDuration;
    [SerializeField] private float heightOffset;
    [SerializeField] private float velocityModifier;
    [Space]
    [Header("References")]
    [SerializeField] private Transform rFoot;
    [SerializeField] private Transform lFoot;
    [SerializeField] private Collider weaponCollider;
    [Header("Sounds")]
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField] private AudioClip[] attackAudioClips;
    [SerializeField] private AudioClip[] dodgeAudioClips;
    
    private ParticleSystem[] footstepParticles;
    private Vector2 _movementInput;
    private Vector3 _movement;
    private Vector3 _cameraBasedMovement;
    private Coroutine _heavyAttackCoroutine;

    private bool _isMoving;
    private float tempDodgeSpeed;
    private float tempJumpBSpeed;
    private float _velocity;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
    private static readonly int LightAttackInput = Animator.StringToHash("lightAttackInput");
    private static readonly int LightAttackInputMovement = Animator.StringToHash("lightAttackInputMovement");
    private static readonly int HeavyAttackInput = Animator.StringToHash("heavyAttackInput");

    private const float Gravity = -6F;

    public void SLowTime()
    {
        Time.timeScale = 0.1f;
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
            SetIsLanded(true);
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.gameObject.tag.Equals("Ground"))
            SetIsLanded(false);
    }

    public static PlayerInput GetPlayerInput()
    {
        return _playerInput;
    }

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _weaponTrail = GetComponentInChildren<MeleeWeaponTrail>();
        _powerupManager = GetComponent<PowerupManager>();
        _weapon = GetComponentInChildren<Weapon>();

        _playerInput.PlayerControls.Move.started += OnMovementInput;
        _playerInput.PlayerControls.Move.performed += OnMovementInput;
        _playerInput.PlayerControls.Move.canceled += OnMovementInput;

        _playerInput.PlayerControls.Jump.started += Jump;

        _playerInput.PlayerControls.LightAttack.started += OnLightAttackStarted;
        _playerInput.PlayerControls.LightAttack.canceled += OnLightAttackEnded;

        _playerInput.PlayerControls.HeavyAttack.canceled += OnHeavyAttackEnded;
        _playerInput.PlayerControls.HeavyAttack.started += OnHeavyAttackStarted;

        _playerInput.PlayerControls.Dodge.started += Dodge;
    }

    private void Start()
    {
        footstepParticles = GameManager.instance.GetFootstepParticles();
    }
    private void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleGravity();
        HandleJump();
    }

    private void HandleRotation()
    {
        Vector3 direction;

        direction.x = _cameraBasedMovement.x;
        direction.y = 0;
        direction.z = _cameraBasedMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMoving && direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(currentRotation, newRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleMovement()
    {
        _cameraBasedMovement = ConvertToCameraSpace(_movement);

        if (CanMove() || _characterState == ECharacterStates.ECS_Jumping)
        {
            _animator.SetBool(IsMoving, _isMoving);
            _controller.Move(GetSpeed() * Time.deltaTime * _cameraBasedMovement);
        }
        else
        {
            _animator.SetBool(IsMoving, false);
        }

        if (_characterState == ECharacterStates.ECS_Dodging)
        {
            _isMoving = false;
            Dash();
        }
        else if (_characterState == ECharacterStates.ECS_BackwardJumping)
        {
            JumpBackward();
        } else
        {
            _isMoving = _movementInput != Vector2.zero;
        }

    }

    private void HandleGravity()
    {
        if (_characterState == ECharacterStates.ECS_Jumping)
        {
            _velocity += Gravity * 1.0f * Time.deltaTime;
            _movement.y = _velocity;
        }
        else
            _velocity = 0.0f;
    }

    private void HandleJump()
    {
        if (_characterState == ECharacterStates.ECS_Jumping)
        {
            float oldYVel = _movement.y;
            float newYVel = _movement.y + Mathf.Sqrt(playerJumpHeight * Gravity * -1f);
            float nextYVel = (oldYVel + newYVel) / 2;
            _movement.y = nextYVel;
        }
    }

    public void SetJumpTrigger()
    {
        ResetState();
        _characterState = ECharacterStates.ECS_Jumping;
    }
    
    #region inputFunctions

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (_characterState != ECharacterStates.ECS_Inoccupied && _characterState != ECharacterStates.ECS_LightAttack) return;

        _animator.SetTrigger("jump");
    }

    public void ChangeDamageModifier(int newDamageModifier)
    {
        _weapon.ChangeDamageModifier(newDamageModifier);
    }

    private void Dodge(InputAction.CallbackContext ctx)
    {
        if (!(_characterState == ECharacterStates.ECS_Inoccupied
            || _characterState == ECharacterStates.ECS_LightAttack
            || _characterState == ECharacterStates.ECS_HeavyAttack))
            return;

        ResetDodgeSpeed();
        DisableBox();
        DisableTrail();
        
        if (_movementInput == Vector2.zero)
        {
            _animator.SetTrigger("jumpB");
            _characterState = ECharacterStates.ECS_BackwardJumping;
            AudioManager.Instance.PlaySoundEffect(dodgeAudioClips[Random.Range(dodgeAudioClips.Length / 2 + 1, dodgeAudioClips.Length)]);
            //AudioManager.Instance.PlaySoundEffectAtPoint(dodgeAudioClips[Random.Range(dodgeAudioClips.Length / 2 + 1, dodgeAudioClips.Length)],
                //transform.position);
        }
        else
        {
            _animator.SetTrigger("dodgeF");
            _characterState = ECharacterStates.ECS_Dodging;
            AudioManager.Instance.PlaySoundEffect(dodgeAudioClips[Random.Range(0, dodgeAudioClips.Length / 2)]);
            //AudioManager.Instance.PlaySoundEffectAtPoint(dodgeAudioClips[Random.Range(dodgeAudioClips.Length / 2 + 1, dodgeAudioClips.Length)],
                //transform.position);
        }

    }

    public void Dash()
    {
        _controller.Move(tempDodgeSpeed * Time.deltaTime * transform.forward);
        DecreaseVelocity();
    }

    public void JumpBackward()
    {
        _controller.Move(tempJumpBSpeed * Time.deltaTime * (transform.forward * -1));
        DecreaseVelocity();
    }

    private void OnHeavyAttackStarted(InputAction.CallbackContext ctx)
    {
        if (_characterState == ECharacterStates.ECS_Jumping) return;

        _heavyAttackCoroutine = StartCoroutine(HeavyAttackCoroutine());
    }

    private void OnHeavyAttackEnded(InputAction.CallbackContext ctx)
    {
        if (_characterState == ECharacterStates.ECS_Jumping) return;

        if (_heavyAttackCoroutine != null)
            StopCoroutine(_heavyAttackCoroutine);

        ResetHeavyAttack();
    }

    private void OnLightAttackStarted(InputAction.CallbackContext ctx)
    {
        if (_characterState == ECharacterStates.ECS_Jumping) return;

        if (_movementInput != Vector2.zero && _characterState != ECharacterStates.ECS_LightAttack)
        {
            _animator.SetTrigger(LightAttackInputMovement);
        }
        else
        {
            _animator.SetBool(LightAttackInput, true);
            _characterState = ECharacterStates.ECS_LightAttack;
        }
    }

    private void OnLightAttackEnded(InputAction.CallbackContext ctx)
    {
        if (_characterState == ECharacterStates.ECS_Jumping) return;
        _animator.SetBool(LightAttackInput, false);
    }

    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        _movementInput = ctx.ReadValue<Vector2>();

        _movement.x = _movementInput.x;
        _movement.z = _movementInput.y;

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

    public void SetIsLanded(bool isLanded)
    {
        _animator.SetBool("isLanded", isLanded);
    }

    private float GetSpeed()
    {
        if (_powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is BoostPowerup)
        {
            return playerSpeed * 1.5f;
        }
        else
        {
            return playerSpeed;
        }
    }

    bool CanMove()
    {
        return _characterState == ECharacterStates.ECS_Inoccupied && _isMoving || _characterState == ECharacterStates.ECS_HeavyAttack;
    }

    private void EnableBox()
    {
        weaponCollider.enabled = true;
        AudioManager.Instance.PlaySoundEffect(attackAudioClips[Random.Range(0, attackAudioClips.Length)]);
        //AudioManager.Instance.PlaySoundEffectAtPoint(attackAudioClips[Random.Range(0, attackAudioClips.Length)],
            //_weapon.transform.position);

        EnableTrail();
    }

    public void PlayHeavyAttackSound()
    {
        AudioManager.Instance.PlaySoundEffect(attackAudioClips[Random.Range(0, attackAudioClips.Length)]);

        //AudioManager.Instance.PlaySoundEffectAtPoint(attackAudioClips[Random.Range(0, attackAudioClips.Length)],
            //_weapon.transform.position);
    }
    
    private void DisableBox()
    {
        weaponCollider.enabled = false;
    }
    private void EnableCombo()
    {
        _animator.SetBool(CanDoCombo, true);
    }

    private void DisableCombo()
    {
        _animator.SetBool(CanDoCombo, false);
    }

    public void ResetState()
    {
        _velocity = -1.0f;
        _characterState = ECharacterStates.ECS_Inoccupied;
        AudioManager.Instance.StopSoundEffect();
        _animator.SetBool(CanDoCombo, false);
        DisableTrail();
    }

    public void EnableTrail()
    {
        _weaponTrail.Emit = true;
    }

    public void DisableTrail()
    {
        _weaponTrail.Emit = false;
    }

    private void OnEnable()
    {
        _playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.PlayerControls.Disable();
    }

    public void SetCharacterState(ECharacterStates characterState) { _characterState = characterState; }

    private void FootR()
    {
        int _selection = UnityEngine.Random.Range(0, footstepParticles.Length - 1);
        AudioManager.Instance.PlaySoundEffect(footstepAudioClips[Random.Range(0, footstepAudioClips.Length)]);
        //AudioManager.Instance.PlaySoundEffectAtPoint(footstepAudioClips[Random.Range(0, footstepAudioClips.Length)], rFoot.position);
        Destroy(Instantiate(footstepParticles[_selection], rFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }

    private void FootL()
    {
        int _selection = UnityEngine.Random.Range(0, footstepParticles.Length - 1);
        AudioManager.Instance.PlaySoundEffect(footstepAudioClips[Random.Range(0, footstepAudioClips.Length)]);
        //AudioManager.Instance.PlaySoundEffectAtPoint(footstepAudioClips[Random.Range(0, footstepAudioClips.Length)], lFoot.position);
        Destroy(Instantiate(footstepParticles[_selection], lFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }

    public void DecreaseVelocity()
    {
        tempDodgeSpeed = Mathf.Max(0f, tempDodgeSpeed - velocityModifier);
        tempJumpBSpeed = Mathf.Max(0f, tempJumpBSpeed - velocityModifier);
    }

    public void ResetDodgeSpeed()
    {
        tempDodgeSpeed = dodgeSpeed;
        tempJumpBSpeed = jumpBackwardSpeed;
    }
}
