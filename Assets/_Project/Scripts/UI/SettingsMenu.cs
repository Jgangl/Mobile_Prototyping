using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : Menu
{
    [SerializeField] private Transform homeButton;
    /*
    public void Enable(bool enabled)
    {
        Animator anim = GetComponent<Animator>();
        if (anim) {
            if (enabled)
                anim.SetTrigger("Open");
            else
                anim.SetTrigger("Close");
        }

        //homeButton.gameObject.SetActive(homeButtonEnabled);
    }
    */
    public void EnableHomeButton(bool enabled)
    {
        homeButton.gameObject.SetActive(enabled);
    }
}
