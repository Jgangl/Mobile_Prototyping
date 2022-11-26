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
    [SerializeField] private BasicTrajectory basicTrajectory;
    
    public float squishSoundTime = 0.25f;
    //private bool canPlaySquishSound = true;

    public bool bonesCanCollide = true;
    public float bonesCollisionTime = 0.05f;

    public GameObject hitParticles;

    private Vector2 prevPosition;
    private Vector2 prevPositionTwo;

    public bool isSimulated = false;
    private bool disableInput = false;

    private bool isDead;

    private SlimeGenerator slimeGenerator;

    private Dictionary<Transform, Vector2> bonePositions;

    private List<Bone_Softbody> bones;

    private Dictionary<Bone_Softbody, List<Rigidbody2D>> boneCollisionDict; 

    private bool stuckToPlatform = false;

    void Start()
    {
        slimeGenerator = GetComponent<SlimeGenerator>();
        rb = GetComponent<Rigidbody2D>();
        basicTrajectory = GetComponent<BasicTrajectory>();

        canMove = false;
        isDead = false;

        bonePositions = new Dictionary<Transform, Vector2>();
        SaveBonePositions();

        boneCollisionDict = new Dictionary<Bone_Softbody, List<Rigidbody2D>>();

        bones = new List<Bone_Softbody>();
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out Bone_Softbody bone))
            {
                bones.Add(bone);
                boneCollisionDict.Add(bone, new List<Rigidbody2D>());
            }
        }
    }

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
                if (trajectoryPredictor)
                    trajectoryPredictor.ClearSimulation();
                
                if (basicTrajectory)
                    basicTrajectory.ClearArc();
                
                if (Mathf.Abs(currentSwipeForce.x) >= 0.01f || Mathf.Abs(currentSwipeForce.y) >= 0.01f) {
                    // Enable Movement
                    Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
                    StartMovement();

                    // Add swipe force
                    foreach (Rigidbody2D bone in bones)
                        bone.AddForce(currentSwipeForce);
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
                    // Simulate launch
                    if (trajectoryPredictor)
                        trajectoryPredictor.SimulateLaunch(gameObject.transform, currentSwipeForce);

                    if (basicTrajectory)
                    {
                        basicTrajectory.SimulateArc(gameObject.transform.position, 
                            currentSwipeForce.normalized,
                            currentSwipeForce.magnitude,
                            1f);
                    }
                }
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

    private void StopMovement(Rigidbody2D rb) {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    public void StartMovement() {
        StartCoroutine("IgnorePlatformTimer");
        StartCoroutine("IgnoreBonesTimer");

        stuckToPlatform = false;
        rb.isKinematic = false;
        canMove = false;

        foreach (Bone_Softbody bone in bones)
        {
            bone.SetRigidbodyIsKinematic(false);
        }
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

    public void OnChildCollisionEnter2D(Bone_Softbody bone, Collision2D collision) {
        // Don't collide with other bones
        if (collision.gameObject.tag == "Player" || isDead)
            return;

        if (stuckToPlatform)
            return;

        float velocityMagnitude = GetObjectAverageVelocity().magnitude;
        if (velocityMagnitude > 1f && bonesCanCollide) {
            AudioManager.Instance.PlaySquishSound();
            CinemachineShake.Instance.ShakeCamera(0.5f, 0.2f);
            
            slimeGenerator.Generate(collision.contacts[0].point);
            
            //StartCoroutine("SquishSoundTimer");
            //StartCoroutine("IgnoreBonesTimer");

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

        if (collision.gameObject.tag == "Platform" && canCollideWithPreviousPlatform && !stuckToPlatform) 
        {
            StopMovement(rb);

            if (boneCollisionDict.ContainsKey(bone))
            {
                boneCollisionDict[bone].Add(collision.rigidbody);
            }

            stuckToPlatform = true;
        }
    }

    public void OnChildCollisionStay2D(Bone_Softbody bone, Collision2D collision) {
        if (collision.gameObject.tag == "Platform")
        {
            if (stuckToPlatform)
                return;
            
            if (bonesCanCollide)
            {
                StopMovement(rb);
                stuckToPlatform = true;
            }
        }
    }

    public void OnChildCollisionExit2D(Bone_Softbody bone, Collision2D collision) {
        if (collision.gameObject.tag == "Platform") 
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
        Rigidbody2D[] bones = GetComponentsInChildren<Rigidbody2D>();
        foreach(Rigidbody2D bone in bones) {
            bone.velocity = newVelocity;
        }
    }

    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
    }

    public void Reset(Vector2 newPosition)
    {
        SetIsDead(false);
        SetVelocity(Vector2.zero);

        transform.position = newPosition;

        ResetBonePositions();
    }

    private void SaveBonePositions()
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

    private void ResetBonePositions()
    {
        foreach ((Transform bone, Vector2 bonePos) in bonePositions)
        {
            bone.localPosition = bonePos;
        }
    }
}
