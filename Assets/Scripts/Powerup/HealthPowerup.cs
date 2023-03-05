using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerup : PickupPowerup
{
    [SerializeField] private int healthAmount;
    public override void ApplyPowerup(GameObject player)
    {
        player.GetComponent<PlayerHealth>().AddHealth(healthAmount);
        InstantiatePowerupAura(-1, player.GetComponent<PowerupManager>());
        Destroy(gameObject);
    }
}
