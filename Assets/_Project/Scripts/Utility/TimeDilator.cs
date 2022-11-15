using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDilator
{
    public static Coroutine SlowTime(MonoBehaviour runner, float speed, float time)
    {
        return runner.StartCoroutine(SlowTimeForDuration(speed, time));
    }
    
    public static IEnumerator SlowTimeCoroutine(MonoBehaviour runner, float speed, float time)
    {
        yield return runner.StartCoroutine(SlowTimeForDuration(speed, time));
    }

    private static IEnumerator SlowTimeForDuration(float speed, float time)
    {
        SlowTimeIndefinitely(speed);

        yield return new WaitForSecondsRealtime(time);
        
        ResumeNormalTime();
    }

    public static void SlowTimeIndefinitely(float speed)
    {
        Time.timeScale = Mathf.Clamp01(speed);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public static void ResumeNormalTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
