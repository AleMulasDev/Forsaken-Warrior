using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]

public class AIController : MonoBehaviour
{
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackFirerate;
    [SerializeField] private Collider[] colliders;

    private NavMeshAgent _navMeshAgent;
    private GameObject _playerController;
    private Animator _animator;
    private Health _health;
    private CapsuleCollider _capsuleCollider;
    private EEnemyState _enemyState = EEnemyState.EES_Inoccupied;

    private Vector3 newDestination;
    private float _attackTimer = 0.0f;

    private static readonly int Attack = Animator.StringToHash("attack");

    private void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();   
        _health = GetComponent<Health>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _navMeshAgent.updateRotation = false;
    }

    private void Update()
    {
        if (_health.IsDead()) {
            Die();
            return;
        }
        
        RotateToPlayer();

        newDestination = _playerController.transform.position;

        if (Vector3.Distance(transform.position, newDestination) > _attackDistance)
        {
            EnableNavMesh();
        }
        else
        {
            AttackBehaviour();
        }

        UpdateAnimator();
        _attackTimer += Time.deltaTime;
    }

    private void EnableNavMesh()
    {
        if (!(_enemyState == EEnemyState.EES_Inoccupied))
            return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = newDestination;
    }

    private void DisableNavMesh()
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = _navMeshAgent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        _animator.SetFloat("forwardSpeed", localVelocity.z);
    }

    private void Die()
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
        _capsuleCollider.enabled = false;
    }

    private void AttackBehaviour()
    {
        DisableNavMesh();

        if (Vector3.Distance(transform.position, newDestination) <= _attackDistance && _attackTimer > _attackFirerate
            && !_playerController.GetComponent<Health>().IsDead())
        {
            _enemyState = EEnemyState.EES_Attack;
            _animator.SetTrigger(Attack);
        }
    }

    private void RotateToPlayer()
    {
        Vector3 newPlayerLocation = new Vector3(_playerController.transform.position.x, transform.position.y, _playerController.transform.position.z);
        var targetRotation = Quaternion.LookRotation(newPlayerLocation - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    public void ResetState()
    {
        _attackTimer = 0.0f;
        _enemyState = EEnemyState.EES_Inoccupied;
        EnableNavMesh();
    }

    private void EnableBox()
    {
        foreach(Collider c in colliders)
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
}
