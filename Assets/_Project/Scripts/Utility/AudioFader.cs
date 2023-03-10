using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioFader : Singleton<AudioFader>
{
    public void FadeIn(AudioSource targetAudioSource, float time)
    {
        StopAllCoroutines();
        //StartCoroutine(FadeRoutine(canvasGroup.alpha, 0f, time));
        StartCoroutine(FadeInCoroutine(targetAudioSource, time));
    }

    public void FadeOut(AudioSource targetAudioSource, float time)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(targetAudioSource, time));
    }
    
    public void FadeOutInstant(AudioSource targetAudioSource)
    {
        StopAllCoroutines();
        targetAudioSource.volume = 0f;
    }
    
    public void FadeInInstant(AudioSource targetAudioSource)
    {
        StopAllCoroutines();
        targetAudioSource.volume = 0.25f;
    }

    public IEnumerator FadeInCoroutine(AudioSource targetAudioSource, float time)
    {
        return DOFadeVolumeRoutine(targetAudioSource, 0.25f, time);
    }
    
    public IEnumerator FadeOutCoroutine(AudioSource targetAudioSource, float time)
    {
        return DOFadeVolumeRoutine(targetAudioSource, 0f, time);
    }

    private IEnumerator DOFadeVolumeRoutine(AudioSource targetAudioSource, float target, float time)
    {
        // Fade volume while ignoring timescale
        Tween tween = targetAudioSource.DOFade(target, time).SetUpdate(true);
        yield return tween.WaitForCompletion();
    }
}