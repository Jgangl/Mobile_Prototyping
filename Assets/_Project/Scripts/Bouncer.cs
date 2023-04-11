using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    
    Transform hitLeftCircle;
    Transform hitRightCircle;
    Transform hitMiddleSquare;

    [SerializeField] Vector3 bouncerScale;

    bool canCollideWithBouncer = true;

    [SerializeField]
    float collisionTimeoutTime = 0.25f;

    [SerializeField]
    float simulatedCollisionTimeoutTime = 0.01f;

    public bool isSimulated = false;

    [SerializeField]
    Collider2D[] baseColliders;
    [SerializeField]
    Collider2D hitCollider;
    Rigidbody2D rb;

    bool isAnimationStarted;
    bool isAnimationPlaying;

    // Start is called before the first frame update
    void Start()
    {
        //bouncerScale = transform.localScale;

        bouncerTransform = transform;
        leftCircle = transform.Find("LeftCircle");
        rightCircle = transform.Find("RightCircle");
        middleSquare = transform.Find("MiddleSquare");

        hitLeftCircle = transform.Find("HitSprite").Find("LeftCircle");
        hitRightCircle = transform.Find("HitSprite").Find("RightCircle");
        hitMiddleSquare = transform.Find("HitSprite").Find("MiddleSquare");
        
        bounceCollider = middleSquare.GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bouncerTransform != null && bouncerTransform.hasChanged) 
        {
            // update scales
            //bouncerScale = transform.localScale;

            if (bouncerTransform == null)
                bouncerTransform = transform;

            if (leftCircle == null)
                leftCircle = transform.Find("LeftCircle");

            if (rightCircle == null)
                rightCircle = transform.Find("RightCircle");

            if (middleSquare == null)
                middleSquare = transform.Find("MiddleSquare");
            
            if (hitLeftCircle == null)
                hitLeftCircle = transform.Find("HitSprite").Find("LeftCircle");

            if (hitRightCircle == null)
                hitRightCircle = transform.Find("HitSprite").Find("RightCircle");

            if (hitMiddleSquare == null)
                hitMiddleSquare = transform.Find("HitSprite").Find("MiddleSquare");

            if (bounceCollider == null && middleSquare != null)
                bounceCollider = middleSquare.GetComponent<CapsuleCollider2D>();

            // Update child object scales
            leftCircle.localScale = new Vector3(bouncerScale.y, bouncerScale.y , 1.0f);
            leftCircle.localPosition = new Vector3(0.0f, bouncerScale.x / 2);
            
            rightCircle.localScale = new Vector3(bouncerScale.y, bouncerScale.y , 1.0f);
            rightCircle.localPosition = new Vector3(0.0f, -bouncerScale.x / 2);
            
            middleSquare.localScale = new Vector3(bouncerScale.y - 0.005f, bouncerScale.x , 1.0f);


            hitLeftCircle.localScale = new Vector3(bouncerScale.y / 2, bouncerScale.y / 2 , 1.0f);
            hitLeftCircle.localPosition = new Vector3(bouncerScale.y / 4, bouncerScale.x / 2, 0.0f);
            
            hitRightCircle.localScale = new Vector3(bouncerScale.y / 2, bouncerScale.y / 2 , 1.0f);
            hitRightCircle.localPosition = new Vector3(bouncerScale.y / 4, -bouncerScale.x / 2, 0.0f);
            
            hitMiddleSquare.localScale = new Vector3(bouncerScale.y / 2 - 0.005f, bouncerScale.x , 1.0f);
            hitMiddleSquare.localPosition = new Vector3(bouncerScale.y / 4, 0.0f, 0.0f);

        }

        if (visualizeDirection)
            Debug.DrawRay(transform.position, transform.right);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.otherCollider != hitCollider)
            return;

        if (!enableBouncing || !canCollideWithBouncer)
            return;

        Transform parent = col.collider.transform.parent;
        if (!parent || !parent.TryGetComponent(out PlayerController player))
        {
            return;
        }

        Vector2 playerVelocity  = player.GetObjectAverageVelocity();
        float   playerSpeed     = playerVelocity.magnitude;
        Vector2 bounceDirection = transform.right;
        Vector2 bounceVelocity  = bounceDirection * bounceForce;

        // using player speed to allow player to still bounce when going fast
        //Vector2 targetVelocity = playerVelocity + (bounceVelocity * (playerSpeed / playerSpeedBounceFactor));

        Vector2 targetVelocity = playerVelocity + bounceVelocity;

        player.SetVelocity(targetVelocity);

        StartCoroutine("CollisionTimeoutTimer");

        PlayOnHitAnimations();
    }


    void OnTriggerEnter2D(Collider2D collision) 
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
        //Vector2 targetVelocity = playerVelocity + (bounceVelocity * (playerSpeed / playerSpeedBounceFactor));

        Vector2 targetVelocity = playerVelocity + bounceVelocity;

        player.SetVelocity(targetVelocity);

        StartCoroutine("CollisionTimeoutTimer");

        PlayOnHitAnimations();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Transform parent = other.transform.parent;
        if (!parent || !parent.TryGetComponent(out PlayerController player))
        {
            return;
        }
        
        // Player exited collider

        Debug.Log("On exit player");
        Debug.Log(isAnimationStarted);
        Debug.Log(isAnimationPlaying);
        Debug.Log(IsPlayerOverlapping());
        
        // NEED TO USE A TRIGGER OR SOMETHING, ONCOLLISIONEXIT IS NOT CALLED ON DISABLED COLLIDER
        
        if (isAnimationStarted && !isAnimationPlaying && !IsPlayerOverlapping())
        {
            isAnimationStarted = false;
            Debug.Log("Enabling collision from collision exit");
            //EnableCollision(true);
        }
    }

    void OnValidate() 
    {
        //bouncerScale = transform.localScale;

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
        leftCircle.localScale = new Vector3(bouncerScale.y, bouncerScale.y , 1.0f);
        leftCircle.localPosition = new Vector3(0.0f, bouncerScale.x / 2);
            
        rightCircle.localScale = new Vector3(bouncerScale.y, bouncerScale.y , 1.0f);
        rightCircle.localPosition = new Vector3(0.0f, -bouncerScale.x / 2);
            
        middleSquare.localScale = new Vector3(bouncerScale.y - 0.005f, bouncerScale.x , 1.0f);

        //bounceCollider.size = new Vector2(bounceCollider.size.x, 1 + bouncerScale.x);
    }

    IEnumerator CollisionTimeoutTimer()
    {
        EnableCollision(false);
        canCollideWithBouncer = false;
        
        if (isSimulated)
            yield return new WaitForSeconds(simulatedCollisionTimeoutTime);
        else
            yield return new WaitForSeconds(collisionTimeoutTime);
        
        canCollideWithBouncer = true;
    }

    void ScaleAnimationCompleted()
    {
        isAnimationPlaying = false;
        EnableCollision(true);
    }

    public void PlayOnHitAnimations()
    {
        isAnimationStarted = true;
        isAnimationPlaying = true;

        Vector3 scale = new Vector3(0.35f, 0.35f, 0.35f);
        float duration = 0.1f;
        transform.DOPunchScale(scale, duration, 8, 1.0f).OnComplete(ScaleAnimationCompleted);
    }

    void EnableCollision(bool enable)
    {
        foreach (Collider2D baseCollider in baseColliders)
        {
            baseCollider.enabled = enable;
        }
        
        hitCollider.enabled = enable;
    }

    bool IsPlayerOverlapping()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        List<Collider2D> hitColliders = new List<Collider2D>();

        rb.OverlapCollider(contactFilter2D, hitColliders);

        if (hitColliders.Count == 0)
        {
            return false;
        }
        
        foreach (Collider2D col in hitColliders)
        {
            PlayerController playerController = col.GetComponentInParent<PlayerController>();
            if (!playerController)
            {
                continue;
            }

            return true;
        }

        return true;
    }
}
