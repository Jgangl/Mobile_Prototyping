using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private bool directionLeft;

    Rigidbody2D rb;

    SwipeDetector swipeDetector;

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

    private TrajectoryPredictor trajectoryPredictor;

    private Vector2 prevMousePos;
    private Vector2 prevFingerPos;

    public float SWIPE_LENGTH_THRESHOLD = 50f;

    public Vector3 testForce;

    private Vector3 currentSwipeForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swipeDetector = SwipeDetector.Instance;

        trajectoryPredictor = TrajectoryPredictor.Instance;

        //rb.AddForce(testForce);

        //swipeDetector.AddSwipeListener(OnSwipe);
    }

    // Update is called once per frame
    void Update() {

        Vector2 mousePosition = Input.mousePosition;
        bool mouseMoved = Vector2.Distance(prevMousePos, mousePosition) <= 0.1f;

        if (Input.touchCount == 1) {
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

                    currentSwipeForce = (currSwipeDirection * -1) * speed * (currSwipeLength * swipeLengthVariableGain * swipeLengthFlatGain);
                    // Calculate force 
                    trajectoryPredictor.SimulateLaunch(gameObject.transform, currentSwipeForce);
                    //Debug.Log("Simulating Launch");
                    // Simulate launch
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
        }

        if (Input.GetMouseButtonDown(0)) {
            //rb.AddForce(testForce);
            //trajectoryPredictor.SimulateLaunch(gameObject.transform, testForce);
        }


        prevMousePos = Input.mousePosition;
    }

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
    }

    private void StartMovement() {
        StartCoroutine("IgnorePlatformTimer");
        rb.isKinematic = false;
    }

    IEnumerator IgnorePlatformTimer() {
        canCollideWithPreviousPlatform = false;
        yield return new WaitForSeconds(platformIgnoreTime);
        canCollideWithPreviousPlatform = true;
    }
}
