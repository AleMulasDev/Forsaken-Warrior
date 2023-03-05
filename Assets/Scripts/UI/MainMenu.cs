using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string _path;
    public void NewGame()
    {
        SceneManager.LoadScene("Isles");
    }

    public void LoadGame()
    {
        _path = Application.persistentDataPath + "/savedata.txt";

        PlayerData loadedData = new PlayerData();

        loadedData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(_path));

        SceneManager.LoadScene(loadedData.sceneName);
    }
}
