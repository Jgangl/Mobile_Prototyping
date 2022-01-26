using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class BasicTrajectory : MonoBehaviour
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

    [SerializeField] private int maxSteps = 15;

    private Camera mainCam;
    
    private void Awake()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        mainCam = Camera.main;
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

    public void SimulateArc(Vector2 position, Vector2 directionVector, Vector2 force, float mass, float gravityScale)
    {
        //if (trajectoryVisualLine.points2.Count > 0)
            //trajectoryVisualLine.points2.Clear();
        
        Vector3[] lineRendererPoints = new Vector3[maxSteps];

        Vector2 initialVelocity = force / mass * Time.fixedDeltaTime;

        for (int i = 0; i < maxSteps; i++)
        {
            Vector2 calculatedPosition = position + directionVector * initialVelocity * i * timeStepInterval;
            calculatedPosition.y += Physics2D.gravity.y * gravityScale / 2 * Mathf.Pow(i * timeStepInterval, 2);

            Vector2 uiPoint = mainCam.WorldToScreenPoint(calculatedPosition);
            trajectoryVisualLine.points2.Add(uiPoint);
            
            // if (CheckForCollision(calculatedPosition))
            //     break;
        }
        
        trajectoryVisualLine.SetWidths(lineWidths);
        trajectoryVisualLine.Draw();
        trajectoryVisualLine.points2.Clear();
    }

    public void ClearArc()
    {
        trajectoryVisualLine.points2.Clear();
        trajectoryVisualLine.Draw();
        //lineRenderer.SetPositions(new Vector3[0]);
    }
}
