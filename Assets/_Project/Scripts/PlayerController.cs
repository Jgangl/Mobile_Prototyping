using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] const float SWIPE_LENGTH_THRESHOLD = 0.25f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float swipeLengthVariableGain = 1.0f;
    [SerializeField] float swipeLengthFlatGain = 0.01f;
    [SerializeField] float speed;
    [SerializeField] GameObject hitParticles;
    [SerializeField] bool canMove;
    [SerializeField] bool disableInput;
    [SerializeField] float bonesCollisionTime = 0.05f;
    [SerializeField] float platformIgnoreTime = 0.25f;
    [SerializeField] float maxSwipeLength     = 300f;
    [SerializeField] float playerRadius = 0.75f;
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] double recentCollisionTimeDiffThreshold = 0.2f;
    [SerializeField] float swipeForceThreshold = 20.0f;
    [SerializeField] ParticleSystem playerTrail;
    [SerializeField] ParticleSystem playerSlimeParticles;
    [SerializeField] GameObject leftEye;
    [SerializeField] GameObject rightEye;
    [SerializeField] GameObject mouth;

    GameObject previousPlatform;
    bool platformCollisionTimeout = true;
    bool mouseHeldDown;
    Rigidbody2D rb;
    BasicTrajectory basicTrajectory;
    SlimeGenerator slimeGenerator;
    
    Vector2 fingerDownPos;
    Vector2 fingerCurrentPos;
    Vector2 prevFingerPos;
    Vector2 prevPosition;
    Vector2 prevPositionTwo;
    Vector2 currentSwipeForce;
    float flightTime;
    float hitSpeedThreshold = 1f;

    Dictionary<Transform, Vector2> bonePositions;
    List<Bone_Softbody> bones;
    List<Rigidbody2D> boneRigidbodies;
    bool bonesCanCollide = true;
    Dictionary<Bone_Softbody, List<Rigidbody2D>> boneCollisionDict;
    bool stuckToPlatform = false;
    bool isDead;
    Platform CurrentPlatform;
    FixedJoint2D MovingJoint;

    Dictionary<GameObject, double> recentWallCollisions;

    Tweener leftEyePunch;
    Tweener rightEyePunch;
    Tweener mouthScale;
    Vector2 mouthOriginalScale;

    public static Action OnJumped;

    void Start()
    {
        slimeGenerator  = GetComponent<SlimeGenerator>();
        rb              = GetComponent<Rigidbody2D>();
        basicTrajectory = GetComponent<BasicTrajectory>();
        MovingJoint     = GetComponent<FixedJoint2D>();

        canMove = false;
        isDead  = false;

        bonePositions        = new Dictionary<Transform, Vector2>();
        boneCollisionDict    = new Dictionary<Bone_Softbody, List<Rigidbody2D>>();
        bones                = new List<Bone_Softbody>();
        boneRigidbodies      = new List<Rigidbody2D>();
        recentWallCollisions = new Dictionary<GameObject, double>();

        InitializeBones();

        mouthOriginalScale = mouth.transform.localScale;
    }

    void Update()
    {
        MoveEyesToDirection();
        
        UpdateRecentCollisions();
        
        if (disableInput)
            return;
        
        Vector2 mousePosition = Input.mousePosition;
        bool mouseMoved = Vector2.Distance(prevFingerPos, mousePosition) >= 0.25f;
        if (canMove) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Input.mousePosition;

                if (UI_Manager.Instance.IsPositionWithinLevelUI(mousePos))
                {
                    return;
                }
                
                fingerDownPos = Input.mousePosition;
                mouseHeldDown = true;
            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                mouseHeldDown = false;

                if (basicTrajectory)
                    basicTrajectory.ClearArc();

                if (Mathf.Abs(currentSwipeForce.x) >= swipeForceThreshold || Mathf.Abs(currentSwipeForce.y) >= swipeForceThreshold) 
                {
                    EnableMovement();

                    // Add swipe force
                    AddForce(currentSwipeForce);
                    
                    AudioManager.Instance.PlayLaunchSound();
                    
                    PlaySlimeParticles();

                    OnJumped?.Invoke();
                }

                currentSwipeForce = Vector2.zero;
            }
            else if (mouseHeldDown && (mouseMoved || CurrentPlatform != null)) 
            {
                fingerCurrentPos = Input.mousePosition;

                currentSwipeForce = CalculateSwipeForce(fingerDownPos, fingerCurrentPos);

                //Debug.Log("X: " + currentSwipeForce.x + "  Y: " + currentSwipeForce.y);
                
                if (Mathf.Abs(currentSwipeForce.x) >= swipeForceThreshold ||
                    Mathf.Abs(currentSwipeForce.y) >= swipeForceThreshold ||
                    Mathf.Abs(currentSwipeForce.x) + Mathf.Abs(currentSwipeForce.y) >= swipeForceThreshold)
                {
                    
                    // Simulate launch
                    SimulateTrajectory(currentSwipeForce);
                }
                else
                {
                    
                    //Debug.Log(currentSwipeForce.y);
                    //Debug.Log("Clear arc");
                    if (basicTrajectory)
                        basicTrajectory.ClearArc();
                }
            }
        }

        Vector2 currentPosition = transform.position;
        prevPositionTwo = prevPosition;
        prevPosition = currentPosition;

        prevFingerPos = Input.mousePosition;
    }

    public void AddForce(Vector2 force)
    {
        foreach (Rigidbody2D bone in boneRigidbodies)
        {
            bone.AddForce(force);
        }
    }

    public void PlaySlimeParticles()
    {
        playerSlimeParticles.Play();
    }

    public Vector2 CalculateSwipeForce(Vector2 startPos, Vector2 endPos)
    {
        // Calculate current position difference
        Vector2 currSwipeDirection = (endPos - startPos).normalized;
        float currSwipeLength = Vector2.Distance(endPos, startPos);

        // Clamp swipe length
        if (currSwipeLength > maxSwipeLength)
            currSwipeLength = maxSwipeLength;

        // Calculate force
        Vector2 swipeForce = (currSwipeDirection * -1) * speed * currSwipeLength * swipeLengthVariableGain *
                            swipeLengthFlatGain;

        return swipeForce;
    }

    public void SimulateTrajectory(Vector2 swipeForce)
    {
        if (basicTrajectory)
        {
            basicTrajectory.SimulateArc(gameObject.transform.position, 
                swipeForce.normalized,
                swipeForce.magnitude,
                rb.mass);
        }
    }
    
    void InitializeBones()
    {
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out Bone_Softbody bone))
            {
                bones.Add(bone);
                boneRigidbodies.Add(bone.GetRigidbody());
                boneCollisionDict.Add(bone, new List<Rigidbody2D>());
            }
        }
        
        boneRigidbodies.Add(rb);
        
        SaveBonePositions();
    }

    void StopMovement(bool enableKinematicRigidbody) 
    {
        if (enableKinematicRigidbody)
            rb.isKinematic = true;
        
        playerSlimeParticles.Stop();
        
        rb.velocity = Vector2.zero;
        canMove = true;

        mouth.transform.DOScale(mouthOriginalScale, 0.2f);
    }

    public void EnableMovement() 
    {
        StartCoroutine("IgnorePlatformTimer");
        StartCoroutine("IgnoreBonesTimer");

        stuckToPlatform = false;
        rb.isKinematic = false;
        canMove = false;

        SetVelocity(Vector2.zero);
        
        DisableMovingJoint();

        foreach (Bone_Softbody bone in bones)
        {
            bone.SetRigidbodyIsKinematic(false);
        }

        Vector3 addMouthScale = new Vector3(0.5f, 0.1f, 0.0f);
        Vector2 targetScale = mouth.transform.localScale + addMouthScale;
        float duration = 0.2f;
        mouth.transform.DOScale(targetScale, duration);
    }

    public void EnableInput(bool newEnabled) 
    {
        disableInput = !newEnabled;
    }

    IEnumerator IgnorePlatformTimer() 
    {
        platformCollisionTimeout = false;
        yield return new WaitForSeconds(platformIgnoreTime);
        platformCollisionTimeout = true;
    }

    public Vector2 GetObjectAverageVelocity() 
    {
        Vector2 totalVelocity = Vector2.zero;

        if (bones == null || bones.Count == 0 || boneRigidbodies == null)
            return Vector2.zero;

        // Average all bone positions
        foreach (Rigidbody2D boneRb in boneRigidbodies) 
        {
            totalVelocity += boneRb.velocity;
        }

        return totalVelocity / bones.Count;
    }

    public void OnChildCollisionEnter2D(Bone_Softbody bone, Collision2D collision) 
    {
        // Don't collide with other bones
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || isDead)
            return;
        
        bool isHazard = collision.gameObject.GetComponentInParent<Hazard>() != null;

        if (isHazard)
        {
            isDead = true;
            GameManager.Instance.PlayerDied();
        }

        if (stuckToPlatform && !isHazard)
            return;

        Platform platformHit;
        bool bHitPlatform = collision.gameObject.TryGetComponent(out platformHit);
        if (!bHitPlatform)
        {
            // Need to check if the Platform component is in the parent gameobject
            Transform collisionParent = collision.transform.parent;
            if (collisionParent != null)
            {
                bHitPlatform = collisionParent.gameObject.TryGetComponent(out platformHit);
            }
        }
        
        bool bCanCollideWithWall = !recentWallCollisions.ContainsKey(collision.gameObject);

        if (!bHitPlatform)
        {
            if (!recentWallCollisions.ContainsKey(collision.gameObject))
            {
                recentWallCollisions.Add(collision.gameObject, Time.time); 
            }
        }
        
        bool bHitPlatformIsCurrent = CurrentPlatform == platformHit;

        bool speedThresholdPassed = GetObjectAverageVelocity().magnitude > hitSpeedThreshold;

        // If hit a platform OR didn't hit a platform but going fast enough
        if (speedThresholdPassed && bCanCollideWithWall && !bHitPlatform)
        {
            PlayHitEffects(collision);
        }

        if (!bHitPlatform)
            return;

        // Platform collision hasn't timed out and hit platform is same as current
        if (!platformCollisionTimeout && bHitPlatformIsCurrent)
            return;

        CurrentPlatform = platformHit;
        
        PlayHitEffects(collision);
        
        if (platformHit.GetComponent<MovingObject>())
        {
            StopMovement(false);
            EnableMovingJoint(platformHit.GetComponent<Rigidbody2D>());
        }
        else
        {
            StopMovement(true);
        }
        
        if (boneCollisionDict.ContainsKey(bone))
        {
            boneCollisionDict[bone].Add(collision.rigidbody);
        }

        stuckToPlatform = true;
    }

    public void OnChildCollisionStay2D(Bone_Softbody bone, Collision2D collision) 
    {
        // Fixes sliding across platform
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || isDead)
            return;

        if (stuckToPlatform)
            return;

        Platform platformHit;
        bool bHitPlatform = collision.gameObject.TryGetComponent(out platformHit);
        if (!bHitPlatform)
        {
            Transform collisionParent = collision.transform.parent;
            if (collisionParent != null)
            {
                bHitPlatform = collisionParent.gameObject.TryGetComponent(out platformHit);
            }
        }

        bool bHitPlatformIsCurrent = CurrentPlatform == platformHit;

        // If timer timed out and hit the same platform 
        if (bonesCanCollide && bHitPlatformIsCurrent)
        {
            if (platformHit.GetComponent<MovingObject>())
            {
                StopMovement(false);
                EnableMovingJoint(platformHit.GetComponent<Rigidbody2D>());
            }
            else
            {
                StopMovement(true);
            }
            
            PlayHitEffects(collision);
            stuckToPlatform = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, playerRadius);
    }

    public void OnChildCollisionExit2D(Bone_Softbody bone, Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Platform")) 
        {
            if (boneCollisionDict.ContainsKey(bone))
            {
                if (boneCollisionDict[bone].Contains(collision.rigidbody))
                {
                    boneCollisionDict[bone].Remove(collision.rigidbody);
                }
            }

            bool areThereCollisions = false;
            foreach ((Bone_Softbody currBone, List<Rigidbody2D> collisions) in boneCollisionDict)
            {
                if (collisions.Count > 0)
                {
                    areThereCollisions = true;
                    break;
                }
            }

            if (!areThereCollisions)
            {
                //stuckToPlatform = false;
            }
        }
    }

    void PlayHitEffects(Collision2D collision)
    {
        
        // Only play sound/screen shake during the level
        if (!Level_Manager.Instance.IsLevelCompleted() && !Level_Manager.Instance.LevelJustStarted())
        {
            AudioManager.Instance.PlaySquishSound();
            CinemachineShake.Instance.ShakeCamera(0.5f, 0.2f);
        }
        
        Blink();

        slimeGenerator.Generate(collision.contacts[0].point, collision.gameObject);
        SpawnHitParticles(collision);
    }

    void UpdateRecentCollisions()
    {
        List<GameObject> collisionsToRemove = new List<GameObject>();
        
        double currentTime = Time.time;
        foreach ((GameObject collision, double time) in recentWallCollisions)
        {
            double timeDiff = currentTime - time;
            if (timeDiff > recentCollisionTimeDiffThreshold)
            {
                collisionsToRemove.Add(collision);
            }
        }
        
        foreach (GameObject collision in collisionsToRemove)
        {
            recentWallCollisions.Remove(collision);
        }
    }
    
    void SpawnHitParticles(Collision2D collision)
    {
        //  CALCULATE PARTICLES ROTATION USING DIRECTION OF TRAVEL
        Vector2 currentPosition = transform.position;
        Vector2 dirOfTravel = (currentPosition - prevPositionTwo).normalized;

        // Find angle between x axis and direction of travel
        float angle = Mathf.Atan2(dirOfTravel.y, dirOfTravel.x) * Mathf.Rad2Deg;

        // Rotating the angle around an axis (Similar to applying the rotation to a specfic axis)
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector2 colPoint = collision.contacts[0].point;

        // Create particles and apply rotation
        GameObject particles = Instantiate(hitParticles, colPoint, Quaternion.identity);
        particles.transform.rotation = q;
        Destroy(particles, 0.5f);
    }

    IEnumerator IgnoreBonesTimer() 
    {
        bonesCanCollide = false;
        yield return new WaitForSeconds(bonesCollisionTime);
        bonesCanCollide = true;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        if (rb.isKinematic)
            return;
        
        foreach(Rigidbody2D bone in boneRigidbodies) 
        {
            bone.velocity = newVelocity;
        }
    }
    
    public void AddVelocity(Vector2 velocityToAdd) 
    {
        foreach(Rigidbody2D bone in boneRigidbodies) 
        {
            bone.velocity += velocityToAdd;
        }
    }
    
    public void AddForce(Vector2 forceToAdd, ForceMode2D forceMode) 
    {
        foreach(Rigidbody2D bone in boneRigidbodies)
        {
            bone.AddForce(forceToAdd, forceMode);
        }
    }

    public void SetIsDead(bool newIsDead)
    {
        isDead = newIsDead;
    }

    public void Reset(Vector2 newPosition)
    {
        SetIsDead(false);
        SetVelocity(Vector2.zero);
        
        StopMovement(true);

        playerTrail.Pause();
        transform.position = newPosition;
        playerTrail.Play();
        ResetBonePositions();
    }

    void SaveBonePositions()
    {
        bonePositions.Clear();
        
        foreach (Transform bone in transform)
        {
            if (bone.GetComponent<Rigidbody2D>())
            {
                bonePositions.Add(bone, bone.localPosition);
            }
        }
    }

    void ResetBonePositions()
    {
        foreach ((Transform bone, Vector2 bonePos) in bonePositions)
        {
            bone.localPosition = bonePos;
        }
    }

    void EnableMovingJoint(Rigidbody2D connectedRigidbody)
    {
        MovingJoint.enabled = true;
        MovingJoint.connectedBody = connectedRigidbody;
        MovingJoint.autoConfigureConnectedAnchor = false;
    }
    
    void DisableMovingJoint()
    {
        MovingJoint.connectedBody = null;
        MovingJoint.autoConfigureConnectedAnchor = true;
        MovingJoint.enabled = false;
    }

    void Blink()
    {
        if (rightEyePunch != null && rightEyePunch.active)
        {
            return;
        }
        
        if (leftEyePunch != null && leftEyePunch.active)
        {
            return;
        }
        
        
        //Debug.Log("Blink");
        Vector2 targetScale = new Vector2(-1.0f, 0.1f);
        float duration = 0.25f;
        int vibrato = 1;
        float elasticity = 1.0f;
        
        leftEyePunch = leftEye.transform.DOPunchScale(targetScale, duration, vibrato, elasticity);
        targetScale = new Vector2(-1.0f, 0.1f);
        rightEyePunch = rightEye.transform.DOPunchScale(targetScale, duration, vibrato, elasticity);
    }

    void MoveEyesToDirection()
    {
        if (stuckToPlatform)
            return;

        // Don't try to move eyes if scale animation is active
        if (rightEyePunch != null && rightEyePunch.active)
        {
            return;
        }
        
        if (leftEyePunch != null && leftEyePunch.active)
        {
            return;
        }
        
        // Right Eye

        Collider2D eyeBoundsCollider = rightEye.GetComponent<Collider2D>();
        Vector2 velocity = GetObjectAverageVelocity().normalized * 20.0f;
        Debug.DrawRay(transform.position, velocity, Color.red, .01f);
        

        //Vector2 leftVel = leftEye.transform.InverseTransformDirection(velocity);
        //Vector2 rightVel = rightEye.transform.InverseTransformDirection(velocity);
        
        
        Vector2 targetPupilPosition = eyeBoundsCollider.ClosestPoint(velocity);
        Transform rightPupil = rightEye.transform.Find("EyePupil").transform;
        Vector2 currRightPupilPosition = rightPupil.transform.position;

        rightPupil.position = Vector2.Lerp(currRightPupilPosition, targetPupilPosition, Time.deltaTime * 10.0f);

        
        // Left Eye
        eyeBoundsCollider = leftEye.GetComponent<Collider2D>();
        targetPupilPosition = eyeBoundsCollider.ClosestPoint(velocity);
        Transform leftPupil = leftEye.transform.Find("EyePupil").transform;
        Vector2 currLeftPupilPosition = leftPupil.transform.position;

        leftPupil.position = Vector2.Lerp(currLeftPupilPosition, targetPupilPosition, Time.deltaTime * 10.0f);

    }
}
