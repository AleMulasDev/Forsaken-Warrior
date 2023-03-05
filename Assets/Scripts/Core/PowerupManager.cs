using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] private PowerupBar powerupBar;
    [SerializeField] private float _pickupAuraDuration;

    private TimerPowerup _currentPowerup = null;

    public void DisablePowerup()
    {
        powerupBar.Hide();
        Destroy(_currentPowerup.gameObject);
        _currentPowerup = null;
    }

    public void StartPowerup(float powerupDuration)
    {
        StartCoroutine(PowerupCoroutine(powerupDuration));
    }

    public IEnumerator PowerupCoroutine(float powerupDuration)
    {
        powerupBar.SetPowerupBar(powerupDuration, _currentPowerup);
        yield return new WaitForSeconds(powerupDuration);
        DisablePowerup();
    }

    private IEnumerator HandleAura(float powerupDuration, ParticleSystem auraPowerup)
    {
        yield return new WaitForSeconds(powerupDuration);
        auraPowerup.Stop(true);
    }

    // When powerup is more likely a simple pickup
    public void InstantiatePowerupAura(ParticleSystem auraPowerup)
    {
        ParticleSystem auraPowerupInstance = Instantiate(auraPowerup, transform);
        StartCoroutine(HandleAura(_pickupAuraDuration, auraPowerupInstance));
    }

    // When powerup actually has a timer 
    public void InstantiatePowerupAura(ParticleSystem auraPowerup, float powerupDuration)
    {
        ParticleSystem auraPowerupInstance = Instantiate(auraPowerup, transform);
        StartCoroutine(HandleAura(powerupDuration, auraPowerupInstance));
    }

    public void SetCurrentPowerup(Powerup currentPowerup) {
        this._currentPowerup = (TimerPowerup)currentPowerup;
    }

    public Powerup GetCurrentPowerup() { return _currentPowerup; }
}
