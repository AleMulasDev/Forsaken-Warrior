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
    public int score;
    public float time;
    public int health;
    public int keys;
}

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem instance;

    private PlayerData loadedData;
    private GameObject _player;
    private string _path;
    private void Awake()
    {
        if(instance == null)
            instance = this;

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }
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

    public void SaveGame()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _path = Application.persistentDataPath + "/maswa.txt";

        PlayerData data = new PlayerData();

        data.playerName = "Maswa";
        data.sceneName = SceneManager.GetActiveScene().name;
        data.position = _player.transform.position;
        data.score = GameManager.instance.GetScore();
        data.time = GameManager.instance.GetTimeRaw();
        data.health = _player.GetComponent<Health>().GetHealth();
        data.keys = GameManager.instance.GetKeys();

        string json = JsonUtility.ToJson(data);

        using (StreamWriter sw = File.CreateText(_path))
        {
            sw.WriteLine(json);
        }
    }
}
