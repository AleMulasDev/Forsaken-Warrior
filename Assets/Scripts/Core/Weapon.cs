using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float damage;

    private PowerupManager _powerupManager;
    private Health _health;
    private float _damageModifier = 1;
    private void Start()
    {
        _powerupManager = GetComponentInParent<PowerupManager>();
        _health = GetComponentInParent<Health>();
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

    public void ChangeDamageModifier(float newDamageModifier)
    {
        _damageModifier = newDamageModifier;
    }

    private bool IsOneShotEnabled(Collider other)
    {
        return _powerupManager != null && 
            (_powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is OneShotPowerup) 
            && !(other.tag.Equals("Player"));
    }

    private float GetDamage()
    {
        if (_powerupManager != null && _powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is DamagePowerup)
        {
            return damage * 3f * _damageModifier;
        }
        else {
            return damage * _damageModifier;
        }
    }

    public void Shoot(Projectile bullet, GameObject player)
    {
        GetComponentInChildren<SpawnParticle>()?.SpawnParticleAtPosition();
        Projectile bulletInstance = Instantiate(bullet, transform.position, bullet.transform.rotation);
        bulletInstance.SetProjectile(player);
    }
}
