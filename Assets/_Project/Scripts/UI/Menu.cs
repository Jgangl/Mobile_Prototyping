using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        if (exitButton)
            exitButton.onClick.AddListener(Close);
    }

    public void Open()
    {
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
