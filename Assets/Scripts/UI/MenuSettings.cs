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
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] _screenRes;
    private CanvasGroup _canvasGroup;
    private UIHandler _uiHandler;


    private void Awake()
    {
        _uiHandler = GetComponentInParent<UIHandler>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _screenRes = Screen.resolutions;
        audioMixer.SetFloat("Master_Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") == 0 ? 0 : PlayerPrefs.GetFloat("masterVolume")) * 20);
        audioMixer.SetFloat("Music_Volume", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("musicVolume")) * 20);
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume") == 0 ? 0 : PlayerPrefs.GetFloat("sfxVolume")) * 20);
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

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("masterVolume", volume);
        audioMixer.SetFloat("Master_Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") == 0 ? 0 : PlayerPrefs.GetFloat("masterVolume")) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume") == 0 ? 0 : PlayerPrefs.GetFloat("sfxVolume")) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
        audioMixer.SetFloat("Music_Volume", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("musicVolume")) * 20);
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

    public void SwitchCheats(bool enabled)
    {
        GameManager.instance.SwitchCheats(enabled);
    }
}
