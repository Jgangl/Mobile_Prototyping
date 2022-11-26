using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class BasicTrajectory : ImmediateModeShapeDrawer
{
    [SerializeField] float timeStepInterval = 0.1f;
    [SerializeField] float lineWidthStart = 13.0f;
    [SerializeField] float lineWidthEnd = 5.0f;
    [SerializeField] private int maxSteps = 10;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private LayerMask collisionLayers;

    private List<Vector2> linePositions;
    private PolylinePath polyPath;
    private List<float> lineWidths;

    private void Awake()
    {
        polyPath = new PolylinePath();
        linePositions = new List<Vector2>();
    }

    private void Start()
    {
        int lineWidthLength = maxSteps - 1;
        lineWidths = new List<float>(new float[lineWidthLength]);
        float stepSize = (lineWidthStart - lineWidthEnd) / lineWidthLength;
        
        for (int i = 0; i < lineWidths.Count; i++) {
            lineWidths[i] = Mathf.Clamp(lineWidthStart - (i * stepSize), lineWidthEnd, lineWidthStart);
        }
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

            polyPath.AddPoint(point);
            
            linePositions.Add(calculatedPosition);
        }
    }

    public void ClearArc()
    {
        polyPath.ClearAllPoints();
        linePositions.Clear();
    }

    public override void DrawShapes(Camera cam)
    {
        using( Draw.Command( cam ) ){

            // set up static parameters. these are used for all following Draw.Line calls
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.ThicknessSpace = ThicknessSpace.Pixels;
            Draw.Thickness = 4; // 4px wide

            Draw.Polyline(polyPath,  false, 5f, Color.green);
            
            // Draw circle at end of trajectory
            if (linePositions.Count > 0)
                Draw.Disc(linePositions[^1], Vector3.forward, 0.1f, DiscColors.Flat(Color.green));
        }
    }
}
