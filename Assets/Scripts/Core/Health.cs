using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [SerializeField] GameObject _bloodParticle;
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _currentHealth;

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
    public virtual void TakeDamage(int damage)
    {
        if (_characterController && _powerupManager.GetCurrentPowerup() is InvulnerabilityPowerup && _powerupManager.GetCurrentPowerup() != null)
            return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        if (_currentHealth == 0 && !_isDead)
        {
            GetComponent<AIController>()?.HandleDeath();
            GetComponent<SimpleEnemyController>()?.onEnemyBossKill?.Invoke();

            _animator.SetTrigger("isDead");
            _isDead = true;

            if (GetComponent<PlayerController>() != null)
                AudioManager.Instance.PlaySoundEffect(GetComponent<PlayerController>()?.GetAudioSource(), GetComponent<PlayerController>()?.GetDeathAudioClip());
            foreach(AnimateCutout aC in GetComponentsInChildren<AnimateCutout>())
                aC.Dissolve(2f);

            Destroy(gameObject, 7f);
        }

        if (_navMeshAgent != null) // Enemy
        {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _navMeshAgent.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        }
        else if (_characterController != null) // Player
        {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _characterController.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        } else
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, 1f, 0), _bloodParticle.transform.rotation), 1f);
    }

    public void Kill()
    {
        TakeDamage(_maxHealth);
    }

    public bool IsDead() { return _isDead; }

    public int GetHealth() { return _currentHealth; }

    public int GetMaxHealth() { return _maxHealth; }
}
