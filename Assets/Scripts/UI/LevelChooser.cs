using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChooser : MonoBehaviour
{
    [SerializeField] Button loadSavedLevel;
    [SerializeField] Button backButton;
    [SerializeField] GameObject noData;
    void Start()
    {
        loadSavedLevel.onClick.AddListener(() => SavingSystem.Instance.LoadSavedData());
        backButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }

    public void LoadDesiredLevel(string desiredLevel)
    {
        SavingSystem.Instance.LoadDesiredLevel(desiredLevel);
    }

    public void ShowNoDataText()
    {
        noData.SetActive(true);
    }
}
