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

    [SerializeField] private bool _enabledCheats = false;
    [SerializeField] Transform spawnPoint;
    private int _score = 0;
    private float _time = 0;
    private int _keys = 0;

    private ParticleSystem[] footstepParticles;

    public static GameManager Instance;

    private bool _minibossKilled = false;
    private bool _keyGathered = false;
    private Portal _portal;

    public void SwitchCheats(bool enabled)
    {
        _enabledCheats = enabled;
    }

    public void MinibossKilled()
    {
        _minibossKilled = true;
        SpawnPortal();
    }

    public bool AreCheatsEnabled()
    {
        return _enabledCheats;
    }

    public void ChangeSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint.position = newSpawnPoint.position;
    }

    public void Respawn(Transform player)
    {
        player.position = spawnPoint.position;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        footstepParticles = Resources.LoadAll<ParticleSystem>("Footsteps");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" || scene.name == "LevelChooser")
            return;

        ResetGameManager();
    }

    private void ResetGameManager()
    {
        _timeText = GameObject.FindGameObjectWithTag("timeText").GetComponentInChildren<TextMeshProUGUI>();
        _scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponentInChildren<TextMeshProUGUI>();
        _keysText = GameObject.FindGameObjectWithTag("keysText").GetComponentInChildren<TextMeshProUGUI>();

        _portal = FindObjectOfType<Portal>();
        _score = 0;
        _time = 0;
        _scoreText.text = "0";
        _timeText.text = "0s";
        _keyGathered = false;
        _minibossKilled = false;
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
    
    public void SpawnPortal()
    {
        if(_keyGathered && _minibossKilled)
        {
            _portal.ActivatePortal();
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
        _keyGathered = true;
        _keys++;
        _keysText.text = _keys + "/3";
        SpawnPortal();
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
