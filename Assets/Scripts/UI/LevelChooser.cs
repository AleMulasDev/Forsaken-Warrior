using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChooser : MonoBehaviour
{
    [SerializeField] private Button loadSavedLevel;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI noData;
    [SerializeField] private TextMeshProUGUI recordText;
    void Start()
    {
        loadSavedLevel.onClick.AddListener(() => SavingSystem.Instance.LoadSavedData());
        backButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));

        recordText.text = "Total time: " + GameManager.Instance.GetTime(SavingSystem.Instance.GetPlayerData().totalTime) + "\n\nTotal keys: " + SavingSystem.Instance.GetPlayerData().keys + "/3" + "\n\nTotal score: " + SavingSystem.Instance.GetPlayerData().totalScore;
    }

    public void LoadDesiredLevel(string desiredLevel)
    {
        SavingSystem.Instance.LoadDesiredLevel(desiredLevel);
    }

    public void ShowWarning(string text)
    {
        noData.text = text;
    }
}
