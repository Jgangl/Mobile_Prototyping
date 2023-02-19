using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    public float speed = 0.05f;

    public GameObject targetA;
    public GameObject targetB;
    private GameObject target;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = targetA;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(Vector2.MoveTowards(transform.position, target.transform.position, speed));

        if (Vector2.Distance(transform.position, target.transform.position) < 0.01f) {
            // Swap targets
            if (target == targetA)
                target = targetB;
            else
                target = targetA;
        }
    }
}
