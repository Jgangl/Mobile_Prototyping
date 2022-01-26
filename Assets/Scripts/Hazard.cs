using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public GameObject bloodSplatterPrefab;

    private bool collided = false;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.root.tag != "Player")
            return;

        if (collided)
            return;
        
        // Set collided flag to stop multiple collisions from player bones
        collided = true;

        Vector2 splatterPosition = collision.contacts[0].point;

        // Create splatter at player collision point
        CreateSplatter(splatterPosition);

        GameManager.Instance.PlayerDied();

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        //collision.transform.parent = transform;
    }

    private void CreateSplatter(Vector2 splatterPos) {
        Quaternion randomRot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        if (transform.parent != null)
            Instantiate(bloodSplatterPrefab, splatterPos, randomRot, transform.parent);
        else
            Instantiate(bloodSplatterPrefab, splatterPos, randomRot);
    }
}
