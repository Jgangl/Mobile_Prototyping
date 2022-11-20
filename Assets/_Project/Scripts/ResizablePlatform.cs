using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizablePlatform : MonoBehaviour
{
    [SerializeField] private GameObject cornerTopLeft;
    [SerializeField] private GameObject cornerBottomLeft;
    [SerializeField] private GameObject cornerTopRight;
    [SerializeField] private GameObject cornerBottomRight;
    [SerializeField] private GameObject topBottomRect;
    [SerializeField] private GameObject leftRightRect;
    
    private Vector2 prevSpriteRendererSize;
    private SpriteRenderer sprite;

    private float xScaleOffset = 0.02f;
    private float cornerCircleScale = 0.475f;
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        Vector3 scale = transform.localScale;

        // (xScale * spriteRendererXWidth) / 2 + (circleXScale / 2)
        // (yScale * spriteRendererYWidth) / 2 + (circleYScale / 2)
        Vector2 spriteSize = sprite.size;
        float localXPosLeft = -(scale.x * spriteSize.x) / 2 + ((cornerCircleScale - 0.005f) / 2);
        float localXPosRight = (scale.x * spriteSize.x) / 2 - ((cornerCircleScale - 0.005f) / 2);
        float localYPosTop = (scale.y * spriteSize.y) / 2 - ((cornerCircleScale - 0.005f) / 2);
        float localYPosBottom = -(scale.y * spriteSize.y) / 2 + ((cornerCircleScale - 0.005f) / 2);


        cornerTopLeft.transform.localPosition     = new Vector3(localXPosLeft, localYPosTop, 0f);
        cornerBottomLeft.transform.localPosition  = new Vector3(localXPosLeft, localYPosBottom, 0f);
        cornerTopRight.transform.localPosition    = new Vector3(localXPosRight, localYPosTop, 0f);
        cornerBottomRight.transform.localPosition = new Vector3(localXPosRight, localYPosBottom, 0f);

        topBottomRect.transform.localScale = new Vector3(spriteSize.x - 0.5f, spriteSize.y, 0f);
        leftRightRect.transform.localScale = new Vector3(spriteSize.x, spriteSize.y - 0.5f, 0f);

        //Vector3 topLeftCornerPos = minBounds + new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(xOffset, -yOffset, 0f);
        //Vector3 botLeftCornerPos = minBounds + new Vector3(xOffset, yOffset, 0f);
        //Vector3 topRightCornerPos = maxBounds + new Vector3(-xOffset, -yOffset, 0f);
        //Vector3 botRightCornerPos = maxBounds - new Vector3(0f, mainBounds.extents.y * 2f, 0f) + new Vector3(-xOffset, yOffset, 0f);
    }

    private void Update()
    {
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        Vector2 currentSpriteSize = sprite.size;

        if (currentSpriteSize != prevSpriteRendererSize)
        {
            Debug.Log("Resizable Platform: Updating");
            prevSpriteRendererSize = currentSpriteSize;

            
            Vector3 scale = transform.localScale;

            // (xScale * spriteRendererXWidth) / 2 + (circleXScale / 2)
            // (yScale * spriteRendererYWidth) / 2 + (circleYScale / 2)
            Vector2 spriteSize = sprite.size;
            float localXPosLeft = -(scale.x * spriteSize.x) / 2 + ((cornerCircleScale - 0.005f) / 2);
            float localXPosRight = (scale.x * spriteSize.x) / 2 - ((cornerCircleScale - 0.005f) / 2);
            float localYPosTop = (scale.y * spriteSize.y) / 2 - ((cornerCircleScale - 0.005f) / 2);
            float localYPosBottom = -(scale.y * spriteSize.y) / 2 + ((cornerCircleScale - 0.005f) / 2);


            cornerTopLeft.transform.localPosition     = new Vector3(localXPosLeft, localYPosTop, 0f);
            cornerBottomLeft.transform.localPosition  = new Vector3(localXPosLeft, localYPosBottom, 0f);
            cornerTopRight.transform.localPosition    = new Vector3(localXPosRight, localYPosTop, 0f);
            cornerBottomRight.transform.localPosition = new Vector3(localXPosRight, localYPosBottom, 0f);

            topBottomRect.transform.localScale = new Vector3(spriteSize.x - 0.5f, spriteSize.y, 0f);
            leftRightRect.transform.localScale = new Vector3(spriteSize.x, spriteSize.y - 0.5f, 0f);
        }
    }
}
