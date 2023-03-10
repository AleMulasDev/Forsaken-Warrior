using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class AIController : MonoBehaviour
{
    [SerializeField] float powerupSpawnChance;
    [SerializeField] int onKillScore;
    [SerializeField] protected List<Collider> colliders;
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;

    private ParticleSystem[] footstepParticles;
    private NavMeshAgent _navMeshAgent;
    protected GameObject _playerController;
    protected Animator _animator;
    protected Health _health;
    private CapsuleCollider _capsuleCollider;
    protected EEnemyState _enemyState = EEnemyState.EES_Inoccupied;

    protected Vector3 newDestination;
    protected float _attackTimer = 0.0f;

    virtual protected void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _navMeshAgent.updateRotation = false;

        footstepParticles = GameManager.instance.GetFootstepParticles();
    }

    protected void EnableNavMesh()
    {
        if (!(_enemyState == EEnemyState.EES_Inoccupied))
            return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = newDestination;
    }

    protected void DisableNavMesh()
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
    }

    protected void UpdateAnimator()
    {
        Vector3 velocity = _navMeshAgent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        _animator.SetFloat("forwardSpeed", localVelocity.z);
    }

    virtual protected void Die()
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
        _capsuleCollider.enabled = false;
    }

    

    public void HandleDeath()
    {
        SpawnPowerup();
        GameManager.instance.AddScore(onKillScore);
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

    public void ResetState()
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
        Destroy(Instantiate(footstepParticles[_selection], rFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }

    private void FootL()
    {
        int _selection = UnityEngine.Random.Range(0, footstepParticles.Length - 1);
        Destroy(Instantiate(footstepParticles[_selection], lFoot.transform.position, footstepParticles[_selection].transform.rotation).gameObject, 1f);
    }
}
