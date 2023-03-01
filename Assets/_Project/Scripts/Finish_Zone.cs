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
        PlayerController playerController;
        playerController = collision.GetComponent<PlayerController>();

        if (!playerController)
        {
            playerController = collision.transform.parent.GetComponent<PlayerController>();
        }

        // Didn't hit player
        if (!playerController)
        {
            return;
        }

        if (!playerCollided) 
        {
            GameManager.Instance.CompleteLevel();
        }

        playerCollided = true;
    }

    void OnLevelReset()
    {
        playerCollided = false;
    }
}
