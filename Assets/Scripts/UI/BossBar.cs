using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    private Slider _slider;
    private Coroutine _coroutine;

    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    public void UpdateBossBar(float value)
    {
        if(_coroutine != null )
            StopCoroutine(_coroutine);    

        _coroutine = StartCoroutine(UpdateBossBarCoroutine(value));
    }

    private IEnumerator UpdateBossBarCoroutine(float amount)
    {
        while (amount > _slider.value)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, amount, .5f * Time.deltaTime);
            yield return null;
        }
    }
}
