using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Utils
{
    public static IEnumerator UIWindowHandler(EUIMode uiMode, CanvasGroup canvasGroup)
    {
        switch (uiMode)
        {
            case EUIMode.EUIM_Show:
                canvasGroup.gameObject.SetActive(true);
                while (canvasGroup.alpha < 1.0f)
                {
                    canvasGroup.alpha += 0.01f;
                    yield return null;
                }
                break;
            case EUIMode.EUIM_Hide:
                while (canvasGroup.alpha > 0f)
                {
                    canvasGroup.alpha -= 0.01f;
                    yield return null;
                }
                canvasGroup.gameObject.SetActive(false);
                break;
        }

    }
}
