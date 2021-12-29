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
    CapsuleCollider2D bounceCollider;

    Vector3 bouncerScale;

    private bool canCollideWithBouncer = true;

    [SerializeField]
    private float collisionTimeoutTime = 0.25f;

    [SerializeField]
    private float simulatedCollisionTimeoutTime = 0.01f;

    public bool isSimulated = false;

    // Start is called before the first frame update
    void Start()
    {
        bouncerScale = transform.localScale;

        bouncerTransform = transform;
        leftCircle = transform.Find("LeftCircle");
        rightCircle = transform.Find("RightCircle");
        middleSquare = transform.Find("MiddleSquare");
        bounceCollider = middleSquare.GetComponent<CapsuleCollider2D>();
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

            if (bounceCollider == null && middleSquare != null)
                bounceCollider = middleSquare.GetComponent<CapsuleCollider2D>();

            // Update child object scales
            leftCircle.localScale = new Vector3(leftCircle.localScale.x, (bouncerScale.x / bouncerScale.y), leftCircle.localScale.z);
            rightCircle.localScale = new Vector3(rightCircle.localScale.x, (bouncerScale.x / bouncerScale.y), rightCircle.localScale.z);
            bounceCollider.size = new Vector2(bounceCollider.size.x, 1 + (bouncerScale.x / bouncerScale.y));
        }

        if (visualizeDirection)
            Debug.DrawRay(transform.position, transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        print("hit");
        if (isSimulated) {
            //Debug.Log("Simulated Bouncer hit: " + collision.transform.root.gameObject);
        }
        else {
            //Debug.Log("Bouncer hit: " + collision.transform.root.gameObject);
        }

        if (!enableBouncing || !canCollideWithBouncer)
            return;

        Rigidbody2D[] bones = collision.transform.root.gameObject.GetComponentsInChildren<Rigidbody2D>();
        Vector2 contactDir = collision.contacts[0].normal;

        //if (bones[0].gameObject.layer == LayerMask.NameToLayer("PlayerSim"))
        //    Debug.Log("Bouncer hit sim player");

        Vector2 dir = transform.rotation * Vector3.forward;
        if (bones.Length != 0) {
            Debug.Log("Adding force to bones");
            foreach(Rigidbody2D bone in bones) {
                bone.AddForce(transform.right * bounceForce, ForceMode2D.Impulse);
                //Debug.Log("Bouncer adding force to bone for sim player");
            }

            // Start collisionn timeout timer to avoid multiple collisions in a small time frame
            StartCoroutine("CollisionTimeoutTimer");
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

        if (bounceCollider == null && middleSquare != null)
            bounceCollider = middleSquare.GetComponent<CapsuleCollider2D>();

        // Update child object scales
        //Debug.Log("Updating circle y scale with: " + bouncerScale.x);
        leftCircle.localScale = new Vector3(leftCircle.localScale.x, bouncerScale.x, leftCircle.localScale.z);
        rightCircle.localScale = new Vector3(rightCircle.localScale.x, bouncerScale.x, rightCircle.localScale.z);

        bounceCollider.size = new Vector2(bounceCollider.size.x, 1 + bouncerScale.x);
    }

    IEnumerator CollisionTimeoutTimer() {
        canCollideWithBouncer = false;
        if (isSimulated)
            yield return new WaitForSeconds(simulatedCollisionTimeoutTime);
        else
            yield return new WaitForSeconds(collisionTimeoutTime);
        canCollideWithBouncer = true;
    }
}
