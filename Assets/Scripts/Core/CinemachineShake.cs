using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    CinemachineFreeLook cinemachineCam;

    CinemachineBasicMultiChannelPerlin topRig;
    CinemachineBasicMultiChannelPerlin middleRig;
    CinemachineBasicMultiChannelPerlin bottomRig;

    public static CinemachineShake instance;

    float shakerTimer = 0;
    float shakerTimerTotal = 0;
    float startingIntensity;
    private void Awake()
    {
        instance = this;

        cinemachineCam = GetComponent<CinemachineFreeLook>();

        topRig = cinemachineCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        middleRig = cinemachineCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        bottomRig = cinemachineCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (shakerTimer > 0)
        {
            shakerTimer -= 0.01f;

            topRig.m_AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakerTimer / shakerTimerTotal);
            middleRig.m_AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakerTimer / shakerTimerTotal);
            bottomRig.m_AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakerTimer / shakerTimerTotal);
            topRig.m_FrequencyGain = Mathf.Lerp(0f, startingIntensity, shakerTimer / shakerTimerTotal);
            middleRig.m_FrequencyGain = Mathf.Lerp(0f, startingIntensity, shakerTimer / shakerTimerTotal);
            bottomRig.m_FrequencyGain = Mathf.Lerp(0f, startingIntensity, shakerTimer / shakerTimerTotal);

        }
    }
    public void ShakeCamera(float timer, float intensity)
    {
        shakerTimerTotal = timer;
        shakerTimer = timer;

        startingIntensity = intensity;
        topRig.m_AmplitudeGain = intensity;
        middleRig.m_AmplitudeGain = intensity;
        bottomRig.m_AmplitudeGain = intensity;
        topRig.m_FrequencyGain = intensity;
        middleRig.m_FrequencyGain = intensity;
        bottomRig.m_FrequencyGain = intensity;
    }
}
