using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_NoSoftbody : MonoBehaviour
{
    public float speed;

    Rigidbody2D rb;
    Animator anim;
    BasicTrajectory basicTrajectory;

    [SerializeField]
    private bool canMove;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float swipeLengthVariableGain = 1.0f;
    private float swipeLengthFlatGain = 0.01f;

    private GameObject previousPlatform;
    private bool canCollideWithPreviousPlatform = true;

    [SerializeField]
    private float platformIgnoreTime = 0.25f;

    public float maxSwipeLength = 300f;

    Vector2 fingerDownPos;
    Vector2 fingerCurrentPos;
    Vector2 fingerUpPos;

    bool mouseHeldDown = false;

    Vector2 prevFingerPos;

    public float SWIPE_LENGTH_THRESHOLD = 0.25f;

    Vector2 currentSwipeForce;

    float squishSoundTime = 0.25f;

    [SerializeField]
    GameObject hitParticles;
    
    [SerializeField]
    bool disableInput = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        basicTrajectory = GetComponent<BasicTrajectory>();
    }
    
    void Update()
    {
        if (disableInput) return;
        
        bool mouseMoved = CheckMouseMoved(GetMousePosition(), prevFingerPos);
        if (canMove) 
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                HandleOnMouseDown();
            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                HandleOnMouseUp();
            }
            else if (mouseHeldDown && mouseMoved)
            {
                HandleMouseHeldDown();
            }
        }

        UpdateAnimator();
        RotateTowardsVelocity();
        
        UpdatePreviousFingerPosition();
    }

    private Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }

    private bool CheckMouseMoved(Vector2 currentPos, Vector2 prevPos)
    {
        return Vector2.Distance(currentPos, prevPos) >= 0.25f;
    }

    private static Vector2 UpdateMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        return mousePosition;
    }

    private void HandleOnMouseDown()
    {
        fingerDownPos = Input.mousePosition;
        mouseHeldDown = true;
    }

    private void HandleOnMouseUp()
    {
        fingerUpPos = Input.mousePosition;
        mouseHeldDown = false;
                
        if (currentSwipeForce.x >= 0.01f || currentSwipeForce.y >= 0.01f) {
            // Enable Movement
            StartMovement();

            rb.AddForce(currentSwipeForce);
        }

        currentSwipeForce = Vector2.zero;
        
        TrajectoryPredictor_New.Instance.ClearSimulation();
        //basicTrajectory.ClearArc();
    }

    private void HandleMouseHeldDown()
    {
        fingerCurrentPos = Input.mousePosition;

        // Even in the Moved touchPhase, the finger wasn't actually 'moving' much
        float fingerPosDiff = Vector2.Distance(prevFingerPos, fingerCurrentPos);
        if (fingerPosDiff >= SWIPE_LENGTH_THRESHOLD) {
            // Calculate current position difference
            Vector2 currSwipeDirection = (fingerCurrentPos - fingerDownPos).normalized;
            Vector2 forceDirection = currSwipeDirection * -1;

            float currSwipeLength = Vector2.Distance(fingerCurrentPos, fingerDownPos);

            // Clamp swipe length
            if (currSwipeLength > maxSwipeLength)
                currSwipeLength = maxSwipeLength;

            // Calculate force
            currentSwipeForce = (currSwipeDirection * -1) * speed * (currSwipeLength * swipeLengthVariableGain * swipeLengthFlatGain);

            //Vector2 directionVector = transform.position + new Vector3(forceDirection.x, forceDirection.y, 0f);

            //basicTrajectory.SimulateArc(transform.position, currentSwipeForce.normalized, currentSwipeForce, rb.mass, rb.gravityScale);
            //print("Simulation force: " + currentSwipeForce);
            TrajectoryPredictor_New.Instance.SimulateTrajectory(gameObject, currentSwipeForce);
        }
    }

    private void UpdatePreviousFingerPosition()
    {
        prevFingerPos = Input.mousePosition;
    }

    private void UpdateAnimator()
    {
        anim.SetFloat("speed", rb.velocity.magnitude);
    }

    private void RotateTowardsVelocity()
    {
        Vector2 velocityDirection = rb.velocity.normalized;

        if (Mathf.Approximately(velocityDirection.x, 0f) && Mathf.Approximately(velocityDirection.y, 0f)) return;
        
        float angle = Mathf.Atan2(velocityDirection.y, velocityDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void StopMovement(Rigidbody2D rb) {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    public void StartMovement() {
        StartCoroutine("IgnorePlatformTimer");
        
        rb.isKinematic = false;
        canMove = false;
    }

    public void EnableMovement(bool enabled) {
        disableInput = !enabled;
    }

    IEnumerator IgnorePlatformTimer() {
        canCollideWithPreviousPlatform = false;
        yield return new WaitForSeconds(platformIgnoreTime);
        canCollideWithPreviousPlatform = true;
    }
    /*
    IEnumerator SquishSoundTimer() {
        canPlaySquishSound = false;
        yield return new WaitForSeconds(squishSoundTime);
        canPlaySquishSound = true;
    }
    */

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {            
            if (collision.gameObject == previousPlatform) {
                if (canCollideWithPreviousPlatform) {
                    StopMovement(rb);
                }
            }
            else {
                StopMovement(rb);
            }

            previousPlatform = collision.gameObject;
        }
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            if (collision.gameObject == previousPlatform && canCollideWithPreviousPlatform) {
                StopMovement(rb);
            }
        }
    }
}
