using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private bool playerCollided = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCollided = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.root.gameObject.CompareTag("Player") && !playerCollided) {
            playerCollided = true;
            GameManager.Instance.GameOver();
        }
    }
}
