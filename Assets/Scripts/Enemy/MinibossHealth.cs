using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossHealth : Health
{
    [SerializeField] private MinibossHealthBar healthBar;
    [SerializeField] GameObject stunStars;

    bool alreadyStunned = false;

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if(_currentHealth < (_maxHealth / 2) && !alreadyStunned)
        {
            Destroy(Instantiate(stunStars, transform.position + new Vector3(0, 2f, 0), stunStars.transform.rotation), 5f);
            alreadyStunned = true;
            GetComponent<Animator>().SetTrigger("stun");
            GetComponent<AIController>().SetEnemyState(EEnemyState.EES_Stunned);
        }

        healthBar.UpdateHealthBar((float)_currentHealth / _maxHealth);
    }
}
