using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    private TrajectoryPredictor trajectoryPredictor;

    public float squishSoundTime = 0.25f;
    private bool canPlaySquishSound = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        trajectoryPredictor = TrajectoryPredictor.Instance;

        canMove = false;
    }

    // Update is called once per frame
    void Update() {
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
                if (trajectoryPredictor)
                    trajectoryPredictor.ClearSimulation();

                // Enable Movement
                Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
                //foreach(Rigidbody2D bone in bones)
                StartMovement(this.rb);

                // Add swipe force

                // TODO: TRY TO ADD FORCE TO ALL BONES
                foreach(Rigidbody2D bone in bones)
                    bone.AddForce(currentSwipeForce);
                //rb.AddForce(currentSwipeForce);
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
                    // Simulate launch
                    if (trajectoryPredictor)
                        trajectoryPredictor.SimulateLaunch(gameObject.transform, currentSwipeForce);
                }
            }
        }

#if UNITY_EDITOR
        prevFingerPos = Input.mousePosition;
#elif UNITY_ANDROID
        //prevFingerPos = Input.touches[0].position;
#endif
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            Debug.Log("Hit platform");
            if (collision.gameObject == previousPlatform && canCollideWithPreviousPlatform) {
                //Debug.Log("Stopping Movement");
                StopMovement();
            }

            previousPlatform = collision.gameObject;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            //Debug.Log("Hit platform");
            if (collision.gameObject == previousPlatform && canCollideWithPreviousPlatform) {
                //Debug.Log("Stopping Movement");
                StopMovement();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            //Debug.Log("Exit platform top");
        }
    }
    */

    private void StopMovement(Rigidbody2D rb) {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    private void StartMovement(Rigidbody2D rb) {
        StartCoroutine("IgnorePlatformTimer");
        rb.isKinematic = false;
        canMove = false;
    }

    IEnumerator IgnorePlatformTimer() {
        canCollideWithPreviousPlatform = false;
        yield return new WaitForSeconds(platformIgnoreTime);
        canCollideWithPreviousPlatform = true;
    }

    IEnumerator SquishSoundTimer() {
        canPlaySquishSound = false;
        yield return new WaitForSeconds(squishSoundTime);
        canPlaySquishSound = true;
    }

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

    public void OnChildCollisionEnter2D(Bone_Softbody bone, Collision2D collision) {
        // Don't collide with other bones
        if (collision.gameObject.tag == "Player")
            return;

        Debug.Log("BONE COLLISION HIT: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Platform") {
            if (collision.gameObject == previousPlatform && canCollideWithPreviousPlatform) {
                //Debug.Log("Play Squish Sound");
                if (canPlaySquishSound && !canMove) {
                    

                    Sound_Manager.Instance.PlaySquishSound();
                    StartCoroutine("SquishSoundTimer");

                    CinemachineShake.Instance.ShakeCamera(0.5f, 0.2f);
                }

                StopMovement(this.rb);
            }

            previousPlatform = collision.gameObject;
        }
    }

    // Need to figure out when player hits object

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
}



/*
    public void OnSwipe(Swipe newSwipe) {
        if (!canMove)
            return;

        StartMovement();
        Vector2 swipeDirection = newSwipe.GetDirection();
        float swipeLength = newSwipe.GetLength();

        rb.velocity = (swipeDirection.normalized * -1) * speed * (swipeLength * swipeLengthVariableGain * swipeLengthFlatGain);
    }


    private void OnDestroy() {
        //swipeDetector.RemoveSwipeListener(OnSwipe);
    }
*/
