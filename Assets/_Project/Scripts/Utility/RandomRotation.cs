using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotation : MonoBehaviour
{
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        float randZRot = Random.Range(minRotation, maxRotation);
        Vector3 originalRotation = transform.rotation.eulerAngles;
        
        transform.rotation = quaternion.Euler(originalRotation.x, originalRotation.y, randZRot);
    }
    
}
