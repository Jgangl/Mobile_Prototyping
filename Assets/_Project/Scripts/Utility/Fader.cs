using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : Singleton<Fader>
{
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeOut(float time)
    {
        StartCoroutine(FadeRoutine(0f, 1f, time));
    }
    
    public IEnumerator FadeOutCoroutine(float time)
    {
        return FadeRoutine(0f, 1f, time);
    }

    public void FadeIn(float time)
    {
        StartCoroutine(FadeRoutine(1f, 0f, time));
    }
    
    public IEnumerator FadeInCoroutine(float time)
    {
        return FadeRoutine(1f, 0f, time);
    }
    
    public Coroutine Fade(MonoBehaviour runner, float start, float target, float time)
    {
        return runner.StartCoroutine(FadeRoutine(start, target, time));
    }

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
}
