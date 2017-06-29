using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;


public class AudioManager : Singleton<AudioManager> {

    AudioSource[] audios;
    public AudioSource select_veranda;
    public AudioSource welcome;
    public AudioSource scan;
    public AudioSource anchors;
    public bool muted;

    private AudioSource current;

    // Use this for initialization
    void Start () {
    }
	
    public void InitSounds()
    {
        audios = GetComponents<AudioSource>();
        select_veranda = audios[1];
        scan = audios[0];
        welcome = audios[2];
        anchors = audios[3];
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if(!muted)
        {
            StopPlayingSound();

            current = soundToPlay;
            current.Play();
        }
    }

    public void StopPlayingSound()
    {
        if (current != null && current.isPlaying)
        {
            current.Stop();
        }
    }
}
