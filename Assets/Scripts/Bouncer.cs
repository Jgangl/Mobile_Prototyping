using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Bouncer : MonoBehaviour
{
    public float bounceForce = 10f;

    public bool visualizeDirection;

    public bool enableBouncing = true;

    Transform bouncerTransform;
    Transform leftCircle;
    Transform rightCircle;
    Transform middleSquare;
    CapsuleCollider2D collider;

    Vector3 bouncerScale;

    // Start is called before the first frame update
    void Start()
    {
        bouncerScale = transform.localScale;

        bouncerTransform = transform;
        leftCircle = transform.Find("LeftCircle");
        rightCircle = transform.Find("RightCircle");
        middleSquare = transform.Find("MiddleSquare");
        collider = middleSquare.GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bouncerTransform != null && bouncerTransform.hasChanged) {
            // update scales
            bouncerScale = transform.localScale;

            if (bouncerTransform == null)
                bouncerTransform = transform;

            if (leftCircle == null)
                leftCircle = transform.Find("LeftCircle");

            if (rightCircle == null)
                rightCircle = transform.Find("RightCircle");

            if (middleSquare == null)
                middleSquare = transform.Find("MiddleSquare");

            if (collider == null && middleSquare != null)
                collider = middleSquare.GetComponent<CapsuleCollider2D>();

            // Update child object scales
            //Debug.Log("Updating circle y scale with: " + bouncerScale.x);
            leftCircle.localScale = new Vector3(leftCircle.localScale.x, (bouncerScale.x / bouncerScale.y), leftCircle.localScale.z);
            //leftCircle.localPosition = new Vector3(leftCircle.localPosition.x, bouncerScale.x + leftCircle.localScale.y, leftCircle.localPosition.z);
            //leftCircle.localPosition = new Vector3(leftCircle.localPosition.x, bouncerScale.x, leftCircle.localPosition.z);

            rightCircle.localScale = new Vector3(rightCircle.localScale.x, (bouncerScale.x / bouncerScale.y), rightCircle.localScale.z);
            //rightCircle.localPosition = new Vector3(rightCircle.localPosition.x, -(bouncerScale.x + rightCircle.localScale.y), rightCircle.localPosition.z);
            //rightCircle.localPosition = new Vector3(rightCircle.localPosition.x, -(bouncerScale.x), rightCircle.localPosition.z);



            // circle y scale = bouncer x scale / bouncer y scale
            // circle y pos = bouncer x scale + (circle y scale / 2)

            collider.size = new Vector2(collider.size.x, 1 + (bouncerScale.x / bouncerScale.y));
        }

        if (visualizeDirection)
            Debug.DrawRay(transform.position, transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Bouncer hit: " + collision.gameObject);

        if (!enableBouncing)
            return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        Vector2 contactDir = collision.contacts[0].normal;
        //Debug.DrawRay(transform.position, contactDir);

        Vector2 dir = transform.rotation * Vector3.forward;
        //Debug.DrawLine(transform.position, dir * 3f, Color.yellow, 3f);
        if (rb) {
            rb.AddForce(transform.right * bounceForce, ForceMode2D.Impulse);
        }
    }

    private void OnValidate() {
        bouncerScale = transform.localScale;

        if (bouncerTransform == null)
            bouncerTransform = transform;

        if (leftCircle == null)
            leftCircle = transform.Find("LeftCircle");

        if (rightCircle == null)
            rightCircle = transform.Find("RightCircle");

        if (middleSquare == null)
            middleSquare = transform.Find("MiddleSquare");

        if (collider == null && middleSquare != null)
            collider = middleSquare.GetComponent<CapsuleCollider2D>();

        // Update child object scales
        //Debug.Log("Updating circle y scale with: " + bouncerScale.x);
        leftCircle.localScale = new Vector3(leftCircle.localScale.x, bouncerScale.x, leftCircle.localScale.z);
        rightCircle.localScale = new Vector3(rightCircle.localScale.x, bouncerScale.x, rightCircle.localScale.z);

        collider.size = new Vector2(collider.size.x, 1 + bouncerScale.x);
    }

    
}
