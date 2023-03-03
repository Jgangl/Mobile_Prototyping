using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hazard : MonoBehaviour
{
    public GameObject bloodSplatterPrefab;

    private bool collided = false;

    private void Start()
    {
        Level_Manager.Instance.OnLevelReset += OnLevelReset;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        // Player has already collided
        if (collided)
            return;
        
        if (player)
            player.SetIsDead(true);

        
        // Set collided flag to stop multiple collisions from player bones
        collided = true;
        GameManager.Instance.PlayerDied();
    }

    private void OnLevelReset()
    {
        collided = false;
    }
}
