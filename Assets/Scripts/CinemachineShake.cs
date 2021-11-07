using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : Singleton<CinemachineShake>
{
    private CinemachineVirtualCamera cmVirtualCam;
    private CinemachineBasicMultiChannelPerlin cmPerlin;

    // Start is called before the first frame update
    void Start()
    {
        cmVirtualCam = GetComponent<CinemachineVirtualCamera>();
        cmPerlin = cmVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float duration) {
        cmPerlin.m_AmplitudeGain = intensity;

        StartCoroutine(ShakeTimer(duration));
    }

    IEnumerator ShakeTimer(float duration) {
        Debug.Log("Start Shaking");
        float shakeTimer = 0f;
        while (shakeTimer < duration) {
            shakeTimer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Stop Shaking");
        cmPerlin.m_AmplitudeGain = 0f;

        yield return 0;
    }
}
