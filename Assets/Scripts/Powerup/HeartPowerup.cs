using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeartPowerup : PickupPowerup
{
    [SerializeField] private int heartAmount;

    public override void ApplyPowerup(GameObject player)
    {
        player.GetComponent<PlayerHealth>().AddHeart(heartAmount);
        InstantiatePowerupAura(-1, player.GetComponent<PowerupManager>());
        Destroy(gameObject);
    }

}
