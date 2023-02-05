using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Pool;

public class SlimeGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] slimeBallPrefabs;

    ObjectPool<GameObject> slimePool;

    public bool collectionChecks = true;

    [SerializeField] int slimePoolSize;
    int numActiveObjects;
    
    Queue<GameObject> slimeQueue;

    Transform slimeBallParent;

    GameObject oldestObject;

    void Start()
    {
        slimeQueue = new Queue<GameObject>();
        
        slimePool = new ObjectPool<GameObject>( CreatePooledItem, 
                                                OnTakeFromPool, 
                                                OnReturnedToPool, 
                                                OnDestroyPoolObject, 
                                                collectionChecks,
                                                slimePoolSize);
        
        slimeBallParent = new GameObject("SlimeBallParent").transform;
    }

    public void Generate(Vector3 position, GameObject hitObject)
    {
        GameObject slime = slimePool.Get();

        // If generating slime on moving platform, parent it to the hit transform
        if (hitObject.TryGetComponent(out MovingPlatform movingPlatform))
            slime.transform.parent = hitObject.transform;
        
        slime.transform.position = position;

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

    GameObject CreatePooledItem()
    {
        GameObject randomSlimePrefab = GetRandomSlimePrefab();
        
        GameObject slimeBall = Instantiate(randomSlimePrefab, Vector2.zero, Quaternion.identity);
        slimeBall.SetActive(false);
        /*
        foreach (Transform t in slimeBall.transform)
        {
            SpriteRenderer sprite = t.GetComponent<SpriteRenderer>();

            if (sprite)
            {
                Color currSpriteColor = sprite.color;
                currSpriteColor.a = 0f;
                sprite.color = currSpriteColor;
            }
        }
        */
        slimeBall.transform.parent = slimeBallParent;
        
        return slimeBall;
    }

    void OnTakeFromPool(GameObject slime)
    {
        slime.SetActive(true);
        
        slimeQueue.Enqueue(slime);

        if (slimeQueue.Count >= slimePoolSize - 2)
        {
            GameObject slimeToFade = slimeQueue.Dequeue();
            
            //slimeToFade
            
            slimePool.Release(slimeToFade);
        }

        /*
        foreach (Transform t in slime.transform)
        {
            SpriteRenderer sprite = t.GetComponent<SpriteRenderer>();

            if (sprite)
            {
                Color currSpriteColor = sprite.color;
                currSpriteColor.a = 255f;
                sprite.color = currSpriteColor;
            }
        }
        */
        
        //slimeQueue.Enqueue(slime);
    }

    void OnReturnedToPool(GameObject slime)
    {
        /*
        foreach (Transform t in slime.transform)
        {
            SpriteRenderer sprite = t.GetComponent<SpriteRenderer>();

            if (sprite)
            {
                sprite.DOFade(0f, 5f);
            }
        }
        */
        slime.SetActive(false);
    }

    void OnDestroyPoolObject(GameObject slime)
    {
        Destroy(slime);
    }

    GameObject GetRandomSlimePrefab()
    {
        int randIndex = Random.Range(0, slimeBallPrefabs.Length);

        return slimeBallPrefabs[randIndex];
    }
}
