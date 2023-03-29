using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource gameMusicSource, menuMusicSource;

    private AudioClip _menuMusic;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _menuMusic = Resources.Load<AudioClip>("MenuMusicAudioClip");
    }

    public void PlaySoundEffect(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void StopSoundEffect(AudioSource source)
    {
        source.Stop();
    }

    public void PlayMenuMusic()
    {
        menuMusicSource.clip = _menuMusic;
        menuMusicSource.Play();
    }

    public void StopMenuMusic()
    {
        menuMusicSource.Stop();
    }

    public void PlayGameMusic(AudioClip clip)
    {
        gameMusicSource.clip = clip;
        gameMusicSource.Play();
    }

    public void PauseGameMusic()
    {
        gameMusicSource.Pause();
    }
}
