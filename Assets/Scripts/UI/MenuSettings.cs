using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class MenuSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mouseSensValue;
    [SerializeField] private TextMeshProUGUI cameraSensValue;
    [SerializeField] private CanvasGroup pauseCanvasGroup;

    private Resolution[] _screenRes;
    private CanvasGroup _canvasGroup;
    private UIHandler _uiHandler;

    private void Awake()
    {
        _uiHandler = GetComponentInParent<UIHandler>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _screenRes = Screen.resolutions;
        //audioMixer.SetFloat("musicVol", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("musicVolume")) * 20);
    }

    public void OpenPauseMenu()
    {
        _uiHandler.OpenPauseMenu();
        _uiHandler.CloseSettings();
        pauseCanvasGroup.transform.SetAsLastSibling();
    }

    public void ChangeCameraSens(float value)
    {
        cameraSensValue.text = value.ToString();
    }

    public void ChangeMouseSens(float value)
    {
        mouseSensValue.text = value.ToString();
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
        //audioMixer.SetFloat("musicVol", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("musicVolume")) * 20);
    }

    public void SetBrightness(float brightness)
    {
        Screen.brightness = brightness;
        PlayerPrefs.SetFloat("brightness", brightness);
    }

    public void SetRenderDistance(float renderDistance)
    {
        PlayerPrefs.SetFloat("renderDistance", renderDistance);
    }
    public void ChangeQuality(float index)
    {
        int graphicsIndex = Mathf.RoundToInt(index);
        QualitySettings.SetQualityLevel(graphicsIndex);
        PlayerPrefs.SetInt("graphic", graphicsIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", (isFullscreen ? 1 : 0));
    }

    public void SetResolution(int resIndex)
    {
        if (_screenRes == null)
            return;

        Screen.SetResolution(_screenRes[resIndex].width, _screenRes[resIndex].height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", resIndex);
    }

    public void SetVsync(bool isVsync)
    {
        if (isVsync)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;

        PlayerPrefs.SetInt("vsync", (isVsync ? 1 : 0));
    }
}
