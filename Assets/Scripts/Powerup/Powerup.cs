using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private ParticleSystem powerupAura;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            other.GetComponent<PowerupManager>().SetCurrentPowerup(this);
            ApplyPowerup(other.gameObject);
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
