using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Bouncer : MonoBehaviour
{
    public float bounceForce = 10f;
    [Range(2f, 4f)]
    public float playerSpeedBounceFactor = 3f;

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

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (!enableBouncing || !canCollideWithBouncer)
            return;

        Transform parent = collision.transform.parent;
        if (!parent || !parent.TryGetComponent(out PlayerController player))
        {
            return;
        }

        Vector2 playerVelocity  = player.GetObjectAverageVelocity();
        float   playerSpeed     = playerVelocity.magnitude;
        Vector2 bounceDirection = transform.right;
        Vector2 bounceVelocity  = bounceDirection * bounceForce;

        // using player speed to allow player to still bounce when going fast
        Vector2 targetVelocity = playerVelocity + (bounceVelocity * (playerSpeed / playerSpeedBounceFactor));

        player.SetVelocity(targetVelocity);

        StartCoroutine("CollisionTimeoutTimer");
    }

    private void OnValidate() 
    {
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

        leftCircle.localScale = new Vector3(leftCircle.localScale.x, bouncerScale.x, leftCircle.localScale.z);
        rightCircle.localScale = new Vector3(rightCircle.localScale.x, bouncerScale.x, rightCircle.localScale.z);

        bounceCollider.size = new Vector2(bounceCollider.size.x, 1 + bouncerScale.x);
    }

    IEnumerator CollisionTimeoutTimer() 
    {
        canCollideWithBouncer = false;
        
        if (isSimulated)
            yield return new WaitForSeconds(simulatedCollisionTimeoutTime);
        else
            yield return new WaitForSeconds(collisionTimeoutTime);
        
        canCollideWithBouncer = true;
    }
}
