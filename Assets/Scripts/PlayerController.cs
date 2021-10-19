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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swipeDetector = SwipeDetector.Instance;

        swipeDetector.AddSwipeListener(OnSwipe);
    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {
        
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
        swipeDetector.RemoveSwipeListener(OnSwipe);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Platform") {
            Debug.Log("Hit platform");
            if (collision.gameObject == previousPlatform && canCollideWithPreviousPlatform) {
                Debug.Log("Stopping Movement");
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
            Debug.Log("Exit platform top");
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
