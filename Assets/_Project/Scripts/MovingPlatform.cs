using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Movement speed in units per second.
    [SerializeField]  float speed = 1.0F;

    // Time when the movement started.
    [SerializeField]  float startTime;

    // Total distance between the markers.
    float journeyLength;
    [SerializeField] Transform StartPoint;
    [SerializeField] Transform EndPoint;

    FixedJoint2D PlayerJoint;
    
    void Start()
    {
        PlayerJoint = GetComponent<FixedJoint2D>();
        
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(StartPoint.position, EndPoint.position);
    }

    // Move to the target end position.
    void Update()
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(StartPoint.position, EndPoint.position, fractionOfJourney);
    }

    public void EnableJoint()
    {
        PlayerJoint.enabled = true;
    }
    
    public void DisableJoint()
    {
        PlayerJoint.enabled = false;
    }
}
