using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [SerializeField] GameObject _bloodParticle;
    [SerializeField] MultipleIconValueBarTool _multipleIconValueBarTool;
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
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        if (_currentHealth == 0 && !_isDead)
        {
            _animator.SetTrigger("isDead");
            _isDead = true;
        }

        if (_navMeshAgent != null) // Enemy
        {
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _navMeshAgent.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        }
        else // Player
        {
            _multipleIconValueBarTool.SetNowValue(_currentHealth);
            Destroy(Instantiate(_bloodParticle, transform.position + new Vector3(0, _characterController.height / 2, 0), _bloodParticle.transform.rotation), 1f);
        }
    }

    public void AddHeart(float heart)
    {
        _currentHealth = _maxHealth += (heart * 4);
        _multipleIconValueBarTool.SetMaxValue(_maxHealth);
        _multipleIconValueBarTool.SetNowValue(_maxHealth);
        _multipleIconValueBarTool.RefreshUI();
    }

    public void AddHealth(float health)
    {
        _currentHealth += health;
        _multipleIconValueBarTool.SetNowValue(_currentHealth);
        _multipleIconValueBarTool.RefreshUI();
    }

    public void Kill()
    {
        TakeDamage(_maxHealth);
    }

    public bool IsDead() { return _isDead; }
}
