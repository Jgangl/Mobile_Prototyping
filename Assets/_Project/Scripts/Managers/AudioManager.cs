using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip backgroundMusicClip;

    [SerializeField] private bool enableMusic;
    [SerializeField] private bool enableSoundFX;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource squishSource;
    
    public float[] pitchValues;
    public AudioClip[] squishClips;

    public void PlaySquishSound() {
        if (!enableSoundFX)
            return;

        squishSource.clip = squishClips[Random.Range(0, squishClips.Length)];
        squishSource.pitch = pitchValues[Random.Range(0, pitchValues.Length)];
        squishSource.Stop();
        squishSource.Play();
    }

    private void UpdateMusicStatus() {
        if (enableMusic) {
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

    private void UpdateSoundFXStatus() {
        if (enableSoundFX) {
            // Stop all Sound FX Sources
            squishSource.Play();
        }
        else
        {
            squishSource.Stop();
        }
    }

    public void OnSoundFXChecked(bool isChecked) {
        enableSoundFX = isChecked;
        UpdateSoundFXStatus();
    }

    public void OnMusicChecked(bool isChecked) {
        enableMusic = isChecked;
        UpdateMusicStatus();
    }

    public void UpdateMusicEnabled(bool enabled) {
        if (enableMusic != enabled) {
            enableMusic = enabled;
            UpdateMusicStatus();
        }
    }

    public void UpdateSoundFXEnabled(bool enabled) {
        if (enableSoundFX != enabled) {
            enableSoundFX = enabled;
            UpdateSoundFXStatus();
        }
    }
}