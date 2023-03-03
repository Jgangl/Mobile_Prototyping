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
    [SerializeField] ParticleSystem[] slimePrefabs;

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
        /*
        slimePool = new ObjectPool<GameObject>( CreatePooledItem, 
                                                OnTakeFromPool, 
                                                OnReturnedToPool, 
                                                OnDestroyPoolObject, 
                                                collectionChecks,
                                                slimePoolSize);
        */
        slimeBallParent = new GameObject("SlimeBallParent").transform;
    }

    public void Generate(Vector3 position, GameObject hitObject)
    {
        ParticleSystem slime = Instantiate(slimePrefabs[0], position, Quaternion.identity);
        
        //GameObject slime = slimePool.Get();
        MovingObject movingObject = hitObject.GetComponentInParent<MovingObject>();
        //MovingPlatform movingPlatform = hitObject.GetComponentInParent<MovingPlatform>();

        bool bParentToHitObject = movingObject != null;
        
        slime.transform.position = position;
        
        // Parent to a moving object
        if (bParentToHitObject)
            slime.transform.parent = movingObject.transform;
    }

    GameObject CreatePooledItem()
    {
        GameObject randomSlimePrefab = GetRandomSlimePrefab();
        
        GameObject slimeBall = Instantiate(randomSlimePrefab, Vector2.zero, Quaternion.identity);
        slimeBall.SetActive(false);

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

            slimePool.Release(slimeToFade);
        }
    }

    void OnReturnedToPool(GameObject slime)
    {
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
