using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlimeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject slimeBallPrefab;

    public void Generate(Vector3 position)
    {
        GameObject slimeBall = Instantiate(slimeBallPrefab, position, Quaternion.identity);
        
        /*
        int randNumBalls = Random.Range(2, 4);

        for (int i = 0; i < randNumBalls; i++)
        {
            float randX = Random.Range(0f, 0.1f);
            Vector2 randForce = Vector2.up + new Vector2(randX, 0f);
            GameObject slimeBall = Instantiate(slimeBallPrefab, transform.position, Quaternion.identity);
            Rigidbody2D slimeRb = slimeBall.GetComponent<Rigidbody2D>();
            slimeRb.AddForce(randForce);
        }
        */
    }
}
