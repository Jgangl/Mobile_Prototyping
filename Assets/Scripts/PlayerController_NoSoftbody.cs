using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_NoSoftbody : MonoBehaviour
{
    public float speed;

    Rigidbody2D rb;

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

    private Vector2 fingerDownPos;
    private Vector2 fingerCurrentPos;
    private Vector2 fingerUpPos;

    private bool mouseHeldDown = false;

    private Vector2 prevFingerPos;

    public float SWIPE_LENGTH_THRESHOLD = 0.25f;

    private Vector2 currentSwipeForce;

    public float squishSoundTime = 0.25f;
    public float bonesCollisionTime = 0.05f;

    public GameObject hitParticles;

    private Vector2 prevPosition;

    public bool disableInput = false;

    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        //canMove = false;
    }

    // Update is called once per frame
    void Update() {
        if (tag != "Player")
            return;

        if (disableInput)
            return;
        /*
        Touch touch = Input.touches[0];
        if (touch.phase == TouchPhase.Began || Input.GetMouseButtonDown(0)) {
            fingerDownPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved || mouseMoved) {
            fingerCurrentPos = touch.position;

            // Even in the Moved touchPhase, the finger wasn't actually 'moving' much
            float fingerPosDiff = Vector2.Distance(prevFingerPos, fingerCurrentPos);
            if (fingerPosDiff >= SWIPE_LENGTH_THRESHOLD) {
                // Calculate current position difference
                Vector2 currSwipeDirection = (fingerCurrentPos - fingerDownPos).normalized;
                float currSwipeLength = Vector2.Distance(fingerCurrentPos, fingerDownPos);

                // Calculate force
                currentSwipeForce = (currSwipeDirection * -1) * speed * (currSwipeLength * swipeLengthVariableGain * swipeLengthFlatGain);

                // Simulate launch
                trajectoryPredictor.SimulateLaunch(gameObject.transform, currentSwipeForce);
            }

        }
        else if (touch.phase == TouchPhase.Ended) {
            fingerUpPos = touch.position;
            trajectoryPredictor.ClearSimulation();
            // Enable Movement
            StartMovement();
            // Add swipe force
            rb.AddForce(currentSwipeForce);
        }
        
        prevFingerPos = touch.position;
        */

        Vector2 mousePosition = Input.mousePosition;
        bool mouseMoved = Vector2.Distance(prevFingerPos, mousePosition) >= 0.25f;
        if (canMove) {
            if (Input.GetMouseButtonDown(0)) {
                fingerDownPos = Input.mousePosition;
                mouseHeldDown = true;
            }
            else if (Input.GetMouseButtonUp(0)) {
                fingerUpPos = Input.mousePosition;
                mouseHeldDown = false;
                
                if (currentSwipeForce.x >= 0.01f || currentSwipeForce.y >= 0.01f) {
                    // Enable Movement
                    StartMovement();
                    rb.AddForce(currentSwipeForce);
                    
                    /*
                    Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
                    

                    // Add swipe force
                    foreach (Rigidbody2D bone in bones)
                        bone.AddForce(currentSwipeForce);
                    */
                }

                currentSwipeForce = Vector2.zero;
            }
            else if (mouseHeldDown && mouseMoved) {
                fingerCurrentPos = Input.mousePosition;

                // Even in the Moved touchPhase, the finger wasn't actually 'moving' much
                float fingerPosDiff = Vector2.Distance(prevFingerPos, fingerCurrentPos);
                if (fingerPosDiff >= SWIPE_LENGTH_THRESHOLD) {
                    // Calculate current position difference
                    Vector2 currSwipeDirection = (fingerCurrentPos - fingerDownPos).normalized;
                    float currSwipeLength = Vector2.Distance(fingerCurrentPos, fingerDownPos);

                    // Clamp swipe length
                    if (currSwipeLength > maxSwipeLength)
                        currSwipeLength = maxSwipeLength;

                    // Calculate force
                    currentSwipeForce = (currSwipeDirection * -1) * speed * (currSwipeLength * swipeLengthVariableGain * swipeLengthFlatGain);
                }
            }
        }

        Vector2 velocity = rb.velocity;
        
        anim.SetFloat("speed", velocity.magnitude);

        Vector2 velocityDirection = velocity.normalized;

        
        
        if (Mathf.Approximately(velocityDirection.x, 0f) && Mathf.Approximately(velocityDirection.y, 0f)) return;
        
        float angle = Mathf.Atan2(velocityDirection.y, velocityDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

#if UNITY_EDITOR
        prevFingerPos = Input.mousePosition;
#elif UNITY_ANDROID
        //prevFingerPos = Input.touches[0].position;
#endif
    }

    private void StopMovement(Rigidbody2D rb) {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    public void StartMovement() {
        StartCoroutine("IgnorePlatformTimer");

        if (!rb)
            rb = GetComponent<Rigidbody2D>();

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
    private Vector2 GetObjectAveragePosition() {
        Vector3 avgPos = Vector2.zero;

        // Get all rigidbodies of bones
        Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
        if (bones.Length == 0)
            return Vector2.zero;

        // Average all bone positions
        foreach (Rigidbody2D rb in bones) {
            avgPos += rb.transform.localPosition;
        }

        return (avgPos / bones.Length);
    }

    public Vector2 GetObjectAverageVelocity() {
        Vector2 totalVelocity = Vector2.zero;

        // Get all rigidbodies of bones
        Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
        if (bones.Length == 0)
            return Vector2.zero;

        // Average all bone positions
        foreach (Rigidbody2D rb in bones) {
            totalVelocity += rb.velocity;
        }

        return (totalVelocity / bones.Length);
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        // Don't collide with other bones
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerSim")
            return;
    

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

    public void OnChildCollisionStay2D(Bone_Softbody bone, Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            if (collision.gameObject == previousPlatform && canCollideWithPreviousPlatform) {
                //StopMovement(bone.GetComponent<Rigidbody2D>());
                StopMovement(this.rb);
            }
        }
    }

    public void OnChildCollisionExit2D(Bone_Softbody bone, Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            //Debug.Log("Exit platform top");
        }
    }

    public void SetVelocity(Vector2 newVelocity) {
        Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
        foreach(Rigidbody2D bone in bones) {
            bone.velocity = newVelocity;
        }
    }
}
