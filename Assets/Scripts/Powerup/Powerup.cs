using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private ParticleSystem powerupAura;

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
