using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class Sound
{
    [Header("References")]
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;

    [Header("Parameters")]
    [SerializeField] private float volume, pitch;
    [SerializeField] private string soundName;

    public float Volume { get => volume; set => volume = value; }
    public float Pitch { get => pitch; set => pitch = value; }
    public string SoundName { get => soundName; set => soundName = value; }
    public AudioSource Source { get => source; set => source = value; }
    public AudioClip Clip { get => clip; set => clip = value; }
}
