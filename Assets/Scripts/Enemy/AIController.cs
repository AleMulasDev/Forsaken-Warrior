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
    
    [SerializeField] private Collider[] colliders;
    [SerializeField] private bool shouldEnableRootMotion;

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

    protected void Die()
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
        _capsuleCollider.enabled = false;
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
        return shouldEnableRootMotion;
    }
}
