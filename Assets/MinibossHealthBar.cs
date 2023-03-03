using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUIMode { EUIM_Show, EUIM_Hide }

public class MinibossHealthBar : MonoBehaviour
{
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
}
