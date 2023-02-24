using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotPowerup : Powerup
{
    [SerializeField] private float powerupDuration;

    private PlayerController _playerController;
    public override void ApplyPowerup(GameObject player)
    {
        _playerController = player.GetComponent<PlayerController>();
        InstantiatePowerupAura(powerupDuration, _playerController);
        StartCoroutine(_playerController.OneShotCoroutine(powerupDuration));
        DestroyPowerup();
    }
}
