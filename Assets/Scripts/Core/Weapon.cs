using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private int damage;

    private PowerupManager _powerupManager;
    private Health _health;
    private int _damageModifier = 1;
    private void Start()
    {
        _powerupManager = GetComponentInParent<PowerupManager>();
        _health = GetComponentInParent<Health>();
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(_health.IsDead()) return;

        if (IsOneShotEnabled(other))
        {
            other.GetComponent<Health>().Kill();
        }
        else if (other.tag.Equals(targetTag))
        {
            other.GetComponent<Health>().TakeDamage(GetDamage());
        }
    }

    public void ChangeDamageModifier(int newDamageModifier)
    {
        _damageModifier = newDamageModifier;
    }

    private bool IsOneShotEnabled(Collider other)
    {
        return _powerupManager != null && 
            (_powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is OneShotPowerup) 
            && !(other.tag.Equals("Player"));
    }

    private int GetDamage()
    {
        if (_powerupManager != null && _powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is DamagePowerup)
        {
            return damage * 3 * _damageModifier;
        }
        else if (_powerupManager != null)
        {
            return damage * _damageModifier;
        }
        else
            return damage;
    }

    public void Shoot(Projectile bullet, GameObject player)
    {
        GetComponentInChildren<SpawnParticle>()?.SpawnParticleAtPosition();
        Projectile bulletInstance = Instantiate(bullet, transform.position, bullet.transform.rotation);
        bulletInstance.SetProjectile(player, damage);
    }
}
