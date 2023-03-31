using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    bool collided = false;

    void Start()
    {
        Level_Manager.Instance.OnLevelReset += OnLevelReset;
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
/*
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();

        // Player has already collided
        if (collided)
            return;
        
        if (player)
            player.SetIsDead(true);
        else
        {
            Debug.Log("PLAYER NULL");
        }

        // Set collided flag to stop multiple collisions from player bones
        collided = true;
        GameManager.Instance.PlayerDied();
        */
    }

    public void Collided()
    {
        collided = true;
    }

    void OnLevelReset()
    {
        collided = false;
    }
}
