using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Manager : Singleton<Sound_Manager>
{
    public AudioClip backgroundMusicClip;

    [SerializeField]
    private bool enableMusic;
    [SerializeField]
    private bool enableSoundFX;

    public float[] pitchValues;
    public AudioClip[] squishClips;

    [SerializeField]
    private List<AudioSource> backgroundSources;

    [SerializeField]
    private AudioSource squishSource;

    // Start is called before the first frame update
    void Start()
    {
        squishSource = transform.Find("SquishSource_One").GetComponent<AudioSource>();

        AudioSource backgroundSource = transform.Find("BackgroundSource_One").GetComponent<AudioSource>();
        if (backgroundSource)
            backgroundSources.Add(backgroundSource);

        backgroundSource = transform.Find("BackgroundSource_Two").GetComponent<AudioSource>();
        if (backgroundSource)
            backgroundSources.Add(backgroundSource);

        PlayBackgroundMusicOne();
    }

    public void PlaySquishSound() {
        if (!enableSoundFX)
            return;

        squishSource.clip = squishClips[Random.Range(0, squishClips.Length)];
        squishSource.pitch = pitchValues[Random.Range(0, pitchValues.Length)];
        squishSource.Stop();
        squishSource.Play();
    }

    public void PlayBackgroundMusicOne() {
        if (!enableMusic)
            return;

        AudioSource backgroundSource = null;
        // Find first available background source
        foreach(AudioSource source in backgroundSources) {
            if (!source.isPlaying)
                backgroundSource = source;
        }

        if (backgroundSource != null) {
            // Play music clip
            backgroundSource.clip = backgroundMusicClip;
            backgroundSource.Play();
        }
    }

    public void UpdateMusicStatus() {
        if (!enableMusic) {
            foreach(AudioSource source in backgroundSources) {
                source.Stop();
            }
        }
    }

    public void UpdateSoundFXStatus() {
        if (!enableSoundFX) {
            // Stop all Sound FX Sources
            squishSource.Stop();
        }
    }

    public void ChangeSourceVolume(AudioSource source, float volume) {
        source.volume = volume;
    }

    public void OnSoundFXChecked(bool isChecked) {
        //Debug.Log("Sound FX Checked: " + isChecked);

        enableSoundFX = isChecked;
        UpdateSoundFXStatus();
    }

    public void OnMusicChecked(bool isChecked) {
        //Debug.Log("Music Checked: " + isChecked);

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