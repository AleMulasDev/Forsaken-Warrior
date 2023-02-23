using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] private float _attackRadius;

    private NavMeshAgent _navMeshAgent;
    private GameObject _playerController;
    private Animator _animator;
    private Health _health;
    private CapsuleCollider _capsuleCollider;

    private Vector3 newDestination;
    private void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();   
        _health = GetComponent<Health>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (_health.IsDead()) {
            Die();
            return;
        }
        

        newDestination = _playerController.transform.position;

        if (Vector3.Distance(transform.position, newDestination) > _attackRadius)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.destination = newDestination;
        }
        else
        {
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.isStopped = true;
        }

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = _navMeshAgent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        _animator.SetFloat("forwardSpeed", localVelocity.z);
    }

    private void Die()
    {
        _navMeshAgent.enabled = false;
        _capsuleCollider.enabled = false;
    }
}
