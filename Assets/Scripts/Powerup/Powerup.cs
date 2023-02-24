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
            ApplyPowerup(other.gameObject);
        }
    }

    protected void InstantiatePowerupAura(float powerupDuration, PlayerController _playerController)
    {
        if (powerupDuration > 0)
        {
            _playerController.InstantiatePowerupAura(powerupAura, powerupDuration);
        } else
        {
            _playerController.InstantiatePowerupAura(powerupAura);
        }
    }

    protected void DestroyPowerup()
    {
        GetComponent<ParticleSystem>().Stop(true);
        Destroy(gameObject, 2f);
    }

    public abstract void ApplyPowerup(GameObject player);
}
