using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooser : MonoBehaviour
{
    [SerializeField] Button loadSavedLevel;
    void Start()
    {
        loadSavedLevel.onClick.AddListener(() => SavingSystem.Instance.LoadSavedData());
    }

    public void LoadDesiredLevel(string desiredLevel)
    {
        SavingSystem.Instance.LoadDesiredLevel(desiredLevel);
    }
}
