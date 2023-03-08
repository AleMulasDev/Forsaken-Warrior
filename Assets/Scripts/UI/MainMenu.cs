using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform loadContentRoot;
    [SerializeField] private Button loadButton;

    private string _path;
    public void NewGame()
    {
        SceneManager.LoadScene("Isles");
    }

    public void Start()
    {
        _path = Application.persistentDataPath;

        //PlayerData loadedData = new PlayerData();

        //loadedData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(_path));

        //SceneManager.LoadScene(loadedData.sceneName);

        string[] files = Directory.GetFiles(_path);

        foreach(string file in files)
        {
            Instantiate(loadButton, loadContentRoot);
        }
    }
}
