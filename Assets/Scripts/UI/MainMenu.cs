using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform loadContentRoot;
    [SerializeField] private LoadButton loadButton;

    private string _path;

    public void NewGame()
    {
        SceneManager.LoadScene("Isles");
    }

    public void Start()
    {
        foreach (Transform child in loadContentRoot)
            Destroy(child.gameObject);

        _path = Application.persistentDataPath;

        string[] files = Directory.GetFiles(_path);

        foreach(string file in files)
        {
            LoadButton button = Instantiate(loadButton, loadContentRoot);
            button.SetButton(file);
        }
    }
}
