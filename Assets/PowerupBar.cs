using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupBar : MonoBehaviour
{
    [SerializeField] private Image powerupImage; 
    [SerializeField] private Image powerupFill;


    private CanvasGroup _cg;
    private float _powerupDuration;
    private float _tempPowerupDuration;

    private void Start()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    public void SetPowerupBar(float powerupDuration)
    {
        _powerupDuration = powerupDuration;
        _tempPowerupDuration = powerupDuration;
        StartCoroutine(PowerupBarCoroutine(EUIMode.EUIM_Show));
    }

    public void Hide()
    {
        StartCoroutine(PowerupBarCoroutine(EUIMode.EUIM_Hide));
    }

    private void Update()
    {
        if (_powerupDuration > 0)
        {
            powerupFill.fillAmount = _tempPowerupDuration / _powerupDuration;
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
