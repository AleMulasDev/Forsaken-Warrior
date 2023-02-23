using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The code in this example increases the zoom of an image by a multiple of an instant, and then gradients back to the original zoom at a certain speed
public class TweenIconScaleDemo : MonoBehaviour
{
    public Transform TweenScaleObj;
    public float Speed = 3;
    float EndSize;
    bool Tween;
    private void Awake()
    {
        EndSize = TweenScaleObj.localScale.x;
    }
    private void Update()
    {
        if (Tween)
        {
            if (TweenScaleObj.localScale.x != EndSize)
            {
                TweenScaleObj.localScale= Vector3.MoveTowards(TweenScaleObj.localScale, Vector3.one* EndSize, Time.deltaTime * Speed);
            }
            else
            {
                Tween = false;
            }
        }
    }
    public void SetSelfScaleTween(float BigOffet=0.2f)
    {
        TweenScaleObj.localScale = Vector3.one * EndSize;
        TweenScaleObj.localScale += Vector3.one * EndSize* BigOffet;
        Tween = true;
    }
}
