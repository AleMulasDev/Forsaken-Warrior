using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private ParticleSystem powerupAura;
    [SerializeField] private bool IsPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PowerupManager _powerupManager = other.GetComponent<PowerupManager>();

            if (IsPickup)
                ApplyPowerup(other.gameObject);
            else if (_powerupManager.GetCurrentPowerup() == null)
            {
                _powerupManager.SetCurrentPowerup(this);
                ApplyPowerup(other.gameObject);
            }
        }
    }

    protected void InstantiatePowerupAura(float powerupDuration, PowerupManager _powerupManager)
    {
        if (powerupDuration > 0)
        {
            _powerupManager.InstantiatePowerupAura(powerupAura, powerupDuration);
        }
        else
        {
            _powerupManager.InstantiatePowerupAura(powerupAura);
        }
    }

    protected void HidePowerup()
    {
        gameObject.SetActive(false);
    }

    public abstract void ApplyPowerup(GameObject player);
}
