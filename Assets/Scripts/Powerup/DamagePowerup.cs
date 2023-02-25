using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePowerup : Powerup
{
    [SerializeField] private float powerupDuration;

    private PowerupManager _powerupManager;
    public override void ApplyPowerup(GameObject player)
    {
        _powerupManager = player.GetComponent<PowerupManager>();
        InstantiatePowerupAura(powerupDuration, player.GetComponent<PowerupManager>());
        StartCoroutine(_powerupManager.PowerupCoroutine(powerupDuration));
        DestroyPowerup();
    }
}
