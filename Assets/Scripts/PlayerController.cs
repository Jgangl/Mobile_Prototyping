using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    Rigidbody2D rb;

    [SerializeField]
    private bool canMove = true;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float swipeLengthVariableGain = 1.0f;

    private float swipeLengthFlatGain = 0.01f;

    private GameObject previousPlatform;
    private bool canCollideWithPreviousPlatform = true;

    [SerializeField]
    private float platformIgnoreTime = 0.25f;

    private Vector2 fingerDownPos;
    private Vector2 fingerCurrentPos;
    private Vector2 fingerUpPos;

    private bool mouseHeldDown = false;

    private Vector2 prevFingerPos;

    public float SWIPE_LENGTH_THRESHOLD = 50f;

    private Vector2 currentSwipeForce;

    private TrajectoryPredictor trajectoryPredictor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        trajectoryPredictor = TrajectoryPredictor.Instance;
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

                trajectoryPredictor.ClearSimulation();

                // Enable Movement
                StartMovement();

                // Add swipe force
                rb.AddForce(currentSwipeForce);
            }
            else if (mouseHeldDown && mouseMoved) {
                fingerCurrentPos = Input.mousePosition;

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
        }
        

#if UNITY_EDITOR
        prevFingerPos = Input.mousePosition;
#elif UNITY_ANDROID
        prevFingerPos = Input.touches[0].position;
#endif
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            //Debug.Log("Hit platform");
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

    private void StopMovement() {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    private void StartMovement() {
        StartCoroutine("IgnorePlatformTimer");
        rb.isKinematic = false;
        canMove = false;
    }

    IEnumerator IgnorePlatformTimer() {
        canCollideWithPreviousPlatform = false;
        yield return new WaitForSeconds(platformIgnoreTime);
        canCollideWithPreviousPlatform = true;
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
