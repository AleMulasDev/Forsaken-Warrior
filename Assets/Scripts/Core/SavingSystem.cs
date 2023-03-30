using System;
using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public string playerName;
    public Vector3 position;
    public string sceneName;
    public int score;
    public float time;
    public int health;
    public int keys;
    public bool keyGathered;
    public bool minibossKilled;
}

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem Instance;

    private PlayerData loadedData;
    private GameObject _player;
    private string _path;

    private bool _loadWorldData = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public PlayerData GetPlayerData()
    {
        return loadedData;
    }

    public void SetLoadedData(PlayerData loadedData)
    {
        this.loadedData = loadedData;
        SceneManager.LoadScene("LevelChooser");
    }

    public void LoadSavedData()
    {
        if (loadedData.sceneName != String.Empty)
        {
            _loadWorldData = true;
            SceneManager.LoadScene(loadedData.sceneName);
        }
    }

    public void LoadDesiredLevel(string desiredLevel)
    {
        _loadWorldData = false;
        SceneManager.LoadScene(desiredLevel);
    }

    public bool ShouldLoadWorldData()
    {
        return _loadWorldData;
    }

    public void SaveGame()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _path = Application.persistentDataPath + "/" + loadedData.playerName + ".txt";

        PlayerData data = new PlayerData();

        data.playerName = loadedData.playerName;
        data.sceneName = SceneManager.GetActiveScene().name;
        data.position = _player.transform.position;
        data.score = GameManager.Instance.GetScore();
        data.time = GameManager.Instance.GetTimeRaw();
        data.health = _player.GetComponent<Health>().GetHealth();
        data.keys = GameManager.Instance.GetKeys();
        data.keyGathered = GameManager.Instance.GetKeyGathered();
        data.minibossKilled = GameManager.Instance.GetMinibossKilled();

        string json = JsonUtility.ToJson(data);

        using (StreamWriter sw = File.CreateText(_path))
        {
            sw.WriteLine(json);
        }
    }
}
