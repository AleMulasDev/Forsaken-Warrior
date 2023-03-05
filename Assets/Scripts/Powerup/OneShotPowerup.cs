using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotPowerup : TimerPowerup
{
    private PowerupManager _powerupManager;
    public override void ApplyPowerup(GameObject player)
    {
        _powerupManager = player.GetComponent<PowerupManager>();
        InstantiatePowerupAura(powerupDuration, player.GetComponent<PowerupManager>());
        _powerupManager.StartPowerup(powerupDuration);
        HidePowerup();
    }
}
