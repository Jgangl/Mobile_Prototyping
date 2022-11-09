using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDilator
{
    public static Coroutine SlowTime(MonoBehaviour runner, float speed, float time)
    {
        return runner.StartCoroutine(StopTime(speed, time));
    }
    
    public static IEnumerator SlowTimeCoroutine(MonoBehaviour runner, float speed, float time)
    {
        yield return runner.StartCoroutine(StopTime(speed, time));
    }

    private static IEnumerator StopTime(float speed, float time)
    {
        Time.timeScale = Mathf.Clamp01(speed);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
