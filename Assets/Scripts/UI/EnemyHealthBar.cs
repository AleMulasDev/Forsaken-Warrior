using AmazingAssets.AdvancedDissolve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EUIMode { EUIM_Show, EUIM_Hide }

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image backFill;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();    
    }
    public void ShowHealthBar()
    {
        gameObject.SetActive(true);
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, _canvasGroup));
    }

    public void HideHealthBar()
    {
        gameObject.SetActive(true);
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, _canvasGroup));
    }

    private IEnumerator UpdateHealthBarCoroutine(float amount)
    {
        while(backFill.fillAmount > amount)
        {
            backFill.fillAmount = Mathf.MoveTowards(backFill.fillAmount, amount, .5f * Time.deltaTime);
            yield return null;
        }
    }

    public void UpdateHealthBar(float amount)
    {
        StartCoroutine(UpdateHealthBarCoroutine(amount));
    }
}
