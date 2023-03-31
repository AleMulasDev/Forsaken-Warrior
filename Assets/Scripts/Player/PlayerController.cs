using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
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
    private AudioSource source;
    private PlayerHealth _health;
    private ECharacterStates _characterState = ECharacterStates.ECS_Inoccupied;
    private string _audioPath = "PlayerAudioClips";

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
    [SerializeField] private float fallVelocity;
    [Space]
    [Header("References")]
    [SerializeField] private Transform rFoot;
    [SerializeField] private Transform lFoot;
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private GameObject indicator;

    private AudioClip[] _footstepAudioClips;
    private AudioClip[] _attackAudioClips;
    private AudioClip[] _dodgeAudioClips;
    private AudioClip[] _takeDamageAudioClips;
    private AudioClip[] _deathAudioClips;

    private ParticleSystem[] footstepParticles;
    private Vector2 _movementInput;
    private Vector3 _movement;
    private Vector3 _cameraBasedMovement;
    private Coroutine _heavyAttackCoroutine;
    private Transform _enemy;
    private ParticleSystem _respawnEffect;
    private ParticleSystem _jumpEffect;

    private bool _isMoving;
    private bool _canJump = true;
    private float tempDodgeSpeed;
    private float tempJumpBSpeed;
    private float _velocity;
    private float _tempVelocity;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
    private static readonly int LightAttackInput = Animator.StringToHash("lightAttackInput");
    private static readonly int LightAttackInputMovement = Animator.StringToHash("lightAttackInputMovement");
    private static readonly int HeavyAttackInput = Animator.StringToHash("heavyAttackInput");

    [SerializeField] private float Gravity = -6F;

    public bool GetIsMoving()
    {
        return _isMoving;
    }

    //private void OnTriggerEnter(Collider collision)
    //{
    //    if (collision.gameObject.tag.Equals("Ground"))
    //        SetIsLanded(true);
    //}

    //private void OnTriggerExit(Collider collision)
    //{
    //    if (collision.gameObject.tag.Equals("Ground"))
    //        SetIsLanded(false);
    //}

    public void SwitchJump(bool enabled)
    {
        _canJump = enabled;
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
        source = GetComponent<AudioSource>();
        _health = GetComponent<PlayerHealth>();

        _jumpEffect = Resources.Load<ParticleSystem>("PlayerJumpEffect");
        _respawnEffect = Resources.Load<ParticleSystem>("RespawnEffect");
        _footstepAudioClips = Resources.LoadAll<AudioClip>(_audioPath + "/Footstep");
        _attackAudioClips = Resources.LoadAll<AudioClip>(_audioPath + "/Attack");
        _deathAudioClips = Resources.LoadAll<AudioClip>(_audioPath + "/Death");
        _takeDamageAudioClips = Resources.LoadAll<AudioClip>(_audioPath + "/Damage");
        _dodgeAudioClips = Resources.LoadAll<AudioClip>(_audioPath + "/Dodge");

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
        _tempVelocity = 0f;
        footstepParticles = GameManager.Instance.GetFootstepParticles();
    }

    public void SetEnemy(Transform source)
    {
        _enemy = source;

        if(source != null)
            indicator.GetComponentInChildren<ParticleSystem>().Play(true);
    }

    private void Update()
    {
        //print(_controller.isGrounded);

        if(_health.IsDead())
        {
            return;
        }

        HandleRotation();
        HandleMovement();
        HandleGravity();
        HandleJump();

        if (_enemy == null) return;

        indicator.transform.position = transform.position;
        indicator.transform.LookAt(_enemy.position);
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

        if (CanMove())
        {
            _animator.SetBool(IsMoving, _isMoving);
            _controller.Move(GetSpeed() * Time.deltaTime * _cameraBasedMovement);
        }
        else if (_characterState == ECharacterStates.ECS_Jumping)
        {
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
        }
        else
        {
            _isMoving = _movementInput != Vector2.zero;
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("OOB"))
        {
            Destroy(Instantiate(_respawnEffect, transform.position, _respawnEffect.transform.rotation).gameObject, 3f);

            if (_health.GetHealth() > 8)
            {
                _health.TakeDamage(8);
                GameManager.Instance.Respawn(transform);
                gameObject.SetActive(false);
            }
            else
            {
                Die();
                _health.Kill();
            }

            ResetState();
        }
    }

    public void Die()
    {
        UIHandler.Instance.OpenDeathScreen();
    }

    private void HandleGravity()
    {
        if (ShouldApplyGravity())
        {
            if (_tempVelocity > fallVelocity)
                _tempVelocity += Gravity * 1.0f * Time.deltaTime;
            else
                _tempVelocity = fallVelocity;

            _velocity = _tempVelocity;
            _animator.SetBool("isLanded", false);
            _animator.SetBool("isFalling", true);
        }

        if (_controller.isGrounded)
        {
            _animator.SetBool("isLanded", true);
            _animator.SetBool("isFalling", false);
            _velocity = -1.0f;
            _tempVelocity = 0f;
        }
        else
        {
            Vector3 downVector = new Vector3(0, _cameraBasedMovement.y, 0);
            _controller.Move(GetSpeed() * Time.deltaTime * downVector);
        }

        _movement.y = _velocity;
    }

    private bool ShouldApplyGravity()
    {
        return _characterState == ECharacterStates.ECS_Jumping || !_controller.isGrounded && _characterState != ECharacterStates.ECS_Dodging && _characterState != ECharacterStates.ECS_BackwardJumping;
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
        Destroy(Instantiate(_jumpEffect, transform.position, _jumpEffect.transform.rotation).gameObject, 2f);
        ResetState();
        _characterState = ECharacterStates.ECS_Jumping;
    }

    #region inputFunctions

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (_characterState != ECharacterStates.ECS_Inoccupied && _characterState != ECharacterStates.ECS_LightAttack) return;

        if (!_canJump) return;

        _animator.CrossFade("Jumping", 0f, 0, 0, 0.25f);
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
            _animator.CrossFade("Jump-Backward", 0f, 0, 0, 0.25f);
            _characterState = ECharacterStates.ECS_BackwardJumping;
            AudioManager.Instance.PlaySoundEffect(source, _dodgeAudioClips[Random.Range(_dodgeAudioClips.Length / 2 + 1, _dodgeAudioClips.Length)]);
        }
        else
        {
            _animator.CrossFade("Roll-Forward", 0f, 0, 0, 0.25f);
            _characterState = ECharacterStates.ECS_Dodging;
            AudioManager.Instance.PlaySoundEffect(source, _dodgeAudioClips[Random.Range(0, _dodgeAudioClips.Length / 2)]);
        }

    }

    public void Dash()
    {
        _controller.Move(tempDodgeSpeed * Time.deltaTime * transform.forward);
        //DecreaseVelocity();
    }

    public void JumpBackward()
    {
        _controller.Move(tempJumpBSpeed * Time.deltaTime * (transform.forward * -1));
        //DecreaseVelocity();
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

        if(_characterState == ECharacterStates.ECS_Jumping)
        {
            _movement.x *= 0.9f; 
            _movement.z *= 0.9f;
        }

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
        _characterState = ECharacterStates.ECS_Inoccupied;
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

    public AudioSource GetAudioSource()
    {
        return source;
    }

    public AudioClip GetDamageAudioClip()
    {
        return _takeDamageAudioClips[Random.Range(0, _takeDamageAudioClips.Length)];
    }

    public AudioClip GetDeathAudioClip()
    {
        return _deathAudioClips[Random.Range(0, _deathAudioClips.Length)];
    }

    bool CanMove()
    {
        return _characterState == ECharacterStates.ECS_Inoccupied && _isMoving || _characterState == ECharacterStates.ECS_HeavyAttack;
    }

    private void EnableBox()
    {
        weaponCollider.enabled = true;
        //AudioManager.Instance.PlaySoundEffect(source, _attackAudioClips[Random.Range(0, _attackAudioClips.Length)]);

        EnableTrail();
    }

    public void PlayHeavyAttackSound()
    {
        AudioManager.Instance.PlaySoundEffect(source, _weapon.GetWeaponAudioClip());
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
        _characterState = ECharacterStates.ECS_Inoccupied;
        _animator.SetBool(CanDoCombo, false);
        _weapon.ChangeDamageModifier(1);
        DisableTrail();
        tempDodgeSpeed = 0f;
        tempJumpBSpeed = 0f;
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
        AudioManager.Instance.PlaySoundEffect(source, _footstepAudioClips[Random.Range(0, _footstepAudioClips.Length)]);
        Destroy(Instantiate(footstepParticles[_selection], rFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }

    private void FootL()
    {
        int _selection = UnityEngine.Random.Range(0, footstepParticles.Length - 1);
        AudioManager.Instance.PlaySoundEffect(source, _footstepAudioClips[Random.Range(0, _footstepAudioClips.Length)]);
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

    private void PlayAttackAudioClip()
    {
        AudioManager.Instance.PlaySoundEffect(source, _weapon.GetWeaponAudioClip());
    }
}
