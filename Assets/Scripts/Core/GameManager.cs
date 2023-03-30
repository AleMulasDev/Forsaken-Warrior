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
    [SerializeField] private TMP_InputField inputField;
    Transform _spawnPoint;
    private int _score = 0;
    private float _time = 0;
    private int _keys = 0;

    private ParticleSystem[] footstepParticles;

    public static GameManager Instance;

    private bool _minibossKilled = false;
    private bool _keyGathered = false;
    private Portal _portal;

    private AudioClip _victoryAudioClip;

    private void Start()
    {
        _victoryAudioClip = Resources.Load<AudioClip>("VictoryAudioClip");
        footstepParticles = Resources.LoadAll<ParticleSystem>("Footsteps");
    }

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
        _spawnPoint.position = newSpawnPoint.position;
    }

    private IEnumerator RespawnCoroutine(Transform player)
    {
        yield return new WaitForSeconds(1f);
        player.gameObject.SetActive(true);
        player.gameObject.GetComponent<CharacterController>().Move(_spawnPoint.position - player.position);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Respawn(Transform player)
    {
        StartCoroutine(RespawnCoroutine(player));
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _portal = FindObjectOfType<Portal>();
        _score = 0;
        _time = 0;
        _scoreText.text = "0";
        _timeText.text = "0s";
        _keyGathered = false;
        _minibossKilled = false;
        _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

        if (SavingSystem.Instance.ShouldLoadWorldData())
        {
            PlayerData data = SavingSystem.Instance.GetPlayerData();
            player.transform.position = data.position;
            _score = data.score;
            _time = data.time;
            _keyGathered = data.keyGathered;
            _minibossKilled = data.minibossKilled;
        }
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
        _keysText.text = _keys + "/3";
        _scoreText.text = "Score: " + _score;

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

    public void ShowVictoryScreen()
    {
        AudioManager.Instance.PlayGameMusicOneShot(_victoryAudioClip);
        _portal.ActivatePortal();
    }

    public void GiveUp()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void NewGame()
    {
        PlayerData newData = new PlayerData();
        newData.playerName = inputField.text;
        newData.sceneName = String.Empty;
        SavingSystem.Instance.SetLoadedData(newData);
    }

    public bool GetKeyGathered()
    {
        return _keyGathered;
    }

    public bool GetMinibossKilled()
    {
        return _minibossKilled;
    }
}
