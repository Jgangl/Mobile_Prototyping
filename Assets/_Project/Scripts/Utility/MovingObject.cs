using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 1.0f;

    public GameObject targetA;
    public GameObject targetB;
    private GameObject target;
    
    FixedJoint2D PlayerJoint;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerJoint = GetComponent<FixedJoint2D>();
        
        target = targetA;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float step = speed * Time.deltaTime;

        rb.MovePosition(Vector2.MoveTowards(transform.position, target.transform.position, speed));
        
        //rb.velocity = Vector2.right * speed;
        //transform.position = 

        if (Vector2.Distance(transform.position, target.transform.position) < 0.01f) {
            // Swap targets
            if (target == targetA)
                target = targetB;
            else
                target = targetA;
        }
    }
    
    public void EnableJoint()
    {
        PlayerJoint.enabled = true;
        PlayerJoint.autoConfigureConnectedAnchor = false;
    }
    
    public void DisableJoint()
    {
        PlayerJoint.autoConfigureConnectedAnchor = true;
        PlayerJoint.enabled = false;
    }
}
