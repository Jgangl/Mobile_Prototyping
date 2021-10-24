using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float bounceForce = 10f;

    public bool visualizeDirection;

    public bool enableBouncing = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (visualizeDirection)
            Debug.DrawRay(transform.position, transform.up);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!enableBouncing)
            return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        Vector2 contactDir = collision.contacts[0].normal;
        Debug.DrawRay(transform.position, contactDir);

        Vector2 dir = transform.rotation * Vector3.forward;
        //Debug.DrawLine(transform.position, dir * 3f, Color.yellow, 3f);
        if (rb) {
            rb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}
