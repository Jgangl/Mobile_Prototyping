using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using Shapes;

public class BasicTrajectory : ImmediateModeShapeDrawer
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
    private PolylinePath polyPath;
    
    private List<float> lineWidths;

    [SerializeField] private int maxSteps = 10;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private LayerMask collisionLayers;

    //[SerializeField] private Polyline polyline;

    private List<Vector2> linePositions;

    private Camera mainCam;
    
    private void Awake()
    {
        polyPath = new PolylinePath();
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
        
        //trajectoryVisualLine = new VectorLine("Trajectory", new List<Vector2>(), lineTex, lineWidthStart, lineType);
        
        //trajectoryVisualLine.color = lineColor;
        //trajectoryVisualLine.textureScale = 1.0f;
        //trajectoryVisualLine.smoothWidth = true;
    }

    public void SimulateArc(Vector2 launchPosition, Vector2 directionVector, float velocity, float mass)
    {
        polyPath.ClearAllPoints();
        PolylinePoint polylinePoint = new PolylinePoint(launchPosition, Color.green, 1f);
        polyPath.AddPoint(polylinePoint);
        linePositions.Clear();
        linePositions.Add(launchPosition);
        float initialVelocity = velocity / mass * Time.fixedDeltaTime;

        for (int i = 1; i < maxSteps; i++)
        {
            Vector2 calculatedPosition = launchPosition + directionVector * (initialVelocity * i * timeStepInterval);
            calculatedPosition.y += (Physics2D.gravity.y * gravityScale) / 2 * Mathf.Pow(i * timeStepInterval, 2);

            Vector2 previousPos = linePositions[i - 1];
            Vector2 rayDirection = calculatedPosition - previousPos;
            
            PolylinePoint point = new PolylinePoint(calculatedPosition, Color.green, 1f);
            
            RaycastHit2D hit = Physics2D.Raycast(previousPos, rayDirection, rayDirection.magnitude, collisionLayers);
            if (hit.collider)
            {
                // Collided with something
                
                point = new PolylinePoint(hit.point, Color.green, 1f);
                linePositions.Add(hit.point);
                polyPath.AddPoint(point);
                
                break;
            }
            
            //Vector2 uiPoint = mainCam.WorldToScreenPoint(calculatedPosition);
            //trajectoryVisualLine.points2.Add(uiPoint);

            
            //polyline.points.Add(point);
            polyPath.AddPoint(point);
            
            linePositions.Add(calculatedPosition);
        }

        //trajectoryVisualLine.SetWidths(lineWidths);
        //trajectoryVisualLine.Draw();
        //polyline.points.Clear();
        //trajectoryVisualLine.points2.Clear();
    }

    public void ClearArc()
    {
        polyPath.ClearAllPoints();
        //polyline.points.Clear();
        linePositions.Clear();
        //trajectoryVisualLine.points2.Clear();
        //trajectoryVisualLine.Draw();
    }

    public override void DrawShapes(Camera cam)
    {
        using( Draw.Command( cam ) ){

            // set up static parameters. these are used for all following Draw.Line calls
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.ThicknessSpace = ThicknessSpace.Pixels;
            Draw.Thickness = 4; // 4px wide

            Draw.Polyline(polyPath,  false, 5f, Color.green);
            
            if (linePositions.Count > 0)
                Draw.Disc(linePositions[^1], Vector3.forward, 0.1f, DiscColors.Flat(Color.green));
        }
    }
}
