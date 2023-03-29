using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private CanvasGroup pauseCanvasGroup;
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private CanvasGroup deathScreenCanvasGroup;

    private Coroutine _settingsCoroutine;
    private Coroutine _pauseCoroutine;
    private Coroutine _hudCoroutine;

    private PlayerInput _playerInput;

    public static UIHandler Instance;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _playerInput = PlayerController.GetPlayerInput();

        _playerInput.PlayerControls.Pause.started += HandlePauseTrigger;
    }

    private void HandlePauseTrigger(InputAction.CallbackContext ctx)
    {
        if (settingsCanvasGroup.alpha > 0)
        {
            CloseSettings();
            OpenPauseMenu();
            return;
        }

        if (pauseCanvasGroup.alpha == 0)
        {
            AudioManager.Instance.PlayMenuMusic();
            OpenPauseMenu();
        }
        else
        {
            AudioManager.Instance.StopMenuMusic();
            ClosePauseMenu(true);
        }
    }

    public void OpenDeathScreen()
    {
        deathScreenCanvasGroup.transform.SetAsLastSibling();

        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, hudCanvasGroup));
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, deathScreenCanvasGroup));
    }

    public void OpenPauseMenu()
    {

        pauseCanvasGroup.transform.SetAsLastSibling();

        if (_pauseCoroutine != null) StopCoroutine(_pauseCoroutine);

        _pauseCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, pauseCanvasGroup));

        CloseHUD();
    }

    public void ClosePauseMenu(bool openHud)
    {

        if (_pauseCoroutine != null) StopCoroutine(_pauseCoroutine);

        _pauseCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, pauseCanvasGroup));

        if(openHud)
            OpenHUD();
    }

    public void OpenSettings()
    {
        if (_settingsCoroutine != null) StopCoroutine(_settingsCoroutine);

        _settingsCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, settingsCanvasGroup));
    }

    public void CloseSettings()
    {
        if (_settingsCoroutine != null) StopCoroutine(_settingsCoroutine);

        _settingsCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, settingsCanvasGroup));
    }

    public void OpenHUD()
    {

        Time.timeScale = 1;

        if (_hudCoroutine != null) StopCoroutine(_hudCoroutine);

        _hudCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, hudCanvasGroup));
    }

    public void CloseHUD()
    {

        Time.timeScale = 0;

        if (_hudCoroutine != null) StopCoroutine(_hudCoroutine);

        _hudCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, hudCanvasGroup));
    }
    
    public void Continue()
    {
        AudioManager.Instance.StopMenuMusic();
        ClosePauseMenu(true);
    }
}
