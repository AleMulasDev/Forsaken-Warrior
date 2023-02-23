using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class OneShowIconValueSet : MonoBehaviour
{

    public Image BackImage;//The background image object of the icon
    public Image ForwardImage;//The main image object of the icon
    public UnityEvent OnBeReduceValue, OnBeReduceTo0Value, OnBeAddValue, OnBeAddToMaxValue;
    //OnBeReduceValue:An event is triggered instantly when the value is reduced
    //OnBeReduceTo0Value:Triggered when the value is reduced to 0
    //OnBeAddValue:An event is triggered instantly when the value is increased
    //OnBeAddToMaxValue:The event that fires when the value is increased to the maximumvalue
    Transform AllUI;
    float NowValue;
    float MaxValue;
    float RealMaxValue;
    float InitSize;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Initialize the
    public void SetInitData(float maxvalue)
    {
        if (transform.childCount > 0)
        {
            AllUI = transform.GetChild(0);
        }
        else
        {
            AllUI = transform;
        }
        InitSize = AllUI.transform.localScale.x;
        // transform.DOPunchScale(Vector3.one * 1.1f, 0.3f);
        MaxValue = maxvalue;
        SetShowUIData();
    }
    public void SetRealMaxValue(float v)
    {
        RealMaxValue = v;
        SetShowUIData();
    }
    public void SetNowValue(float v)
    {
        if (v > MaxValue) { v = MaxValue; }
        if (v < 0) { v = 0; }
        if (v != NowValue) { OnValueChange(NowValue, v); }
        NowValue = v;
        SetShowUIData();
    }
    void OnValueChange(float lastdata, float newdata)
    {
        AllUI.transform.localPosition = Vector3.one;
        AllUI.transform.localScale = Vector3.one * InitSize;
        if (newdata > lastdata)
        {
            // AllUI.transform.DOPause();
            //ValueBar.DOColor(Color.green,0);
            // ValueBar.DOColor(ValueBarColor, 0.5f);
            OnBeAddValue.Invoke();
            if (newdata == MaxValue)
            {
                OnBeAddToMaxValue.Invoke();
                // AllUI.transform.DOPunchScale(Vector3.one * 1.2f, 0.4f);
            }
        }
        if (newdata < lastdata)
        {
            OnBeReduceValue.Invoke();
            // AllUI.transform.DOPause();
            // AllUI.transform.DOShakePosition(1f, 5);
        }
        if (newdata <= 0)
        {
            OnBeReduceTo0Value.Invoke();
        }

    }

    void SetShowUIData()
    {
        ForwardImage.fillAmount = NowValue / MaxValue;
        if (BackImage) { BackImage.fillAmount = RealMaxValue / MaxValue; }
    }
}
