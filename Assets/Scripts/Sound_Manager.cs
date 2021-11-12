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

    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

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
        audioSource.clip = squishClip;
        //audioSource.pitch
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Stop();
        audioSource.Play();
    }
}
