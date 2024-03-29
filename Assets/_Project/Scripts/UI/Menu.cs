using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    private Animator anim;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        
        if (exitButton)
            exitButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        UI_Manager.Instance.SetCurrentOpenMenu(this);
        if (anim) 
        {
            anim.SetTrigger("Open");
        }
    }

    public void Close()
    {
        if (anim)
        {
            anim.SetTrigger("Close");
        }
    }

    public void CloseInstant()
    {
        if (anim)
        {
            anim.SetTrigger("Close_Instant");
        }
    }
}
