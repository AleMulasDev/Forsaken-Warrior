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

    private Coroutine _hideCoroutine;

    private void Start()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    public void SetPowerupBar(float powerupDuration, TimerPowerup powerup)
    {
        _currentPowerup = powerup;
        _powerupDuration = powerupDuration;
        _tempPowerupDuration = powerupDuration;
        powerupImage.sprite = powerup.GetSprite();
        powerupFillImage.color = powerup.GetPowerupColor();
        powerupFrame.color = powerup.GetPowerupColor();

        if(_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }

        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, _cg));
    }

    public void Hide()
    {
        _hideCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, _cg));
    }

    private void Update()
    {
        if (_powerupDuration > 0)
        {
            powerupFill.value = _tempPowerupDuration / _powerupDuration;
            _tempPowerupDuration -= Time.deltaTime;
        }
    }
}
