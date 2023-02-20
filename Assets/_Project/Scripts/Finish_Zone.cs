using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish_Zone : MonoBehaviour
{
    bool playerCollided = false;

    void Start()
    {
        Level_Manager.Instance.OnLevelReset += OnLevelReset;
    }

    void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.tag == "Player") 
        {
            if (!playerCollided) 
            {
                GameManager.Instance.CompleteLevel();
            }

            playerCollided = true;
        }
    }

    void OnLevelReset()
    {
        playerCollided = false;
    }
}
