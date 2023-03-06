using AmazingAssets.AdvancedDissolve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EUIMode { EUIM_Show, EUIM_Hide }

public class MinibossHealthBar : MonoBehaviour
{
    [SerializeField] private Image backFill;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();    
    }
    public void ShowHealthBar()
    {
        StartCoroutine(HealthBarCoroutine(EUIMode.EUIM_Show));
    }

    public void HideHealthBar()
    {
        StartCoroutine(HealthBarCoroutine(EUIMode.EUIM_Hide));
    }

    private IEnumerator UpdateHealthBarCoroutine(float amount)
    {
        while(backFill.fillAmount > amount)
        {
            backFill.fillAmount = Mathf.MoveTowards(backFill.fillAmount, amount, .5f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator HealthBarCoroutine(EUIMode uiMode)
    {
        switch(uiMode)
        {
            case EUIMode.EUIM_Show:
                while (_canvasGroup.alpha < 1.0f)
                {
                    _canvasGroup.alpha += 0.01f;
                    yield return null;
                }
                break;
            case EUIMode.EUIM_Hide:
                while (_canvasGroup.alpha > 0f)
                {
                    _canvasGroup.alpha -= 0.01f;
                    yield return null;
                }
                break;
        }
        
    }

    public void UpdateHealthBar(float amount)
    {
        StartCoroutine(UpdateHealthBarCoroutine(amount));
    }
}
