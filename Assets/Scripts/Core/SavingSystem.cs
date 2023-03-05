using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public Vector3 position;
    public string sceneName;
    public int score;
    public float time;
    public int health;
    public int keys;
}

public class SavingSystem : MonoBehaviour
{
    private GameObject _player;
    private string _path;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SaveGame()
    {
        _path = Application.persistentDataPath + "/savedata.txt";

        PlayerData data = new PlayerData();

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
