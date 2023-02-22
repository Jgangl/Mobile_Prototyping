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
    [SerializeField] private AudioSource successSource;
    
    public SquishAudioClip[] squishClips;
    public SquishAudioClip[] launchClips;
    public SquishAudioClip[] successClips;
    public AudioClip   startClip;
    
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

    public void PlayWinSound()
    {
        PlaySquishSound();
        
        if (!enableSoundFX)
            return;

        int randClip = Random.Range(0, successClips.Length);

        successSource.clip = successClips[randClip].GetAudioClip();
        successSource.pitch = successClips[randClip].GetRandomPitch();
        successSource.Stop();
        successSource.Play();
    }

    public void PlayStartSound()
    {
        AudioSource.PlayClipAtPoint(startClip, Vector3.zero);
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

        squishSource.clip = squishClips[2].GetAudioClip();
        squishSource.pitch = 1.1f;
        
        squishSource.Stop();
        squishSource.Play();
    }
    
    public void PlayCloseMenuSound()
    {
        if (!enableSoundFX) return;
        
        squishSource.clip = squishClips[2].GetAudioClip();
        squishSource.pitch = 0.9f;
        
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