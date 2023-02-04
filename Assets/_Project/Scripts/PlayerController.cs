using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    GameObject previousPlatform;
    bool canCollideWithPreviousPlatform = true;
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
    
    Dictionary<Transform, Vector2> bonePositions;
    List<Bone_Softbody> bones;
    List<Rigidbody2D> boneRigidbodies;
    bool bonesCanCollide = true;
    Dictionary<Bone_Softbody, List<Rigidbody2D>> boneCollisionDict;
    bool stuckToPlatform = false;
    bool isDead;
    MovingObject CurrentMovingObject;
    FixedJoint2D MovingJoint;

    void Start()
    {
        slimeGenerator = GetComponent<SlimeGenerator>();
        rb = GetComponent<Rigidbody2D>();
        basicTrajectory = GetComponent<BasicTrajectory>();

        MovingJoint = GetComponent<FixedJoint2D>();

        canMove = false;
        isDead = false;

        bonePositions = new Dictionary<Transform, Vector2>();
        SaveBonePositions();

        boneCollisionDict = new Dictionary<Bone_Softbody, List<Rigidbody2D>>();

        bones = new List<Bone_Softbody>();
        boneRigidbodies = new List<Rigidbody2D>();
        
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out Bone_Softbody bone))
            {
                bones.Add(bone);
                boneRigidbodies.Add(bone.GetRigidbody());
                boneCollisionDict.Add(bone, new List<Rigidbody2D>());
            }
        }
    }

    void Update() 
    {
        if (disableInput)
            return;
        
        Vector2 mousePosition = Input.mousePosition;
        bool mouseMoved = Vector2.Distance(prevFingerPos, mousePosition) >= 0.25f;
        if (canMove) 
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                fingerDownPos = Input.mousePosition;
                mouseHeldDown = true;
            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                mouseHeldDown = false;

                if (basicTrajectory)
                    basicTrajectory.ClearArc();
                
                if (Mathf.Abs(currentSwipeForce.x) >= 0.01f || Mathf.Abs(currentSwipeForce.y) >= 0.01f) 
                {
                    // Enable Movement
                    //Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
                    StartMovement();

                    // Add swipe force
                    foreach (Rigidbody2D bone in boneRigidbodies)
                        bone.AddForce(currentSwipeForce);
                }

                currentSwipeForce = Vector2.zero;
            }
            else if (mouseHeldDown && (mouseMoved || CurrentMovingObject != null)) 
            {
                fingerCurrentPos = Input.mousePosition;

                // Even in the Moved touchPhase, the finger wasn't actually 'moving' much
//                float fingerPosDiff = Vector2.Distance(prevFingerPos, fingerCurrentPos);
//                if (fingerPosDiff >= SWIPE_LENGTH_THRESHOLD)
//                {

                // Calculate current position difference
                Vector2 currSwipeDirection = (fingerCurrentPos - fingerDownPos).normalized;
                float currSwipeLength = Vector2.Distance(fingerCurrentPos, fingerDownPos);

                // Clamp swipe length
                if (currSwipeLength > maxSwipeLength)
                    currSwipeLength = maxSwipeLength;

                // Calculate force
                currentSwipeForce = (currSwipeDirection * -1) * speed * currSwipeLength * swipeLengthVariableGain *
                                    swipeLengthFlatGain;
                
                // Simulate launch
                if (basicTrajectory)
                {
                    basicTrajectory.SimulateArc(gameObject.transform.position, 
                        currentSwipeForce.normalized,
                        currentSwipeForce.magnitude,
                        1f);
                }
                
                //}
            }
        }

        Vector2 currentPosition = transform.position;
        prevPositionTwo = prevPosition;
        prevPosition = currentPosition;

#if UNITY_EDITOR
        prevFingerPos = Input.mousePosition;
#elif UNITY_ANDROID
        //prevFingerPos = Input.touches[0].position;
#endif
    }

    void StopMovement() 
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    public void StartMovement() 
    {
        StartCoroutine("IgnorePlatformTimer");
        StartCoroutine("IgnoreBonesTimer");

        stuckToPlatform = false;
        rb.isKinematic = false;
        canMove = false;
        
        DisableMovingJoint();

        foreach (Bone_Softbody bone in bones)
        {
            bone.SetRigidbodyIsKinematic(false);
        }
    }

    public void EnableMovement(bool newEnabled) 
    {
        disableInput = !newEnabled;
    }

    IEnumerator IgnorePlatformTimer() 
    {
        canCollideWithPreviousPlatform = false;
        yield return new WaitForSeconds(platformIgnoreTime);
        canCollideWithPreviousPlatform = true;
    }

    public Vector2 GetObjectAverageVelocity() 
    {
        Vector2 totalVelocity = Vector2.zero;

        // Get all rigidbodies of bones
        if (bones.Count == 0)
            return Vector2.zero;

        // Average all bone positions
        foreach (Rigidbody2D boneRb in boneRigidbodies) 
        {
            totalVelocity += boneRb.velocity;
        }

        return (totalVelocity / bones.Count);
    }

    public void OnChildCollisionEnter2D(Bone_Softbody bone, Collision2D collision) 
    {
        // Don't collide with other bones
        if (collision.gameObject.CompareTag("Player") || isDead)
            return;

        if (stuckToPlatform)
            return;

        float velocityMagnitude = GetObjectAverageVelocity().magnitude;
        if (velocityMagnitude > 1f && bonesCanCollide) 
        {
            AudioManager.Instance.PlaySquishSound();
            CinemachineShake.Instance.ShakeCamera(0.5f, 0.2f);
            
            slimeGenerator.Generate(collision.contacts[0].point, collision.gameObject);
            
            StartCoroutine("IgnoreBonesTimer");

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

        if (collision.gameObject.CompareTag("Platform") && canCollideWithPreviousPlatform && !stuckToPlatform) 
        {
            StopMovement();

            if (boneCollisionDict.ContainsKey(bone))
            {
                boneCollisionDict[bone].Add(collision.rigidbody);
            }

            stuckToPlatform = true;
        }
        else if (collision.gameObject.TryGetComponent(out MovingObject movingObject) && canCollideWithPreviousPlatform && !stuckToPlatform)
        {
            rb.velocity = Vector2.zero;
            canMove = true;
            
            EnableMovingJoint(movingObject.GetComponent<Rigidbody2D>());

            CurrentMovingObject = movingObject;

            if (boneCollisionDict.ContainsKey(bone))
            {
                boneCollisionDict[bone].Add(collision.rigidbody);
            }

            stuckToPlatform = true;
        }
    }

    public void OnChildCollisionStay2D(Bone_Softbody bone, Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (stuckToPlatform)
                return;
            
            if (bonesCanCollide)
            {
                StopMovement();
                stuckToPlatform = true;
            }
        }
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
                stuckToPlatform = false;
            }
        }
    }

    IEnumerator IgnoreBonesTimer() 
    {
        bonesCanCollide = false;
        yield return new WaitForSeconds(bonesCollisionTime);
        bonesCanCollide = true;
    }

    public void SetVelocity(Vector2 newVelocity) 
    {
        foreach(Rigidbody2D bone in boneRigidbodies) 
        {
            bone.velocity = newVelocity;
        }
    }

    public void SetIsDead(bool newIsDead)
    {
        this.isDead = newIsDead;
    }

    public void Reset(Vector2 newPosition)
    {
        SetIsDead(false);
        SetVelocity(Vector2.zero);
        
        StartMovement();

        transform.position = newPosition;

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
}
