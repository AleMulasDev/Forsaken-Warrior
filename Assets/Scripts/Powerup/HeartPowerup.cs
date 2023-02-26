using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeartPowerup : Powerup
{
    [SerializeField] private float heartAmount;

    public override void ApplyPowerup(GameObject player)
    {
        player.GetComponent<Health>().AddHeart(heartAmount);
        InstantiatePowerupAura(-1, player.GetComponent<PowerupManager>());
        HidePowerup();
    }

}
