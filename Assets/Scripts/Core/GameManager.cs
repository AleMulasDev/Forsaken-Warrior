using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _keysText;

    [SerializeField] private bool _enabledCheats = false;
    [SerializeField] private AudioMixer audioMixer;
    Transform _spawnPoint;
    private int _score = 0;
    private float _time = 0;
    private int _keys = 0;

    private float _totalTime = 0;
    private int _totalScore = 0;

    private ParticleSystem[] footstepParticles;

    public static GameManager Instance;

    private bool _minibossKilled = false;
    private bool _keyGathered = false;
    private Portal _portal;
    private CanvasGroup _cGVictoryScreen;
    private AudioClip _victoryAudioClip;

    private void Start()
    {
        _victoryAudioClip = Resources.Load<AudioClip>("VictoryAudioClip");
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

        footstepParticles = Resources.LoadAll<ParticleSystem>("Footsteps");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void UpdateTotalRecord()
    {
        _totalScore = SavingSystem.Instance.GetPlayerData().totalScore;
        _totalTime = SavingSystem.Instance.GetPlayerData().totalTime;
        _totalScore += _score;
        _totalTime += _time;

        SavingSystem.Instance.SaveGame();
    }

    public int GetTotalScore()
    {
        return _totalScore;
    }

    public float GetTotalTime()
    {
        return _totalTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        audioMixer.SetFloat("GameMusic_Volume", Mathf.Log10(PlayerPrefs.GetFloat("gameMusicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("gameMusicVolume")) * 20);
        audioMixer.SetFloat("MenuMusic_Volume", Mathf.Log10(PlayerPrefs.GetFloat("menuMusicVolume") == 0 ? 0 : PlayerPrefs.GetFloat("menuMusicVolume")) * 20);
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume") == 0 ? 0 : PlayerPrefs.GetFloat("sfxVolume")) * 20);
        audioMixer.SetFloat("Master_Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") == 0 ? 0 : PlayerPrefs.GetFloat("masterVolume")) * 20);

        if (scene.name == "Menu" || scene.name == "LevelChooser")
            return;

        if (scene.name == "HauntedHouseT" || scene.name == "HauntedHouse")
            _cGVictoryScreen = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CanvasGroup>();

        ResetGameManager();
    }

    private void ResetGameManager()
    {
        PlayerData data = SavingSystem.Instance.GetPlayerData();
        _timeText = GameObject.FindGameObjectWithTag("timeText").GetComponentInChildren<TextMeshProUGUI>();
        _scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponentInChildren<TextMeshProUGUI>();
        _keysText = GameObject.FindGameObjectWithTag("keysText").GetComponentInChildren<TextMeshProUGUI>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _portal = FindObjectOfType<Portal>();
        _score = 0;
        _time = 0;
        print("Reset game manager..");
        print("Chiavi trovati nel salvataggio: " + data.keys);
        _keys = data.keys;
        _keysText.text = _keys + "/3";
        _scoreText.text = "0";
        _timeText.text = "0s";
        _keyGathered = false;
        _minibossKilled = false;
        _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        _totalScore = data.totalScore;
        _totalTime = data.totalTime;

        if (SavingSystem.Instance.ShouldLoadWorldData())
        {
            player.transform.position = data.position;
            _score = data.sceneScore;
            _time = data.sceneTime;
            _keyGathered = data.keyGathered;
            _minibossKilled = data.minibossKilled;
            _keys = data.keys;
            _keysText.text = _keys + "/3";
            player.GetComponent<PlayerHealth>().SetHealth(data.currentHealth, data.maxHealth);

            if (_keyGathered && _minibossKilled)
                _portal.ResetPortal();
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
        _timeText.text = GetTime(_time);
        _keysText.text = _keys + "/3";
        _scoreText.text = "Score: " + _score;

    }

    public void SpeedTime()
    {
        Time.timeScale *= 100;
    }

    public string GetTime(float time)
    {
        int minutes = (int)time / 60;

        if (minutes > 0)
        {
            float rawMinutes = time / 60f;
            float seconds = (rawMinutes - (float)Math.Truncate(rawMinutes)) * 60f;

            //print("Minuti col decimale: " + rawMinutes + ", operazione: " + rawMinutes + " - " + (float)Math.Truncate(rawMinutes) + " quindi secondi: " + seconds);

            return minutes + "m " + Mathf.RoundToInt(seconds) + "s";
        }
        else
        {
            return Mathf.RoundToInt(time).ToString() + "s";
        }
    }
    
    public void SpawnPortal()
    {
        if(_keyGathered && _minibossKilled)
        {
            _portal.ActivatePortal(true);
        }
    }

    public void ShowVictoryScreen()
    {
        AudioManager.Instance.PlayGameMusicOneShot(_victoryAudioClip);
        SavingSystem.Instance.SaveGame();
        StartCoroutine(ShowVictoryScreenCoroutine());
    }

    private IEnumerator ShowVictoryScreenCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _cGVictoryScreen.interactable = true;
        while (_cGVictoryScreen.alpha < 1.0f)
        {
            _cGVictoryScreen.alpha += 0.01f;
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        _cGVictoryScreen.gameObject.GetComponent<VictoryScreenUI>().ShowText();
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
        string playerName = FindObjectOfType<MainMenu>().GetInputText();
        if (File.Exists(Application.persistentDataPath + "/" + playerName + ".txt"))
        {
            FindObjectOfType<MainMenu>().ShowDataAlreadyExists();
            return;
        }

        PlayerData newData = new PlayerData();
        newData.playerName = playerName;
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
