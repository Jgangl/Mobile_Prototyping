using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 1.0f;

    public GameObject targetA;
    public GameObject targetB;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = targetA;
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed);

        if (Vector2.Distance(transform.position, target.transform.position) < 0.01f) {
            // Swap targets
            if (target == targetA)
                target = targetB;
            else
                target = targetA;
        }
    }
}
