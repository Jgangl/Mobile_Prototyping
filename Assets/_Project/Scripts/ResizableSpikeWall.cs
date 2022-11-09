using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizableSpikeWall : MonoBehaviour
{
    [SerializeField] private Transform spikePrefab;

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    private Vector2 prevSpriteRendererSize;

    private bool prevLeftMouseButtonDown = false;

    private List<Transform> spikes = new List<Transform>();

    private SpriteRenderer sprite;

    private void Awake()
    {
        Debug.Log("AWAKE");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("START");
        spikes = new List<Transform>();
        Bounds mainBounds = GetComponent<SpriteRenderer>().bounds;

        Vector3 minBounds = mainBounds.min;
        Vector3 maxBounds = mainBounds.max;

        Vector3 xBoundsExtentsVector = new Vector3(mainBounds.extents.x, 0f, 0f);
        Vector3 yBoundsExtentsVector = new Vector3(0f, mainBounds.extents.y, 0f);
        
        //Vector3 XOffsetVector = new Vector3(xOffset, 0f, 0f);
        //Vector3 YOffsetVector = new Vector3(0f, yOffset, 0f);

        Vector3 leftMiddle = minBounds + new Vector3(0f, mainBounds.extents.y, 0f);
        Vector3 middleTop = mainBounds.center + new Vector3(0f, mainBounds.extents.y, 0f);
        //Vector3 leftTop = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f);
        //Vector3 rightTop = maxBounds;
        //Vector3 rightMiddle = maxBounds - new Vector3(0f, mainBounds.extents.y, 0f);
        
        Vector3 topLeft = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(xOffset, -yOffset, 0f);
        Vector3 botLeft = minBounds + new Vector3(xOffset, yOffset, 0f);
        Vector3 topRight = maxBounds + new Vector3(-xOffset, -yOffset, 0f);
        Vector3 botRight = maxBounds - new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(-xOffset, yOffset, 0f);

        // Corners
        Transform topLeftSpike = Instantiate(spikePrefab, topLeft, Quaternion.Euler(0f, 0f, 45f));
        Transform botLeftSpike = Instantiate(spikePrefab, botLeft, Quaternion.Euler(0f, 0f, 45f + 90f));
        Transform topRightSpike = Instantiate(spikePrefab, topRight, Quaternion.Euler(0f, 0f, -45f));
        Transform botRightSpike = Instantiate(spikePrefab, botRight, Quaternion.Euler(0f, 0f, -45f - 90f));

        topLeftSpike.parent = gameObject.transform;
        botLeftSpike.parent = gameObject.transform;
        topRightSpike.parent = gameObject.transform;
        botRightSpike.parent = gameObject.transform;
        
        spikes.Add(topLeftSpike);
        spikes.Add(botLeftSpike);
        spikes.Add(topRightSpike);
        spikes.Add(botRightSpike);

        float spikeXSize = spikePrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x + 0.02f;
        
        // Total Distance / num Objects
        float xLength = mainBounds.extents.x * 2f;
 
        int count = Mathf.FloorToInt(xLength / spikeXSize);
        if (count == 0) count = 1; //don't do this if you don't want it
        float n = xLength / (count + 1);
 
        for(int i = 0; i < count; i++)
        {
            Vector3 p = leftMiddle + Vector3.right * n * (i + 1);
            // Top
            Transform topSpike = Instantiate(spikePrefab, p + yBoundsExtentsVector - new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 0f));
            topSpike.parent = gameObject.transform;
            // Bottom
            Transform botSpike = Instantiate(spikePrefab, p - yBoundsExtentsVector + new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 180f));
            botSpike.parent = gameObject.transform;
            
            spikes.Add(topSpike);
            spikes.Add(botSpike);
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
            Transform leftSpike = Instantiate(spikePrefab, p - xBoundsExtentsVector + new Vector3(0.02f, 0f), Quaternion.Euler(0f, 0f, 90f));
            leftSpike.parent = gameObject.transform;
            // Right
            Transform rightSpike = Instantiate(spikePrefab, p + xBoundsExtentsVector - new Vector3(0.02f, 0f), Quaternion.Euler(0f, 0f, -90f));
            rightSpike.parent = gameObject.transform;
            
            spikes.Add(leftSpike);
            spikes.Add(rightSpike);
        }
    }

    private void Update()
    {
        //bool leftMouseButtonDown = Input.GetMouseButtonDown(0);


        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        Vector2 currentSpriteSize = sprite.size;
        
        if (currentSpriteSize != prevSpriteRendererSize)
        {
            // Scale changed
            Debug.Log("Scale changed release!");
            prevSpriteRendererSize = currentSpriteSize;

            DeleteAllSpikes();

            if (spikes == null)
            {
                spikes = new List<Transform>();
            }
            
            Bounds mainBounds = GetComponent<SpriteRenderer>().bounds;

            Vector3 minBounds = mainBounds.min;
            Vector3 maxBounds = mainBounds.max;

            Vector3 xBoundsExtentsVector = new Vector3(mainBounds.extents.x, 0f, 0f);
            Vector3 yBoundsExtentsVector = new Vector3(0f, mainBounds.extents.y, 0f);
            
            //Vector3 XOffsetVector = new Vector3(xOffset, 0f, 0f);
            //Vector3 YOffsetVector = new Vector3(0f, yOffset, 0f);

            Vector3 leftMiddle = minBounds + new Vector3(0f, mainBounds.extents.y, 0f);
            Vector3 middleTop = mainBounds.center + new Vector3(0f, mainBounds.extents.y, 0f);
            //Vector3 leftTop = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f);
            //Vector3 rightTop = maxBounds;
            //Vector3 rightMiddle = maxBounds - new Vector3(0f, mainBounds.extents.y, 0f);
            
            Vector3 topLeft = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(xOffset, -yOffset, 0f);
            Vector3 botLeft = minBounds + new Vector3(xOffset, yOffset, 0f);
            Vector3 topRight = maxBounds + new Vector3(-xOffset, -yOffset, 0f);
            Vector3 botRight = maxBounds - new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(-xOffset, yOffset, 0f);

            // Corners
            Transform topLeftSpike = Instantiate(spikePrefab, topLeft, Quaternion.Euler(0f, 0f, 45f));
            Transform botLeftSpike = Instantiate(spikePrefab, botLeft, Quaternion.Euler(0f, 0f, 45f + 90f));
            Transform topRightSpike = Instantiate(spikePrefab, topRight, Quaternion.Euler(0f, 0f, -45f));
            Transform botRightSpike = Instantiate(spikePrefab, botRight, Quaternion.Euler(0f, 0f, -45f - 90f));
            
            // Corner opposites, (To look better)
            Transform topLeftOppSpike = Instantiate(spikePrefab, topLeft, Quaternion.Euler(0f, 0f, 45f + 180f));
            Transform botLeftOppSpike = Instantiate(spikePrefab, botLeft, Quaternion.Euler(0f, 0f, 45f + 90f + 180f));
            Transform topRightOppSpike = Instantiate(spikePrefab, topRight, Quaternion.Euler(0f, 0f, -45f + 180f));
            Transform botRightOppSpike = Instantiate(spikePrefab, botRight, Quaternion.Euler(0f, 0f, -45f - 90f + 180f));

            topLeftSpike.parent = transform;
            botLeftSpike.parent = transform;
            topRightSpike.parent = transform;
            botRightSpike.parent = transform;
            
            topLeftOppSpike.parent = transform;
            botLeftOppSpike.parent = transform;
            topRightOppSpike.parent = transform;
            botRightOppSpike.parent = transform;
            
            spikes.Add(topLeftSpike);
            spikes.Add(botLeftSpike);
            spikes.Add(topRightSpike);
            spikes.Add(botRightSpike);
            
            spikes.Add(topLeftOppSpike);
            spikes.Add(botLeftOppSpike);
            spikes.Add(topRightOppSpike);
            spikes.Add(botRightOppSpike);

            float spikeXSize = spikePrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x + 0.02f;
            
            // Total Distance / num Objects
            float xLength = mainBounds.extents.x * 2f;
     
            int count = Mathf.FloorToInt(xLength / spikeXSize);
            if (count == 0) count = 1; //don't do this if you don't want it
            float n = xLength / (count + 1);
     
            for(int i = 0; i < count; i++)
            {
                Vector3 p = leftMiddle + Vector3.right * n * (i + 1);
                // Top
                Transform topSpike = Instantiate(spikePrefab, p + yBoundsExtentsVector - new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 0f));
                topSpike.parent = gameObject.transform;
                // Bottom
                Transform botSpike = Instantiate(spikePrefab, p - yBoundsExtentsVector + new Vector3(0f, 0.02f), Quaternion.Euler(0f, 0f, 180f));
                botSpike.parent = gameObject.transform;
                
                spikes.Add(topSpike);
                spikes.Add(botSpike);
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
                Transform leftSpike = Instantiate(spikePrefab, p - xBoundsExtentsVector + new Vector3(0.02f, 0f), Quaternion.Euler(0f, 0f, 90f));
                leftSpike.parent = gameObject.transform;
                // Right
                Transform rightSpike = Instantiate(spikePrefab, p + xBoundsExtentsVector - new Vector3(0.02f, 0f), Quaternion.Euler(0f, 0f, -90f));
                rightSpike.parent = gameObject.transform;
                
                spikes.Add(leftSpike);
                spikes.Add(rightSpike);
            }
        }

            //prevLeftMouseButtonDown = leftMouseButtonDown;
    }

    private void DeleteAllSpikes()
    {
        if (spikes == null) return;
        
        foreach (Transform spike in spikes)
        {
            if (spike != null)
                DestroyImmediate(spike.gameObject);
        }
    }
}