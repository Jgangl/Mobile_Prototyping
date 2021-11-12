using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Manager : Singleton<Sound_Manager>
{
    public AudioClip squishClip;

    AudioSource audioSource;

    [SerializeField]
    private bool enableAllSounds;
    [SerializeField]
    private bool enableMusic;
    [SerializeField]
    private bool enableSoundEffects;

    public float[] pitchValues;
    public AudioClip[] squishClips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySquishSound() {
        audioSource.clip = squishClips[Random.Range(0, squishClips.Length)];
        //audioSource.pitch
        audioSource.pitch = pitchValues[Random.Range(0, pitchValues.Length)];
        audioSource.Stop();
        audioSource.Play();
    }
}
