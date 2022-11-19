using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : Singleton<CinemachineShake>
{
    private CinemachineVirtualCamera cmVirtualCam;
    private CinemachineBasicMultiChannelPerlin cmPerlin;

    private bool cameraShakeEnabled;

    // Start is called before the first frame update
    void Start()
    {
        cameraShakeEnabled = true;
        cmVirtualCam = GetComponent<CinemachineVirtualCamera>();
        cmPerlin = cmVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float duration) {
        if (!cameraShakeEnabled)
        {
            return;
        }
        
        cmPerlin.m_AmplitudeGain = intensity;
        StartCoroutine(ShakeTimer(duration));
    }

    IEnumerator ShakeTimer(float duration) {
        float shakeTimer = 0f;
        while (shakeTimer < duration) {
            shakeTimer += Time.deltaTime;
            yield return null;
        }

        cmPerlin.m_AmplitudeGain = 0f;

        yield return 0;
    }

    public void EnableCameraShake()
    {
        cameraShakeEnabled = true;
    }
    
    public void DisableCameraShake()
    {
        cameraShakeEnabled = false;
    }
}
