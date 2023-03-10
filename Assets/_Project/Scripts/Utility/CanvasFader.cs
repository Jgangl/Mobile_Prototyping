using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CanvasFader : Singleton<CanvasFader>
{
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void FadeIn(float time)
    {
        StopAllCoroutines();
        //StartCoroutine(FadeRoutine(canvasGroup.alpha, 0f, time));
        StartCoroutine(FadeInCoroutine(time));
    }

    public void FadeOut(float time)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(time));
        //StartCoroutine(FadeRoutine(canvasGroup.alpha, 1f, time));
    }
    
    public void FadeOutInstant()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 1f;
    }
    
    public void FadeInInstant()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
    }
    
    public void FadeTo(float target, float time)
    {
        StopAllCoroutines();
        StartCoroutine(DOFadeRoutine( target, time));
    }

    public IEnumerator FadeInCoroutine(float time)
    {
        return DOFadeRoutine(0f, time);
        //return FadeRoutine(1f, 0f, time);
    }
    
    public IEnumerator FadeOutCoroutine(float time)
    {
        return DOFadeRoutine(1f, time);
        //return FadeRoutine(0f, 1f, time);
    }
    
    public IEnumerator FadeToCoroutine(float target, float time)
    {
        return DOFadeRoutine(target, time);
        //return FadeRoutine(0f, 1f, time);
    }
    /*
    public Coroutine Fade(MonoBehaviour runner, float start, float target, float time)
    {
        return runner.StartCoroutine(FadeRoutine(start, target, time));
    }
    */
    private IEnumerator FadeRoutine(float start, float target, float time)
    {
        float startValue = start;

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / time)
        {
            float newValue = Mathf.Lerp(startValue, target, t);
            canvasGroup.alpha = newValue;
            yield return null;
        }

        canvasGroup.alpha = target;
    }
    
    private IEnumerator DOFadeRoutine(float target, float time)
    {
        // Fade while ignoring timescale
        Tween tween = canvasGroup.DOFade(target, time).SetUpdate(true);
        yield return tween.WaitForCompletion();
    }
    
    private IEnumerator DOFadeVolumeRoutine(float target, float time, AudioSource targetAudioSource)
    {
        // Fade volume while ignoring timescale
        Tween tween = targetAudioSource.DOFade(target, time).SetUpdate(true);
        yield return tween.WaitForCompletion();
    }
    /*
    private IEnumerator FadeRoutineTest(float start, float target, float time, ref float val)
    {
        float startValue = start;

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / time)
        {
            float newValue = Mathf.Lerp(startValue, target, t);
            canvasGroup.alpha = newValue;
            yield return null;
        }

        canvasGroup.alpha = target;
    }
    
    public static IEnumerator myCoroutine(Action<float> myVariableResult)
    {
        float time = 0;
        float timeLimit = 1;
        while (time < timeLimit)
        {
            // Do calculations
            myVariableResult (newValue);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void FadeOutTest()
    {
        
    }
    */
/*
    public void TestFade(ref float valToChange)
    {
        StartCoroutine(myCoroutine((value) => { valToChange = value;} ))
    }
    */
}