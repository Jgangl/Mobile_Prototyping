using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Manager : Singleton<Sound_Manager>
{
    public AudioClip backgroundMusicClip;

    public AudioClip squishClip;

    [SerializeField]
    private bool enableAllSounds;
    [SerializeField]
    private bool enableMusic;
    [SerializeField]
    private bool enableSoundEffects;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySquishSound() {
        squishSource.clip = squishClips[Random.Range(0, squishClips.Length)];
        squishSource.pitch = pitchValues[Random.Range(0, pitchValues.Length)];
        squishSource.Stop();
        squishSource.Play();
    }

    public void PlayBackgroundMusicOne() {
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
}
