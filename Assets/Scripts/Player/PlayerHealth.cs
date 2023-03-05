using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] MultipleIconValueBarTool _multipleIconValueBarTool;

    // Start is called before the first frame update

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _multipleIconValueBarTool.SetNowValue(_currentHealth);
    }
    public void AddHeart(int heart)
    {
        _currentHealth = _maxHealth += (heart * 8);
        _multipleIconValueBarTool.SetMaxValue(_maxHealth);
        _multipleIconValueBarTool.SetNowValue(_maxHealth);
        _multipleIconValueBarTool.RefreshUI();
    }

    public void AddHealth(int health)
    {
        _currentHealth += health;
        _multipleIconValueBarTool.SetNowValue(_currentHealth);
        _multipleIconValueBarTool.RefreshUI();
    }
}
