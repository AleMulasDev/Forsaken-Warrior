using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerup : Powerup
{
    [SerializeField] private float healthAmount;
    public override void ApplyPowerup(GameObject player)
    {
        player.GetComponent<PlayerHealth>().AddHealth(healthAmount);
        InstantiatePowerupAura(-1, player.GetComponent<PowerupManager>());
        HidePowerup();
    }
}
