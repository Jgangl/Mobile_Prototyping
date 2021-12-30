using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateVector = new Vector3(0f, 0f, rotateSpeed * Time.deltaTime);
        transform.Rotate(rotateVector);
    }
}
