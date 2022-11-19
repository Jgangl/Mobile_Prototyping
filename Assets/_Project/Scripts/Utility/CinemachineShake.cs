using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : Singleton<CinemachineShake>
{
    private CinemachineVirtualCamera cmVirtualCam;
    private CinemachineBasicMultiChannelPerlin cmPerlin;

    [SerializeField] private bool cameraShakeEnabled;
    [SerializeField] private bool saveCameraShakeEnabled;

    // Start is called before the first frame update
    void Start()
    {
        cmVirtualCam = GetComponent<CinemachineVirtualCamera>();
        cmPerlin = cmVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (saveCameraShakeEnabled)
        {
            if (PlayerPrefs.GetString("CameraShake_Enabled") == "")
            {
                PlayerPrefs.SetString("CameraShake_Enabled", cameraShakeEnabled.ToString());
            }
            else if (PlayerPrefs.GetString("CameraShake_Enabled") == "true")
            {
                cameraShakeEnabled = true;
            }
            else if (PlayerPrefs.GetString("CameraShake_Enabled") == "false")
            {
                cameraShakeEnabled = false;
            }
        }
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
        if (saveCameraShakeEnabled)
        {
            PlayerPrefs.SetString("CameraShake_Enabled", "true");
        }
    }
    
    public void DisableCameraShake()
    {
        cameraShakeEnabled = false;
        if (saveCameraShakeEnabled)
        {
            PlayerPrefs.SetString("CameraShake_Enabled", "false");
        }
    }
}
