using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LongIconBarTool : MonoBehaviour
{
    public Sprite[] AllShowIcon;//When the array contains multiple images, when the value changes, the UI will display the image corresponding to the array according to the ratio of the current value to the maximum value(Reference Prefabs: OneLongIconBar_ManyStateIcon)
    public Color[] AllShowColor;//When the array is set with multiple colors and the script value changes, the UI will change the color of the UIBar to the corresponding color in the array according to the ratio of the current value to the maximum value
    public Image BackUI;//The background bar Image object for UIBar
    public Image BarUI;//The main Image object for UIBar
    public Image BarUILerp;//Image object of UIBar for delayed display, sandwiched between the background Bar and the main Bar(Can be null)
    public Text BarTui;//Text hints for numeric values, text objects

    public float ForwardBarLerpSpeed =3.5f,LerpBarSpeed = 2;
    //ForwardBarLerpSpeed:The numerical synchronization delay speed of the main Bar. When the value is 0, it indicates no delay and instantaneous synchronization
    //LerpBarSpeed:The delay synchronization speed of BarUILerp
    public bool IsUpdateRefreshUI;
    //Refresh the display state of the UI in real time.
    //If this option is enabled, there is no need to call any refresh function, only need todirectly change NowTotalValue value to see the change of the UI in real time, but the performance cost is high
    //If not, you need to call the script's SetNowValue () or SetMaxValue () function in your code to update the UI value
    
    public float NowTotalValue=100;
    public float MaxTotalValue =100;
    public UnityEvent OnBeReduceValue, OnBeReduceTo0Value, OnBeAddValue, OnBeAddToMaxValue;
    //OnBeReduceValue:An event is triggered instantly when the value is reduced
    //OnBeReduceTo0Value:Triggered when the value is reduced to 0
    //OnBeAddValue:An event is triggered instantly when the value is increased
    //OnBeAddToMaxValue:The event that fires when the value is increased to the maximumvalue
    int NowShowColorID;
    float NowMoveUIValue;

    private void Start()
    {
        SetNowValue(NowTotalValue);
        SetMaxValue(MaxTotalValue);
        RefreshShowSpriteBar();
    }
    // Update is called once per frame
    void Update()
    {

        if (BarUILerp) {
            if (LerpBarSpeed > 0)
            {
                BarUILerp.fillAmount = Mathf.MoveTowards(BarUILerp.fillAmount, BarUI.fillAmount, Time.deltaTime * LerpBarSpeed);
            }
            else {
                BarUILerp.fillAmount = BarUI.fillAmount;
            }
        }
        if (ForwardBarLerpSpeed  > 0)
        {
            NowMoveUIValue = Mathf.MoveTowards(NowMoveUIValue, NowTotalValue, Time.deltaTime * ForwardBarLerpSpeed );
        }
        else
        {
            NowMoveUIValue = NowTotalValue;
        }
        BarUI.fillAmount = NowMoveUIValue / MaxTotalValue;
        if (AllShowColor.Length > 0 && NowShowColorID < AllShowColor.Length)
        {
            BarUI.color = Vector4.Lerp(BarUI.color, AllShowColor[NowShowColorID], Time.deltaTime * 5);
        }
        if (BarTui) { BarTui.text = NowMoveUIValue.ToString("f0") + "/" + MaxTotalValue; }
        if (IsUpdateRefreshUI)
        {
            SetNowValue(NowTotalValue);
        }
    }

    public int GetShowNum(int length) {
        if (length<=0) { return 0; }
        float onev = MaxTotalValue/ (float)(length);
       // print(onev);
        int nowv = (int)(length-(NowTotalValue / onev));
        // print(nowv);
        if (nowv<0) { nowv = 0; }
        return nowv;
    }

    public void SetNowValue(float v)
    {
        if (v < 0) { v = 0; }
        if (v > MaxTotalValue) { v = MaxTotalValue; }
        if (v != NowTotalValue) { OnValueChange(NowTotalValue, v); }
        NowTotalValue = v;
        if (LerpBarSpeed>0) { NowMoveUIValue = NowTotalValue; }
        RefreshShowSpriteBar();
    }
    public void SetMaxValue(float v)
    {
        MaxTotalValue = v;
        if (MaxTotalValue <= 0) { MaxTotalValue = 1; }
        RefreshShowSpriteBar();
    }
    public void RefreshShowSpriteBar()
    {
        if (AllShowIcon.Length > 0)
        {
            int id = GetShowNum(AllShowIcon.Length);
            if (id< AllShowIcon.Length) {
                BackUI.sprite = AllShowIcon[id];
                BarUI.sprite = AllShowIcon[id];
                BarUILerp.sprite = AllShowIcon[id];
            }
           
        }
        if (AllShowColor.Length > 0)
        {
            NowShowColorID = GetShowNum(AllShowColor.Length);
        }
        if (Application.isPlaying==false) {
            QuiteRefreshUIData();
        }
    }

    public void QuiteRefreshUIData() {
        if (BarUILerp) { BarUILerp.fillAmount = BarUI.fillAmount; }
        NowMoveUIValue = NowTotalValue;
        BarUI.fillAmount = NowMoveUIValue / MaxTotalValue;
        if (AllShowColor.Length > 0 && NowShowColorID < AllShowColor.Length)
        {
            BarUI.color = AllShowColor[NowShowColorID];
        }
        if (BarTui) { BarTui.text = NowMoveUIValue.ToString("f0") + "/" + MaxTotalValue; }
    }

    void OnValueChange(float lastdata, float newdata)
    {
        if (newdata > lastdata)
        {
            OnBeAddValue.Invoke();
            if (newdata >= MaxTotalValue)
            {
                OnBeAddToMaxValue.Invoke();
            }
        }
        if (newdata < lastdata)
        {
            NowMoveUIValue = newdata;
            OnBeReduceValue.Invoke();
        }
        if (newdata <= 0)
        {
            OnBeReduceTo0Value.Invoke();
        }

    }
}
