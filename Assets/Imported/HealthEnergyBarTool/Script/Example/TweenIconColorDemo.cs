using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//This example code will change the color of an image to a certain color instantly, and then change it to the original color at a certain speed
public class TweenIconColorDemo : MonoBehaviour
{
    public Color ColorStart;//Color set the moment the animation starts
    public Image IconUI;//The Image that needs to change color
    public float Speed=3;//Recovery rate
    Color ColorEnd;
    bool Tween;
    private void Awake()
    {
        ColorEnd = IconUI.color;
    }
    private void Update()
    {
        if (Tween)
        {
            if (IconUI.color != ColorEnd)
            {
                IconUI.color = Vector4.MoveTowards(IconUI.color, ColorEnd, Time.deltaTime * Speed);
            }
            else
            {
                Tween = false;
            }
        }
    }
    public void SetSelfColorTween()
    {
       
        IconUI.color = ColorStart;
        Tween = true;
    }
}
