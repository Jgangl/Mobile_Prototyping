using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip backgroundMusicClip;

    [SerializeField] private bool enableMusic;
    [SerializeField] private bool enableSoundFX;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource squishSource;
    [SerializeField] private AudioSource launchSource;
    
    //public float[] pitchValues;
    public SquishAudioClip[] squishClips;
    public SquishAudioClip[] launchClips;
    
    public void PlayLaunchSound() 
    {
        if (!enableSoundFX)
            return;

        int randClip = Random.Range(0, launchClips.Length);

        launchSource.clip = launchClips[randClip].GetAudioClip();
        launchSource.pitch = launchClips[randClip].GetRandomPitch();
        launchSource.Stop();
        launchSource.Play();
    }
    
    public void PlaySquishSound() 
    {
        if (!enableSoundFX)
            return;

        int randClip = Random.Range(0, squishClips.Length);

        squishSource.clip = squishClips[randClip].GetAudioClip();
        squishSource.pitch = squishClips[randClip].GetRandomPitch();
        squishSource.Stop();
        squishSource.Play();
    }

    private void Start()
    {
        UpdateMusicStatus();
    }

    private void UpdateMusicStatus() 
    {
        if (enableMusic) 
        {
            backgroundSource.Play();
        }
        else
        {
            if (backgroundSource.isPlaying)
            {
                backgroundSource.Pause();
            }
        }
    }

    private void UpdateSoundFXStatus() 
    {
        if (!enableSoundFX) 
        {
            // Stop all Sound FX Sources
            squishSource.Stop();
        }
    }

    public void OnSoundFXChecked(bool isChecked) 
    {
        enableSoundFX = isChecked;
        UpdateSoundFXStatus();
    }

    public void OnMusicChecked(bool isChecked) 
    {
        enableMusic = isChecked;
        UpdateMusicStatus();
    }

    public void UpdateMusicEnabled(bool enabled) 
    {
        if (enableMusic != enabled) 
        {
            enableMusic = enabled;
            UpdateMusicStatus();
        }
    }

    public void UpdateSoundFXEnabled(bool enabled) 
    {
        if (enableSoundFX != enabled) 
        {
            enableSoundFX = enabled;
            UpdateSoundFXStatus();
        }
    }

    public void PlayOpenMenuSound()
    {
        if (!enableSoundFX) return;

        squishSource.clip = squishClips[0].GetAudioClip();
        squishSource.pitch = 0.8f;
        
        squishSource.Stop();
        squishSource.Play();
    }
    
    public void PlayCloseMenuSound()
    {
        if (!enableSoundFX) return;
        
        squishSource.clip = squishClips[0].GetAudioClip();
        squishSource.pitch = 0.5f;
        
        squishSource.Stop();
        squishSource.Play();
    }
}

[System.Serializable]
public class SquishAudioClip
{
    [SerializeField] AudioClip clip;
    [SerializeField] float minPitch;
    [SerializeField] float maxPitch;


    public AudioClip GetAudioClip()
    {
        return clip;
    }

    public float GetRandomPitch()
    {
        return Random.Range(minPitch, maxPitch);
    }
}