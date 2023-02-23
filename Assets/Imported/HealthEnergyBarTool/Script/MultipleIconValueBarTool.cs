using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MultipleIconValueBarTool : MonoBehaviour
{
    public GameObject ValueIconStyle;//The style of each icon representing a numeric paragraph
    public bool RefreshUIInStart = true, UpdateRefreshUI;
    //RefreshUIInStart:When the game is playing, refresh the UI state in the start function
    //UpdateRefreshUI:Refresh the display state of the UI in real time.
    //If this option is enabled, there is no need to call any refresh function, only need todirectly change NowTotalValue value to see the change of the UI in real time, but the performance cost is high
   // If not, you need to call the script's SetNowValue () or SetMaxValue () function in your code to update the UI value,you also have to call RefreshUI() to refresh the UI's effects
    public bool BackIconAlwaysShow = true;//The background image of each icon is always displayed
    public float NowTotalValue = 12, MaxTotalValue = 12;
    public float OneHealthIconMaxValue = 4;//Evaluate the value that each paragraph represents
    Transform AllUIParent;
    List<OneShowIconValueSet> NowAllBar = new List<OneShowIconValueSet>();
    private void Awake()
    {
        SetInit();
    }
    private void Start()
    {
        if (RefreshUIInStart) { RefreshUI(); }
    }
    private void Update()
    {
        if (UpdateRefreshUI)
        {
            RefreshUI();
        }
    }
    public void SetInit()
    {
        if (transform.childCount > 0)
        {
            AllUIParent = transform.GetChild(0);
        }
        else
        {
            AllUIParent = transform;
        }
        NowAllBar.Clear();
        DestroyChildren(AllUIParent);
        if (OneHealthIconMaxValue <= 0) { OneHealthIconMaxValue = 1; }

    }
    public void SetNowValue(float v)
    {
        NowTotalValue = v;
        if (NowTotalValue > MaxTotalValue) { NowTotalValue = MaxTotalValue; }
        if (NowTotalValue < 0) { NowTotalValue = 0; }
        if (OneHealthIconMaxValue <= 0) { OneHealthIconMaxValue = 1; }
    }
    public void SetMaxValue(float v)
    {
        MaxTotalValue = v;
        if (MaxTotalValue <= 0) { MaxTotalValue = OneHealthIconMaxValue; }
        if (OneHealthIconMaxValue <= 0) { OneHealthIconMaxValue = 1; }
    }
    public void RefreshUI()
    {
        if (MaxTotalValue < 0) { MaxTotalValue = 1; }
        if (NowTotalValue > MaxTotalValue) { NowTotalValue = MaxTotalValue; }
        if (NowTotalValue < 0) { NowTotalValue = 0; }
        if (OneHealthIconMaxValue <= 0) { OneHealthIconMaxValue = 1; }
        SetMaxValue();
        SetNowValue();
    }

    void SetMaxValue()
    {
        if (NowTotalValue > MaxTotalValue || MaxTotalValue < 0) { return; }
        if (AllUIParent == null) { return; }
        float[] alldata = GetAllSetIconData(MaxTotalValue, true);
        if (NowAllBar.Count != alldata.Length)
        {
            NowAllBar.Clear();
            DestroyChildren(AllUIParent);

            for (int i = 0; i < alldata.Length; i++)
            {
                GameObject nowg = null;
                if (Application.isEditor)
                {

#if UNITY_EDITOR
                    nowg = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(ValueIconStyle);
#endif
                }
                else
                {
                    nowg = (GameObject)Instantiate(ValueIconStyle);
                }
                nowg.transform.SetParent(AllUIParent);
                nowg.transform.localPosition = Vector3.zero;
                nowg.transform.localEulerAngles = Vector3.zero;
                nowg.transform.localScale = Vector3.one;
                OneShowIconValueSet nowdata = nowg.GetComponent<OneShowIconValueSet>();
                nowdata.SetInitData(OneHealthIconMaxValue);

                NowAllBar.Add(nowdata);
            }
        }
    }
    void SetNowValue()
    {
        if (NowTotalValue > MaxTotalValue || NowTotalValue < 0) { return; }
        if (NowAllBar.Count > 0)
        {
            float[] alldata = GetAllSetIconData(NowTotalValue, false);
            float numout = MaxTotalValue % OneHealthIconMaxValue;
            if (alldata.Length <= NowAllBar.Count)
            {
                for (int i = 0; i < NowAllBar.Count; i++)
                {
                    if (i < alldata.Length)
                    {
                        NowAllBar[i].SetNowValue(alldata[i]);
                        NowAllBar[i].SetRealMaxValue(OneHealthIconMaxValue);
                    }
                    else
                    {
                        NowAllBar[i].SetNowValue(0);
                        if (BackIconAlwaysShow)
                        {
                            NowAllBar[i].SetRealMaxValue(OneHealthIconMaxValue);
                        }
                        else
                        {
                            NowAllBar[i].SetRealMaxValue(0);
                        }
                    }


                }
            }
        }
    }

    public float[] GetAllSetIconData(float v, bool justgetnum)
    {
        int num = 0;
        float numout = 0;
        if (v >= OneHealthIconMaxValue)
        {
            num = (int)(v / OneHealthIconMaxValue);
            numout = v % OneHealthIconMaxValue;
        }
        else
        {
            num = 0;
            numout = v % OneHealthIconMaxValue;
        }

        if (numout > 0) { num += 1; }
        float[] AllData = new float[num];
        if (!justgetnum)
        {
            for (int i = 0; i < num; i++)
            {
                if (numout <= 0)
                {
                    AllData[i] = OneHealthIconMaxValue;
                }
                else
                {
                    if (i == num - 1)
                    {
                        AllData[i] = numout;
                    }
                    else
                    {
                        AllData[i] = OneHealthIconMaxValue;
                    }
                }
            }
        }
        return AllData;
    }
    void DestroyChildren(Transform parent)
    {
        bool isPlaying = Application.isPlaying;

        while (parent.childCount != 0)
        {
            Transform child = parent.GetChild(0);

            if (isPlaying)
            {
                child.parent = null;
                Destroy(child.gameObject);
            }
            else DestroyImmediate(child.gameObject);
        }
    }
}
