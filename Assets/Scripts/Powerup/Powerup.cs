using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private ParticleSystem powerupAura;
    [SerializeField] private bool IsPickup;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            PowerupManager _powerupManager = other.GetComponent<PowerupManager>();

            if (_powerupManager.GetCurrentPowerup() == null)
            {
                if(!IsPickup)
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
        } else
        {
            _powerupManager.InstantiatePowerupAura(powerupAura);
        }
    }

    protected void DestroyPowerup()
    {
        GetComponent<ParticleSystem>().Stop(true);
    }

    public abstract void ApplyPowerup(GameObject player);
}
