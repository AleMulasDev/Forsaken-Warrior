using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup hudCanvasGroup;
    private PlayerInput _playerInput;
    private CanvasGroup _canvasGroup;

    private Coroutine _pauseMenucoroutine;
    private Coroutine _hudCoroutine;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _playerInput = PlayerController.GetPlayerInput();

        _playerInput.PlayerControls.Pause.started += HandlePauseTrigger;
    }

    private void HandlePauseTrigger(InputAction.CallbackContext ctx)
    {
        if (_pauseMenucoroutine != null) StopCoroutine(_pauseMenucoroutine);

        if (_canvasGroup.alpha < 1)
            Pause();
        else
            Unpause();
    }

    private void Pause()
    {
        _pauseMenucoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, _canvasGroup));
        Time.timeScale = 0;

        if (_hudCoroutine != null) StopCoroutine(_hudCoroutine);

        _hudCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, hudCanvasGroup));
    }

    private void Unpause()
    {
        _pauseMenucoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, _canvasGroup));
        Time.timeScale = 1;

        if (_hudCoroutine != null) StopCoroutine(_hudCoroutine);

        _hudCoroutine = StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, hudCanvasGroup));
    }

    public void Continue()
    {
        Unpause();
    }
}
