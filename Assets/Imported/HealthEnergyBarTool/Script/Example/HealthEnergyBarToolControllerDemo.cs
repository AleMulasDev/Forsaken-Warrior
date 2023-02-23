using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnergyBarToolControllerDemo : MonoBehaviour
{
    public MultipleIconValueBarTool HealthBar1, HealthBar2, HealthBar3;
    public LongIconBarTool EnergyBar1, EnergyBar2;

    public void ChangeMaxValue(float v) {
        HealthBar1.SetMaxValue(HealthBar1.MaxTotalValue+v);
        HealthBar2.SetMaxValue(HealthBar2.MaxTotalValue + v);
        HealthBar3.SetMaxValue(HealthBar3.MaxTotalValue + v);
        EnergyBar1.SetMaxValue(EnergyBar1.MaxTotalValue+v*10);
        EnergyBar2.SetMaxValue(EnergyBar2.MaxTotalValue + v * 10);
        HealthBar1.RefreshUI();
        HealthBar2.RefreshUI();
        HealthBar3.RefreshUI();
    }


    public void ChangeNowValue(float v) {
        HealthBar1.SetNowValue(HealthBar1.NowTotalValue + v);
        HealthBar2.SetNowValue(HealthBar2.NowTotalValue + v);
        HealthBar3.SetNowValue(HealthBar3.NowTotalValue + v);
        EnergyBar1.SetNowValue(EnergyBar1.NowTotalValue + v*10);
        EnergyBar2.SetNowValue(EnergyBar2.NowTotalValue + v * 10);
        HealthBar1.RefreshUI();
        HealthBar2.RefreshUI();
        HealthBar3.RefreshUI();
    }
    public void SetSilderBarValue(float v) {
        HealthBar1.SetNowValue(HealthBar1.MaxTotalValue*v);
        HealthBar2.SetNowValue(HealthBar2.MaxTotalValue * v);
        HealthBar3.SetNowValue(HealthBar3.MaxTotalValue * v);
        EnergyBar1.SetNowValue(EnergyBar1.MaxTotalValue * v);
        EnergyBar2.SetNowValue(EnergyBar2.MaxTotalValue * v);
        HealthBar1.RefreshUI();
        HealthBar2.RefreshUI();
        HealthBar3.RefreshUI();
    }
}
