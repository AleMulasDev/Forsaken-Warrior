using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dataName;

    private PlayerData _data;
    private Button _button;

    public void SetButton(string dataName)
    {
        _data = new PlayerData();

        _data = JsonUtility.FromJson<PlayerData>(File.ReadAllText(dataName));

        this.dataName.text = _data.playerName;

        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => SavingSystem.Instance.SetLoadedData(_data));
    }
}
