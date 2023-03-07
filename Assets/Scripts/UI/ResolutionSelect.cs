using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSelect : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private List<string> _optionsList = new List<string>();
    protected void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();

        _dropdown.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution res in resolutions)
        {
            _optionsList.Add(res.width + "x" + res.height + " @ " + res.refreshRate + "Hz");
        }

        _dropdown.AddOptions(_optionsList);

        Resolution currentResolution = Screen.currentResolution;

        _dropdown.value = _optionsList.FindIndex(x => x.Contains(currentResolution.width + "x" + currentResolution.height + " @ " + currentResolution.refreshRate + "Hz"));
    }

    //protected void OnEnable()
    //{
    //    if (this.m_SelectField == null)
    //        return;

    //    this.m_SelectField.onChange.AddListener(OnSelectedOption);
    //}

    //protected void OnDisable()
    //{
    //    if (this.m_SelectField == null)
    //        return;

    //    this.m_SelectField.onChange.RemoveListener(OnSelectedOption);
    //}

    protected void OnSelectedOption(int index, string option)
    {
        Resolution res = Screen.resolutions[index];

        if (res.Equals(Screen.currentResolution))
            return;

        Screen.SetResolution(res.width, res.height, true, res.refreshRate);
    }
}
