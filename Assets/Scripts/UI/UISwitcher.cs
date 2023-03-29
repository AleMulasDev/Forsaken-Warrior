using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UISwitcher : MonoBehaviour
{
    private GameObject _currentWindow;

    public void SwitchTo(GameObject newWindow)
    {
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, newWindow.GetComponent<CanvasGroup>()));
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, _currentWindow.GetComponent<CanvasGroup>()));

        foreach (EventTrigger et in newWindow.gameObject.GetComponentsInChildren<EventTrigger>())
            et.enabled = true;

        foreach (EventTrigger et in _currentWindow.gameObject.GetComponentsInChildren<EventTrigger>())
            et.enabled = false;

        newWindow.transform.SetAsLastSibling();
    }

    public void SetCurrentWindow(GameObject currentWindow)
    {
        _currentWindow = currentWindow;
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
