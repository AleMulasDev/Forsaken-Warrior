using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health
{
    [SerializeField] private EnemyHealthBar healthBar;

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        healthBar.UpdateHealthBar((float)_currentHealth / _maxHealth);
    }
}
