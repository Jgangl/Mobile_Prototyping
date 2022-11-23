using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class BasicTrajectory : Singleton<BasicTrajectory>
{
    
    //[SerializeField] float maxDuration = 1f;
    [SerializeField] float timeStepInterval = 0.1f;

    [SerializeField] Texture lineTex;
    [SerializeField] Color lineColor = Color.green;
    [SerializeField] LineType lineType = LineType.Continuous;
    [SerializeField] float lineWidthStart = 13.0f;
    [SerializeField] float lineWidthEnd = 5.0f;
    
    LineRenderer lineRenderer;
    VectorLine trajectoryVisualLine;
    
    private List<float> lineWidths;

    [SerializeField] private int maxSteps = 10;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private LayerMask collisionLayers;

    private List<Vector2> linePositions;

    private Camera mainCam;
    
    private void Awake()
    {
        linePositions = new List<Vector2>();
        mainCam = Camera.main;
        //Debug.Log("Basic Trajectory awake");
    }

    private void Start()
    {
        //int maxSteps = (int)(maxDuration / timeStepInterval);

        int lineWidthLength = maxSteps - 1;
        lineWidths = new List<float>(new float[lineWidthLength]);
        float stepSize = (lineWidthStart - lineWidthEnd) / lineWidthLength;
        
        for (int i = 0; i < lineWidths.Count; i++) {
            lineWidths[i] = Mathf.Clamp(lineWidthStart - (i * stepSize), lineWidthEnd, lineWidthStart);
        }
        
        trajectoryVisualLine = new VectorLine("Trajectory", new List<Vector2>(), lineTex, lineWidthStart, lineType);
        
        trajectoryVisualLine.color = lineColor;
        trajectoryVisualLine.textureScale = 1.0f;
        trajectoryVisualLine.smoothWidth = true;
    }

    public void SimulateArc(Vector2 launchPosition, Vector2 directionVector, float velocity, float mass)
    {
        linePositions.Clear();
        linePositions.Add(launchPosition);
        float initialVelocity = velocity / mass * Time.fixedDeltaTime;

        for (int i = 1; i < maxSteps; i++)
        {
            Vector2 calculatedPosition = launchPosition + directionVector * (initialVelocity * i * timeStepInterval);
            calculatedPosition.y += (Physics2D.gravity.y * gravityScale) / 2 * Mathf.Pow(i * timeStepInterval, 2);

            Vector2 previousPos = linePositions[i - 1];
            Vector2 rayDirection = calculatedPosition - previousPos;
            
            RaycastHit2D hit = Physics2D.Raycast(previousPos, rayDirection, rayDirection.magnitude, collisionLayers);
            if (hit.collider)
            {
                // Collided with something
                break;
            }
            
            Vector2 uiPoint = mainCam.WorldToScreenPoint(calculatedPosition);
            trajectoryVisualLine.points2.Add(uiPoint);



            linePositions.Add(calculatedPosition);
        }
        
        //trajectoryVisualLine.SetWidths(lineWidths);
        //trajectoryVisualLine.Draw();
        trajectoryVisualLine.points2.Clear();
    }

    public void ClearArc()
    {
        linePositions.Clear();
        trajectoryVisualLine.points2.Clear();
        //trajectoryVisualLine.Draw();
    }
}
