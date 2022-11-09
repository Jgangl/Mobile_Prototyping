using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : Singleton<ScreenShake>
{
    private CinemachineImpulseSource cmImpulseSource;

    private void Start()
    {
        cmImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        cmImpulseSource.GenerateImpulse(intensity);
    }
}
