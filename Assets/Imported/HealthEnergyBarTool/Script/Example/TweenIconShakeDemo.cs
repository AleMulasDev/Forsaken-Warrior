using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This example code will make an object shiver for a while
public class TweenIconShakeDemo : MonoBehaviour
{

   
    public Transform ShakeObj;
    public float swing = 0.1f;
    float TweenTime;
    Vector3 initpos;
    // Use this for initialization
    void Awake()
    {

        initpos = ShakeObj.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (TweenTime > 0)
        {
            Vector3 pos = initpos + Random.insideUnitSphere * swing;
            ShakeObj.localPosition = pos;
            TweenTime -= Time.deltaTime;
            if (TweenTime<=0) { ShakeObj.localPosition = initpos; }
        }
        
      
    }

    public void SetShake(float time) {
        ShakeObj.localPosition = initpos;
        TweenTime = time;
    }
}
