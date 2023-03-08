using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI keysText;

    private int _score = 0;
    private float _time = 0;
    private int _keys = 0;

    public static GameManager instance;

    private void Start()
    {
        print(SavingSystem.instance.GetPlayerData().playerName);

        instance = this;

        if(scoreText != null)
            scoreText.text = "Score: " + _score;
    }

    private void Update()
    {
        if (timeText == null) return;

        _time += Time.deltaTime;
        timeText.text = GetTime();
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
        scoreText.text = "Score: " + _score;
    }

    public void AddKey()
    {
        _keys++;
        keysText.text = _keys + "/3";
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
