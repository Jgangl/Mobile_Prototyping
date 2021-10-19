using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float cameraX = 0.0f;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() {
        transform.position = new Vector3(cameraX, player.transform.position.y, transform.position.z);
    }
}
