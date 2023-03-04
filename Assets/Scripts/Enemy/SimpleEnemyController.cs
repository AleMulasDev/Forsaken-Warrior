using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleEnemyController : AIController
{
    [SerializeField] protected float _attackDistance;
    [SerializeField] private float _attackFirerate;
    [SerializeField] private Projectile bullet;

    private Weapon _weapon;

    private static readonly int Attack = Animator.StringToHash("attack");

    protected override void Start()
    {
        _weapon = GetComponentInChildren<Weapon>();
        base.Start();
    }

    void Update()
    {
        if(_enemyState == EEnemyState.EES_Spawning) { return; }
        if (_health.IsDead())
        {
            Die();
            return;
        }

        if (_enemyState == EEnemyState.EES_Inoccupied)
            RotateToPlayer();

        if(!_playerController.GetComponent<Health>().IsDead())
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


    protected void AttackBehaviour()
    {
        DisableNavMesh();

        if (Vector3.Distance(transform.position, newDestination) <= _attackDistance && _attackTimer > _attackFirerate
            && !_playerController.GetComponent<Health>().IsDead())
        {
            _enemyState = EEnemyState.EES_Attack;
            _animator.SetTrigger(Attack);
        }
    }
    protected void Shoot()
    {
        _weapon.Shoot(bullet, _playerController);
    }
}
