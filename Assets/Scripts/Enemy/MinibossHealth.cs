using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossHealth : Health
{
    [SerializeField] private MinibossHealthBar healthBar;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.UpdateHealthBar(_currentHealth / _maxHealth);
    }
}
