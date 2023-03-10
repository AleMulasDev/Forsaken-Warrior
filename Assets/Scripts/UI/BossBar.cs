using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    public void UpdateBossBar(float value)
    {
        StartCoroutine(UpdateBossBarCoroutine(value));
    }

    private IEnumerator UpdateBossBarCoroutine(float amount)
    {
        while (_slider.value > amount)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, amount, .5f * Time.deltaTime);
            yield return null;
        }
    }
}
