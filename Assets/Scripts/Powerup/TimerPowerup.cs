using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimerPowerup : Powerup
{
    [SerializeField] private Sprite powerupSprite;
    [SerializeField] private Color powerupColor;
    [SerializeField] protected float powerupDuration;

    protected PowerupManager _powerupManager;

    public Sprite GetSprite() { return powerupSprite; }

    public Color GetPowerupColor() { return powerupColor; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            _powerupManager = other.GetComponent<PowerupManager>();

            if (_powerupManager.GetCurrentPowerup() == null)
            {
                _powerupManager.SetCurrentPowerup(this);
                ApplyPowerup(other.gameObject);
            }
        }
    }
}
