using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float damage;

    private PowerupManager _powerupManager;
    private void Start()
    {
        _powerupManager = GetComponentInParent<PowerupManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsOneShotEnabled(other))
        {
            other.GetComponent<Health>().Kill();
        }
        else if (other.tag.Equals(targetTag))
        {
            other.GetComponent<Health>().TakeDamage(GetDamage());
        }
    }
    private bool IsOneShotEnabled(Collider other)
    {
        return _powerupManager != null && (_powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is OneShotPowerup) && !(other.tag.Equals("Player"));
    }

    private float GetDamage()
    {
        if (_powerupManager != null && _powerupManager.GetCurrentPowerup() != null && _powerupManager.GetCurrentPowerup() is DamagePowerup)
        {
            print("palle");
            return damage * 3f;
        }
        else {
            return damage;
        }
    }
}
