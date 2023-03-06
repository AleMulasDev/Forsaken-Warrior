using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupBar : MonoBehaviour
{
    [SerializeField] private Image powerupImage; 
    [SerializeField] private Slider powerupFill;
    [SerializeField] private Image powerupFillImage;
    [SerializeField] private Image powerupFrame;

    private CanvasGroup _cg;
    private Powerup _currentPowerup;

    private float _powerupDuration;
    private float _tempPowerupDuration;

    private void Start()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    public void SetPowerupBar(float powerupDuration, TimerPowerup powerup)
    {
        _currentPowerup = powerup;
        _powerupDuration = powerupDuration;
        _tempPowerupDuration = powerupDuration;
        StartCoroutine(PowerupBarCoroutine(EUIMode.EUIM_Show));
        powerupImage.sprite = powerup.GetSprite();
        powerupFillImage.color = powerup.GetPowerupColor();
        powerupFrame.color = powerup.GetPowerupColor();
    }

    public void Hide()
    {
        StartCoroutine(PowerupBarCoroutine(EUIMode.EUIM_Hide));
    }

    private void Update()
    {
        if (_powerupDuration > 0)
        {
            powerupFill.value = _tempPowerupDuration / _powerupDuration;
            _tempPowerupDuration -= Time.deltaTime;
        }
    }

    private IEnumerator PowerupBarCoroutine(EUIMode uiMode)
    {
        switch (uiMode)
        {
            case EUIMode.EUIM_Show:
                while (_cg.alpha < 1.0f)
                {
                    _cg.alpha += 0.01f;
                    yield return null;
                }
                break;
            case EUIMode.EUIM_Hide:
                while (_cg.alpha > 0f)
                {
                    _cg.alpha -= 0.01f;
                    yield return null;
                }
                break;
        }

    }
}
