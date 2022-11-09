using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Transform homeButton;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enable(bool enabled, bool homeButtonEnabled)
    {
        Animator anim = GetComponent<Animator>();
        if (anim) {
            if (enabled)
                anim.SetTrigger("Open");
            else
                anim.SetTrigger("Close");
        }

        homeButton.gameObject.SetActive(homeButtonEnabled);
    }
}
