using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [SerializeField] GameObject _bloodParticle;
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private CharacterController _characterController;
    private PowerupManager _powerupManager;

    private bool _isDead = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _powerupManager = GetComponent<PowerupManager>();
        _characterController = GetComponent<CharacterController>();
    }
    public virtual void TakeDamage(float damage)
    {
        if (_characterController && _powerupManager.GetCurrentPowerup() is InvulnerabilityPowerup && _powerupManager.GetCurrentPowerup() != null)
            return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        if (_currentHealth == 0 && !_isDead)
        {
            GetComponent<AIController>()?.SpawnPowerup();

            _animator.SetTrigger("isDead");
            _isDead = true;

            foreach(AnimateCutout aC in GetComponentsInChildren<AnimateCutout>())
                aC.Dissolve();

            Destroy(gameObject, 7f);
        }

        if (_navMeshAgent != null) // Enemy
        {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _navMeshAgent.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        }
        else // Player
        {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _characterController.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        }
    }

    public void Kill()
    {
        TakeDamage(_maxHealth);
    }

    public bool IsDead() { return _isDead; }
}
