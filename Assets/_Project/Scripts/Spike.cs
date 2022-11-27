using System;
using System.Collections;
using System.Collections.Generic;
using FastSpriteMask;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private MultiMask mask;
    
    // Start is called before the first frame update
    void Start()
    {
        mask = GetComponent<MultiMask>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameVisible()
    {
        mask.enabled = true;
    }

    private void OnBecameInvisible()
    {
        mask.enabled = false;
    }
}
