using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class BasicTrajectory : ImmediateModeShapeDrawer
{
    [SerializeField] float timeStepInterval = 0.1f;
    [SerializeField] float lineWidthStart = 13.0f;
    [SerializeField] float lineWidthEnd = 5.0f;
    [SerializeField] int maxSteps = 10;
    [SerializeField] float gravityScale = 2f;
    [SerializeField] LayerMask collisionLayers;

    [SerializeField] Color lineColor;
    [SerializeField] Color capColor;
    [SerializeField] float lineThickness = 0.1f;
    [SerializeField] float endCapRadius = 0.2f;
    [SerializeField] int numLineSkipPoints;

    List<Vector2> linePositions;

    void Awake()
    {
        linePositions = new List<Vector2>();
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
                linePositions.Add(hit.point);
                
                break;
            }

            linePositions.Add(calculatedPosition);
        }
    }

    public void ClearArc()
    {
        linePositions.Clear();
    }

    public override void DrawShapes(Camera cam)
    {
        using( Draw.Command( cam ) ){
            
            if (linePositions.Count == 0)
                return;
            
            Draw.BlendMode = ShapesBlendMode.Opaque;

            DrawPathPointDiscs();

            DrawFinalDisc();
        }
    }

    void DrawPathPointDiscs()
    {
        Draw.Color = lineColor;

        int currPointSkips = 0;
        for (int i = 0; i < linePositions.Count - 1; i++)
        {
            currPointSkips++;
            if (currPointSkips < numLineSkipPoints)
                continue;

            currPointSkips = 0;
            
            Vector3 point = linePositions[i];

            float distToEndCap = Vector2.Distance(point, linePositions[^1]);
                
            // Don't draw points too close to end cap
            if (distToEndCap < endCapRadius + lineThickness)
                break;
                
            point.z = point.z - 0.5f;
                
            Draw.Disc(point, Quaternion.identity, lineThickness);
        }
    }

    void DrawFinalDisc()
    {
        Draw.Color = capColor;

        Vector3 endCapPos = linePositions[^1];
        endCapPos.z += 0.5f;
            
        Draw.Disc(endCapPos, Quaternion.identity, endCapRadius);
    }
}
