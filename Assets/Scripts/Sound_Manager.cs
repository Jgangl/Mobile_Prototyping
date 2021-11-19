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
    private AudioSource[] backgroundSources;

    [SerializeField]
    private AudioSource squishSource;

    public override void Awake() {
        base.Awake();
        if (keepAlive) {
            // Don't destroy child audio sources
            for(int i = 0; i < transform.childCount; i++) {
                DontDestroyOnLoad(transform.GetChild(i).gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
