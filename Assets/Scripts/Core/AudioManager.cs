using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicSource, environmentSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //environment audio default settings
        environmentSource.volume = 0.2f;
        environmentSource.loop = true;

        //music audio default settings
        musicSource.volume = 0.7f;
        musicSource.loop = true;
    }

    public void PlaySoundEffect(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void StopSoundEffect(AudioSource source)
    {
        source.Stop();
    }

    public void PlayEnvironmentSound(AudioClip clip)
    {
        environmentSource.clip = clip;
        environmentSource.Play();
    }

    public void StopEnvironmentSound()
    {
        environmentSource.Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
