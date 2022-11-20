using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlimeBall : MonoBehaviour
{
    [SerializeField] private GameObject sprite;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        //rb  = GetComponent<Rigidbody2D>();
        col = GetComponentInChildren<Collider2D>();
        
        float randXScale = Random.Range(0.8f, 1.2f);
        float randYScale = Random.Range(0.8f, 1.2f);
        Vector2 randScale = new Vector2(randXScale, randYScale);
        
        transform.localScale = new Vector3(randScale.x, randScale.y, 1f);
    }
/*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<SlimeBall>(out SlimeBall slimeBall))
        {
            return;
        }
        
        Debug.Log(collision.gameObject);
        rb.isKinematic = true;
        col.enabled = false;

        rb.velocity = Vector2.zero;
        
        GenerateSlime(collision.contacts[0].point);
    }

    private void GenerateSlime(Vector3 position)
    {
        sprite.transform.position = position;

        float randXScale = Random.Range(0.8f, 1.2f);
        float randYScale = Random.Range(0.8f, 1.2f);
        Vector2 randScale = new Vector2(randXScale, randYScale);
        
        sprite.transform.localScale = new Vector3(randScale.x, randScale.y, 1f);
    }
*/
}
