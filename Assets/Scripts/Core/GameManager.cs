using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _keysText;

    private bool _enabledCheats = false;
    private int _score = 0;
    private float _time = 0;
    private int _keys = 0;

    private ParticleSystem[] footstepParticles;

    public static GameManager instance;

    public void SwitchCheats(bool enabled)
    {
        _enabledCheats = enabled;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //print(SavingSystem.instance.GetPlayerData().playerName);

        footstepParticles = Resources.LoadAll<ParticleSystem>("Footsteps");

        instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameManager();
    }

    private void ResetGameManager()
    {
        _timeText = GameObject.FindGameObjectWithTag("timeText").GetComponentInChildren<TextMeshProUGUI>();
        _scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponentInChildren<TextMeshProUGUI>();
        _keysText = GameObject.FindGameObjectWithTag("keysText").GetComponentInChildren<TextMeshProUGUI>();

        _score = 0;
        _time = 0;
    }

    public ParticleSystem[] GetFootstepParticles()
    {
        return footstepParticles;
    }

    private void Update()
    {
        if (_timeText == null) return;

        _time += Time.deltaTime;
        _timeText.text = GetTime();
    }

    public void SpeedTime()
    {
        Time.timeScale *= 100;
    }

    public string GetTime()
    {
        int minutes = (int)_time / 60;

        if (minutes > 0)
        {
            float rawMinutes = _time / 60f;
            float seconds = (rawMinutes - (float)Math.Truncate(rawMinutes)) * 60f;

            //print("Minuti col decimale: " + rawMinutes + ", operazione: " + rawMinutes + " - " + (float)Math.Truncate(rawMinutes) + " quindi secondi: " + seconds);

            return minutes + "m " + Mathf.RoundToInt(seconds) + "s";
        }
        else
        {
            return Mathf.RoundToInt(_time).ToString() + "s";
        }
    }
    
    public void GiveUp()
    {
        SceneManager.LoadScene("Menu");
    }
    

    public void Restart()
    {

    }

    public float GetTimeRaw()
    {
        return _time;
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetKeys()
    {
        return _keys;
    }

    public void AddScore(int score)
    {
        _score += score;
        _scoreText.text = "Score: " + _score;
    }

    public void AddKey()
    {
        _keys++;
        _keysText.text = _keys + "/3";
    }

    public void Show(CanvasGroup canvasGroup)
    {
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, canvasGroup));
    }

    public void Hide(CanvasGroup canvasGroup)
    {
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, canvasGroup));
    }
}
