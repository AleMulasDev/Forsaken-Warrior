using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupPowerup : Powerup
{
    protected PowerupManager _powerupManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            _powerupManager = other.GetComponent<PowerupManager>();
            ApplyPowerup(other.gameObject);
        }
    }
}
