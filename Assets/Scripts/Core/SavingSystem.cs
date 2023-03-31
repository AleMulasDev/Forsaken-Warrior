using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public string playerName;
    public Vector3 position;
    public string sceneName;
    public int totalScore;
    public int sceneScore;
    public float sceneTime;
    public float totalTime;
    public int currentHealth;
    public int maxHealth;
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
        _path = Application.persistentDataPath + "/" + loadedData.playerName + ".txt";

        if (File.Exists(_path))
        {
            print("Il file esiste");
            return JsonUtility.FromJson<PlayerData>(File.ReadAllText(_path));
        }
        else
        {
            print("Il file non esiste");
            return loadedData;
        }
    }

    public void SetLoadedData(PlayerData loadedData)
    {
        this.loadedData = loadedData;
        SceneManager.LoadScene("LevelChooser");
    }

    public void ChangeShouldLoadWorldData(bool shouldLoad)
    {
        _loadWorldData = shouldLoad;
    }

    public void LoadSavedData()
    {
        if (loadedData.sceneName != String.Empty)
        {
            _loadWorldData = true;
            SceneManager.LoadScene(loadedData.sceneName);
        }
        else
            FindObjectOfType<LevelChooser>().ShowWarning("there is no saved level in the current data, please choose a level below");
    }

    public void LoadDesiredLevel(string desiredLevel)
    {
        if (desiredLevel == "HauntedHouse" && loadedData.keys != 3)
        {
            FindObjectOfType<LevelChooser>().ShowWarning("You need 3 keys to access to Haunted House level");
            return;
        }

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
        data.sceneScore = GameManager.Instance.GetScore();
        data.sceneTime = GameManager.Instance.GetTimeRaw();
        data.totalTime = GameManager.Instance.GetTotalTime();
        data.totalScore = GameManager.Instance.GetTotalScore();
        data.currentHealth = _player.GetComponent<Health>().GetHealth();
        data.maxHealth = _player.GetComponent<Health>().GetMaxHealth();
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
