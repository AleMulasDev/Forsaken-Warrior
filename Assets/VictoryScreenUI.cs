using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreenUI : MonoBehaviour
{
    [SerializeField] CanvasGroup textCanvasGroup;

    public void ShowText()
    {
        StartCoroutine(ShowTextCoroutine());
    }

    private IEnumerator ShowTextCoroutine()
    {
        yield return new WaitForSeconds(1f);
        textCanvasGroup.interactable = true;
        while (textCanvasGroup.alpha < 1.0f)
        {
            textCanvasGroup.alpha += 0.01f;
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }

        SceneManager.LoadScene("Menu");
    }
}
