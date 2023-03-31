using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class MenuSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cameraSensValue;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] _screenRes;


    private void Awake()
    {
        _screenRes = Screen.resolutions;
        audioMixer.SetFloat("Master_Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") == 0 ? 0 : PlayerPrefs.GetFloat("masterVolume")) * 20);
        audioMixer.SetFloat("MenuMusic_Volume", Mathf.Log10(PlayerPrefs.GetFloat("menuMusicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("menuMusicVolume")) * 20);
        audioMixer.SetFloat("GameMusic_Volume", Mathf.Log10(PlayerPrefs.GetFloat("gameMusicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("gameMusicVolume")) * 20);
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume") == 0 ? 0 : PlayerPrefs.GetFloat("sfxVolume")) * 20);
    }

    public void ChangeCameraSens(float value)
    {
        cameraSensValue.text = value.ToString();
        PlayerPrefs.SetFloat("cameraSens", value);

        if (CinemachineShake.instance != null)
            CinemachineShake.instance.UpdateSpeed();
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("masterVolume", volume);

        if (volume > 0)
        {
            audioMixer.SetFloat("Master_Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") == 0 ? 0 : PlayerPrefs.GetFloat("masterVolume")) * 20);
        }
        else
            audioMixer.SetFloat("Master_Volume", -80);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);

        if (volume > 0)
        {
            audioMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume") == 0 ? 0 : PlayerPrefs.GetFloat("sfxVolume")) * 20);
        } else
            audioMixer.SetFloat("SFX_Volume", -80);
    }

    public void SetMenuMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("menuMusicVolume", volume);

        if (volume > 0)
        {
            audioMixer.SetFloat("MenuMusic_Volume", Mathf.Log10(PlayerPrefs.GetFloat("menuMusicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("menuMusicVolume")) * 20);
        }
        else
            audioMixer.SetFloat("MenuMusic_Volume", -80);
    }

    public void SetGameMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("gameMusicVolume", volume);

        if (volume > 0)
        {
            audioMixer.SetFloat("GameMusic_Volume", Mathf.Log10(PlayerPrefs.GetFloat("gameMusicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("gameMusicVolume")) * 20);
        }
        else
            audioMixer.SetFloat("GameMusic_Volume", -80);
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
        GameManager.Instance.SwitchCheats(enabled);
    }
}
