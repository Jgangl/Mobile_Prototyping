using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish_Zone : MonoBehaviour
{
    bool playerCollided = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            if (!playerCollided) {
                GameManager.Instance.CompleteLevel();
            }

            playerCollided = true;
        }
    }
}
