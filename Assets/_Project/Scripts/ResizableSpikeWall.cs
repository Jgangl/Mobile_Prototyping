using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class ResizableSpikeWall : MonoBehaviour
{
    [SerializeField] Transform spikePrefab;

    [SerializeField] float xOffset;
    [SerializeField] float yOffset;

    [SerializeField] float xSpikeSpacing = 0.02f;
    [SerializeField] float ySpikeSpacing = 0.02f;
    [SerializeField] float leftRightSpikeYOffset = 0.02f;
    [SerializeField] float topBottomSpikeXOffset = 0.02f;

    public bool enableCornerSpikes = true;
    public bool enableTopSpikes = true;
    public bool enableBottomSpikes = true;
    public bool enableLeftSpikes = true;
    public bool enableRightSpikes = true;
    
    Vector2 prevSpriteRendererSize;

    List<Transform> spikes;

    SpriteRenderer sprite;

    Transform spikeParent;

    // Start is called before the first frame update
    void Start()
    {
        RegenerateSpikes();
    }

    void Update()
    {
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        Vector2 currentSpriteSize = sprite.size;

        if (currentSpriteSize != prevSpriteRendererSize)
        {
            RegenerateSpikes();
            
            prevSpriteRendererSize = currentSpriteSize;
        }
    }

    void DeleteAllSpikes()
    {
        if (spikes == null) return;
        
        foreach (Transform spike in spikeParent)
        {
            if (spike != null)
                DestroyImmediate(spike.gameObject);
        }
    }

#if UNITY_EDITOR
    [Button("Regenerate Spikes")]
#endif
    void RegenerateSpikes()
    {
#if UNITY_EDITOR
        // Don't do anything in Prefab mode
        if (PrefabStageUtility.GetCurrentPrefabStage())
            return;
#endif

        spikeParent = transform.Find("SpikeParent");

        if (spikeParent)
        {
            DestroyImmediate(spikeParent.gameObject);
        }
        
        
        spikeParent = new GameObject("SpikeParent").transform;
        spikeParent.transform.parent = transform;

        DeleteAllSpikes();

        spikes = new List<Transform>();

        Bounds mainBounds = GetComponent<SpriteRenderer>().bounds;

        Vector3 minBounds = mainBounds.min;
        Vector3 maxBounds = mainBounds.max;

        Vector3 xBoundsExtentsVector = new Vector3(mainBounds.extents.x, 0f, 0f);
        Vector3 yBoundsExtentsVector = new Vector3(0f, mainBounds.extents.y, 0f);

        Vector3 leftMiddle = minBounds + new Vector3(0f, mainBounds.extents.y, 0f);
        Vector3 middleTop = mainBounds.center + new Vector3(0f, mainBounds.extents.y, 0f);

        Vector3 topLeft = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(xOffset, -yOffset, 0f);
        Vector3 botLeft = minBounds + new Vector3(xOffset, yOffset, 0f);
        Vector3 topRight = maxBounds + new Vector3(-xOffset, -yOffset, 0f);
        Vector3 botRight = maxBounds - new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(-xOffset, yOffset, 0f);

        // Corners
        if (enableCornerSpikes)
        {
            Transform topLeftSpike = Instantiate(spikePrefab, topLeft, Quaternion.Euler(0f, 0f, 45f));
            Transform botLeftSpike = Instantiate(spikePrefab, botLeft, Quaternion.Euler(0f, 0f, 45f + 90f));
            Transform topRightSpike = Instantiate(spikePrefab, topRight, Quaternion.Euler(0f, 0f, -45f));
            Transform botRightSpike = Instantiate(spikePrefab, botRight, Quaternion.Euler(0f, 0f, -45f - 90f));
            
            // Corner opposites, (To look better)
            Transform topLeftOppSpike = Instantiate(spikePrefab, topLeft, Quaternion.Euler(0f, 0f, 45f + 180f));
            Transform botLeftOppSpike = Instantiate(spikePrefab, botLeft, Quaternion.Euler(0f, 0f, 45f + 90f + 180f));
            Transform topRightOppSpike = Instantiate(spikePrefab, topRight, Quaternion.Euler(0f, 0f, -45f + 180f));
            Transform botRightOppSpike = Instantiate(spikePrefab, botRight, Quaternion.Euler(0f, 0f, -45f - 90f + 180f));
            
            topLeftSpike.parent = spikeParent;
            botLeftSpike.parent = spikeParent;
            topRightSpike.parent = spikeParent;
            botRightSpike.parent = spikeParent;
        
            topLeftOppSpike.parent = spikeParent;
            botLeftOppSpike.parent = spikeParent;
            topRightOppSpike.parent = spikeParent;
            botRightOppSpike.parent = spikeParent;
        
            spikes.Add(topLeftSpike);
            spikes.Add(botLeftSpike);
            spikes.Add(topRightSpike);
            spikes.Add(botRightSpike);
        
            spikes.Add(topLeftOppSpike);
            spikes.Add(botLeftOppSpike);
            spikes.Add(topRightOppSpike);
            spikes.Add(botRightOppSpike);
        }

        float spikeXSize = spikePrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x + xSpikeSpacing;
        
        // Total Distance / num Objects
        float xLength = mainBounds.extents.x * 2f;
 
        int count = Mathf.FloorToInt(xLength / spikeXSize);
        if (count == 0) count = 1; //don't do this if you don't want it
        float n = xLength / (count + 1);
 
        for(int i = 0; i < count; i++)
        {
            Vector3 p = leftMiddle + Vector3.right * n * (i + 1);
            
            // Top
            if (enableTopSpikes)
            {
                Transform topSpike = Instantiate(spikePrefab, p + yBoundsExtentsVector - new Vector3(0f, leftRightSpikeYOffset), Quaternion.Euler(0f, 0f, 0f));
                topSpike.parent = spikeParent;
                
                spikes.Add(topSpike);
            }

            // Bottom
            if (enableBottomSpikes)
            {
                Transform botSpike = Instantiate(spikePrefab,
                    p - yBoundsExtentsVector + new Vector3(0f, leftRightSpikeYOffset), Quaternion.Euler(0f, 0f, 180f));
                botSpike.parent = spikeParent;

                spikes.Add(botSpike);
            }
        }

        // Total Distance / num Objects
        float yLength = mainBounds.extents.y * 2f;

        spikeXSize = spikePrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x + ySpikeSpacing;
        
        count = Mathf.FloorToInt(yLength / spikeXSize);
        if (count == 0) count = 1; //don't do this if you don't want it
        n = yLength / (count + 1);
 
        for(int i = 0; i < count; i++)
        {
            Vector3 p = middleTop + Vector3.down * n * (i + 1);
            // Left
            if (enableLeftSpikes)
            {
                Transform leftSpike = Instantiate(spikePrefab,
                    p - xBoundsExtentsVector + new Vector3(topBottomSpikeXOffset, 0f), Quaternion.Euler(0f, 0f, 90f));
                leftSpike.parent = spikeParent;

                spikes.Add(leftSpike);
            }

            // Right
            if (enableRightSpikes)
            {
                Transform rightSpike = Instantiate(spikePrefab,
                    p + xBoundsExtentsVector - new Vector3(topBottomSpikeXOffset, 0f), Quaternion.Euler(0f, 0f, -90f));
                rightSpike.parent = spikeParent;

                spikes.Add(rightSpike);
            }
        }
    }
}
