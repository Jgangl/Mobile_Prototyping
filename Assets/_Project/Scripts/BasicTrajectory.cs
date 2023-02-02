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

    [SerializeField] private Color lineColor;
    [SerializeField] private Color capColor;
    [SerializeField] private float lineThickness = 0.1f;
    [SerializeField] private float endCapRadius = 0.2f;
    [SerializeField] private int numLineSkipPoints;

    [SerializeField] private Polyline polyline;
    [SerializeField] private Disc endOfTrajectoryDisc;

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

        polyline = FindObjectOfType<Polyline>();
        endOfTrajectoryDisc = FindObjectOfType<Disc>();

        endOfTrajectoryDisc.Color = capColor;
    }

    public void SimulateArc(Vector2 launchPosition, Vector2 directionVector, float velocity, float mass)
    {
        //endOfTrajectoryDisc.enabled = true;
        //polyline.points.Clear();
        polyPath.ClearAllPoints();
        
        PolylinePoint polylinePoint = new PolylinePoint(launchPosition, lineColor, 1f);

        //polyline.points.Add(polylinePoint);
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
            
            PolylinePoint point = new PolylinePoint(calculatedPosition, lineColor, 1f);
            
            RaycastHit2D hit = Physics2D.Raycast(previousPos, rayDirection, rayDirection.magnitude, collisionLayers);
            if (hit.collider)
            {
                // Collided with something
                
                point = new PolylinePoint(hit.point, lineColor, 1f);
                linePositions.Add(hit.point);
                
                //polyline.points.Add(point);
                polyPath.AddPoint(point);
                
                break;
            }

            //polyline.points.Add(point);
            polyPath.AddPoint(point);
            
            linePositions.Add(calculatedPosition);
        }
        
        if (linePositions.Count > 0)
            endOfTrajectoryDisc.transform.localPosition = linePositions[^1];

        //polyline.meshOutOfDate = true;
    }

    public void ClearArc()
    {
        polyline.points.Clear();
        polyPath.ClearAllPoints();
        linePositions.Clear();

        endOfTrajectoryDisc.enabled = false;
        
        polyline.meshOutOfDate = true;
    }

    public override void DrawShapes(Camera cam)
    {
        using( Draw.Command( cam ) ){
            
            if (linePositions.Count == 0)
                return;
            
            
            //Draw.Thickness = lineThickness;
            Draw.Color = lineColor;
            Draw.BlendMode = ShapesBlendMode.Opaque;
            
            int currPointSkips = 0;
            for (int i = 0; i < linePositions.Count - 1; i++)
            {
                currPointSkips++;
                if (currPointSkips < numLineSkipPoints)
                    continue;
                
                
                
                currPointSkips = 0;
                
                // Draw point
                Vector3 point = linePositions[i];

                float distToEndCap = Vector2.Distance(point, linePositions[^1]);
                
                // Don't draw points too close to end cap
                if (distToEndCap < endCapRadius + lineThickness)
                    break;
                
                point.z = point.z - 0.5f;
                
                Draw.Disc(point, Quaternion.identity, lineThickness);
            }

            Draw.Color = capColor;

            Vector3 endCapPos = linePositions[^1];
            endCapPos.z += 0.5f;
            
            Draw.Disc(endCapPos, Quaternion.identity, endCapRadius);

            /*
            // set up static parameters. these are used for all following Draw.Line calls
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.ThicknessSpace = ThicknessSpace.Pixels;
            Draw.Thickness = 4; // 4px wide

            Draw.Polyline(polyPath,  false, 5f, Color.green);
            
            // Draw circle at end of trajectory
            if (linePositions.Count > 0)
                Draw.Disc(linePositions[^1], Vector3.forward, 0.1f, DiscColors.Flat(Color.green));
            */
        }
    }
}
