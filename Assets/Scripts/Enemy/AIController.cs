using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    [SerializeField] float powerupSpawnChance;
    [SerializeField] int onKillScore;
    [SerializeField] protected List<Collider> colliders;
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField] private AudioClip[] deathAudioClips;
    [SerializeField] private AudioClip[] takeDamageAudioClips;
    [SerializeField] protected AudioClip[] attackAudioClips;

    private ParticleSystem[] footstepParticles;
    protected NavMeshAgent _navMeshAgent;
    protected AudioSource _audioSource;
    protected GameObject _playerController;
    protected Animator _animator;
    protected Health _health;
    private CapsuleCollider _capsuleCollider;
    protected EEnemyState _enemyState = EEnemyState.EES_Inoccupied;

    protected Vector3 newDestination;
    protected float _attackTimer = 0.0f;

    [HideInInspector] public UnityEvent onEnemyBossKill = new UnityEvent();

    virtual protected void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _audioSource = GetComponent<AudioSource>();

        if (_navMeshAgent != null)
            _navMeshAgent.updateRotation = false;

        footstepParticles = GameManager.instance.GetFootstepParticles();
    }

    protected void EnableNavMesh()
    {
        if (!(_enemyState == EEnemyState.EES_Inoccupied))
            return;

        if (_navMeshAgent == null) return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = newDestination;

    }

    protected void DisableNavMesh()
    {
        if (_navMeshAgent == null) return;

        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
    }

    protected void UpdateAnimator()
    {
        if (_navMeshAgent == null) return;

        Vector3 velocity = _navMeshAgent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        _animator.SetFloat("forwardSpeed", localVelocity.z);
    }

    virtual protected void Die()
    {
        _capsuleCollider.enabled = false;

        if (_navMeshAgent == null) return;

        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
    }

    public void HandleDeath()
    {
        SpawnPowerup();
        GameManager.instance.AddScore(onKillScore);
        AudioManager.Instance.PlaySoundEffect(_audioSource, deathAudioClips[Random.Range(0, deathAudioClips.Length)]);
    }

    public AudioSource GetAudioSource()
    {
        return _audioSource;
    }

    public AudioClip GetDamageAudioClip()
    {
        return takeDamageAudioClips[Random.Range(0, takeDamageAudioClips.Length)];
    }

    private void SpawnPowerup()
    {
        if (Random.Range(0.0f, 1.0f) > powerupSpawnChance) return;

        int randomVal = Random.Range(0, 101);

        Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0);

        if (randomVal < 25)
        {
            Instantiate(Resources.Load<Powerup>("Powerups/BoostPowerup"), spawnPosition, Quaternion.identity);
        }
        else if (randomVal < 45)
        {
            Instantiate(Resources.Load<Powerup>("Powerups/HealthPowerup"), spawnPosition, Quaternion.identity);
        }
        else if (randomVal < 60)
        {
            Instantiate(Resources.Load<Powerup>("Powerups/DamagePowerup"), spawnPosition, Quaternion.identity);
        }
        else if (randomVal < 75)
        {
            Instantiate(Resources.Load<Powerup>("Powerups/HeartPowerup"), spawnPosition, Quaternion.identity);
        }
        else if (randomVal < 85)
        {
            Instantiate(Resources.Load<Powerup>("Powerups/InvulnerabilityPowerup"), spawnPosition, Quaternion.identity);
        }
        else if (randomVal < 95)
        {
            Instantiate(Resources.Load<Powerup>("Powerups/OneShotPowerup"), spawnPosition, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load<Powerup>("Powerups/RandomPowerup"), spawnPosition, Quaternion.identity);
        }
    }

    protected void RotateToPlayer()
    {
        Vector3 newPlayerLocation = new Vector3(_playerController.transform.position.x, transform.position.y, _playerController.transform.position.z);
        var targetRotation = Quaternion.LookRotation(newPlayerLocation - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    public virtual void ResetState()
    {
        _attackTimer = 0.0f;
        _enemyState = EEnemyState.EES_Inoccupied;
        _animator.applyRootMotion = false;
        EnableNavMesh();
    }

    private void EnableBox()
    {
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
    }

    private void DisableBox()
    {
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
    }

    public void SetEnemyState(EEnemyState newEnemyState)
    {
        _enemyState = newEnemyState;
    }

    public bool GetShouldEnableRootMotion()
    {
        return false;
    }

    private void FootR()
    {
        int _selection = UnityEngine.Random.Range(0, footstepParticles.Length - 1);
        AudioManager.Instance.PlaySoundEffect(_audioSource, footstepAudioClips[Random.Range(0, footstepAudioClips.Length)]);
        Destroy(Instantiate(footstepParticles[_selection], rFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }

    private void FootL()
    {
        int _selection = UnityEngine.Random.Range(0, footstepParticles.Length - 1);
        AudioManager.Instance.PlaySoundEffect(_audioSource, footstepAudioClips[Random.Range(0, footstepAudioClips.Length)]);
        Destroy(Instantiate(footstepParticles[_selection], lFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }
}
