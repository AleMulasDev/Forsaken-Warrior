using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [SerializeField] GameObject _bloodParticle;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private CharacterController _characterController;

    private bool _isDead = false;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _characterController = GetComponent<CharacterController>();
    }
    public void TakeDamage(float damage)
    {
        if (_navMeshAgent != null) {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _navMeshAgent.height / 2, 0), _bloodParticle.transform.rotation), 1f);
                }
        else
        {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _characterController.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        }

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        if (_currentHealth == 0 && !_isDead)
        {
            _animator.SetTrigger("isDead");
            _isDead = true;
        }
    }

    public bool IsDead() { return _isDead; }
}
