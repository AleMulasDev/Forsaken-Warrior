using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health
{
    [SerializeField] private EnemyHealthBar healthBar;

    private BossController _bossController;

    private EBossAttackStage _currentAttackStage = EBossAttackStage.EBAS_FirstStage;
    private EBossAttackStage _newAttackStage;

    public override void Start()
    {
        base.Start();
        _bossController = GetComponent<BossController>();
        _currentAttackStage = _bossController.GetAttackStage();
    }

    public override void TakeDamage(int damage)
    {
        if (!_bossController.EligibleForTakeDamage())
            return;

        base.TakeDamage(damage);

        if (_currentHealth > (_maxHealth * 0.66f))
        {
            _newAttackStage = EBossAttackStage.EBAS_FirstStage;
        }
        else if (_currentHealth > (_maxHealth * 0.33f))
        {
            _newAttackStage = EBossAttackStage.EBAS_SecondStage;
        }
        else
        {
            _newAttackStage = EBossAttackStage.EBAS_ThirdStage;
        }

        healthBar.UpdateHealthBar((float)_currentHealth / _maxHealth);

        if (_currentAttackStage != _newAttackStage)
        {
            _bossController.NeedsReset();
            _currentAttackStage = _newAttackStage;
        }
    }
}
