using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicSource, effectSource, environmentSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        //effect and environment audio default settings
        effectSource.volume = environmentSource.volume = 0.2f;
        effectSource.loop = false;
        environmentSource.loop = true;
        
        //music audio default settings
        musicSource.volume = 0.7f;
        musicSource.loop = true;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void PlaySoundEffectAtPoint(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
    
    public void StopSoundEffect()
    {
        effectSource.Stop();
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
