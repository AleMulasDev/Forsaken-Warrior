using Cinemachine;
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
    [SerializeField] private CanvasGroup manualCanvasGroup;

    private Coroutine _settingsCoroutine;
    private Coroutine _pauseCoroutine;
    private Coroutine _hudCoroutine;
    private Coroutine _manualCoroutine;

    private PlayerInput _playerInput;

    public static UIHandler Instance;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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
            AudioManager.Instance.PauseGameMusic();
            OpenPauseMenu();
        }
        else
        {
            AudioManager.Instance.StopMenuMusic();
            AudioManager.Instance.ResumeGameMusic();
            ClosePauseMenu(true);
        }
    }

    public void OpenDeathScreen()
    {
        Camera.main.gameObject.GetComponent<CinemachineBrain>().enabled = false;
        deathScreenCanvasGroup.transform.SetAsLastSibling();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, hudCanvasGroup));
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, deathScreenCanvasGroup));
    }

    public void OpenPauseMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

    public void OpenManual()
    {
        if (_manualCoroutine != null) StopCoroutine(_manualCoroutine);

        _manualCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, manualCanvasGroup));
        manualCanvasGroup.gameObject.transform.SetAsLastSibling();
    }

    public void CloseManual()
    {
        if (_manualCoroutine != null) StopCoroutine(_manualCoroutine);

        _manualCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, manualCanvasGroup));
    }

    public void OpenSettings()
    {
        if (_settingsCoroutine != null) StopCoroutine(_settingsCoroutine);

        _settingsCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, settingsCanvasGroup));
        settingsCanvasGroup.gameObject.transform.SetAsLastSibling();
    }

    public void CloseSettings()
    {
        if (_settingsCoroutine != null) StopCoroutine(_settingsCoroutine);

        _settingsCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, settingsCanvasGroup));
    }

    public void OpenHUD()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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

    public void GiveUp()
    {
        GameManager.Instance.GiveUp();
    }

    public void TryAgain()
    {
        GameManager.Instance.TryAgain();
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void SaveGame()
    {
        SavingSystem.Instance.SaveGame();
    }
}
