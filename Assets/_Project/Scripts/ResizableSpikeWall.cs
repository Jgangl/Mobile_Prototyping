using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizableSpikeWall : MonoBehaviour
{
    [SerializeField] private Transform spikePrefab;

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Resizable spike wall start");
        Bounds mainBounds = GetComponent<SpriteRenderer>().bounds;

        Vector3 minBounds = mainBounds.min;
        Vector3 maxBounds = mainBounds.max;

        Vector3 xBoundsExtentsVector = new Vector3(mainBounds.extents.x, 0f, 0f);
        Vector3 yBoundsExtentsVector = new Vector3(0f, mainBounds.extents.y, 0f);
        
        Vector3 XOffsetVector = new Vector3(xOffset, 0f, 0f);
        Vector3 YOffsetVector = new Vector3(0f, yOffset, 0f);

        Vector3 leftMiddle = minBounds + new Vector3(0f, mainBounds.extents.y, 0f);
        Vector3 middleTop = mainBounds.center + new Vector3(0f, mainBounds.extents.y, 0f);
        Vector3 leftTop = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f);
        Vector3 rightTop = maxBounds;
        Vector3 rightMiddle = maxBounds - new Vector3(0f, mainBounds.extents.y, 0f);
        
        Vector3 topLeft = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(xOffset, -yOffset, 0f);
        Vector3 botLeft = minBounds + new Vector3(xOffset, yOffset, 0f);
        Vector3 topRight = maxBounds + new Vector3(-xOffset, -yOffset, 0f);
        Vector3 botRight = maxBounds - new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(-xOffset, yOffset, 0f);

        // Corners
        Instantiate(spikePrefab, topLeft, Quaternion.Euler(0f, 0f, 45f));
        Instantiate(spikePrefab, botLeft, Quaternion.Euler(0f, 0f, 45f + 90f));
        Instantiate(spikePrefab, topRight, Quaternion.Euler(0f, 0f, -45f));
        Instantiate(spikePrefab, botRight, Quaternion.Euler(0f, 0f, -45f - 90f));
        
        
        //Instantiate(spikePrefab, rightMiddle, Quaternion.Euler(0f, 0f, -90f));
        //Instantiate(spikePrefab, leftMiddle, Quaternion.Euler(0f, 0f, 90f));
        //Instantiate(spikePrefab, leftMiddle, Quaternion.Euler(0f, 0f, 90f));
        //Instantiate(spikePrefab, leftMiddle, Quaternion.Euler(0f, 0f, 90f));

        float spikeXSize = 0.08f + 0.02f;
        
        // Total Distance / num Objects
        float xLength = mainBounds.extents.x * 2f;
 
        int count = Mathf.FloorToInt(xLength / spikeXSize);
        if (count == 0) count = 1; //don't do this if you don't want it
        float n = xLength / (count + 1);
 
        for(int i = 0; i < count; i++)
        {
            Vector3 p = leftMiddle + Vector3.right * n * (i + 1);
            // Top
            Instantiate(spikePrefab, p + yBoundsExtentsVector - new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 0f));
            // Bottom
            Instantiate(spikePrefab, p - yBoundsExtentsVector + new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 180f));
        }

        // Total Distance / num Objects
        float yLength = mainBounds.extents.y * 2f;

        count = Mathf.FloorToInt(yLength / spikeXSize);
        if (count == 0) count = 1; //don't do this if you don't want it
        n = yLength / (count + 1);
 
        for(int i = 0; i < count; i++)
        {
            Vector3 p = middleTop + Vector3.down * n * (i + 1);
            // Left
            Instantiate(spikePrefab, p - xBoundsExtentsVector + new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 90f));
            // Right
            Instantiate(spikePrefab, p + xBoundsExtentsVector - new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, -90f));
        }
    }
}
