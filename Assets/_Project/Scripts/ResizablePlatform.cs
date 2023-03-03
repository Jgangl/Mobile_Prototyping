using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizablePlatform : MonoBehaviour
{
    [SerializeField] GameObject cornerTopLeft;
    [SerializeField] GameObject cornerBottomLeft;
    [SerializeField] GameObject cornerTopRight;
    [SerializeField] GameObject cornerBottomRight;
    [SerializeField] GameObject topBottomRect;
    [SerializeField] GameObject leftRightRect;
    
    Vector2 prevSpriteRendererSize;
    SpriteRenderer sprite;

    //float xScaleOffset = 0.02f;
    float cornerCircleScale = 0.475f;
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        Vector3 scale = transform.localScale;
        
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

    void Update()
    {
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        Vector2 currentSpriteSize = sprite.size;

        if (currentSpriteSize != prevSpriteRendererSize)
        {
            prevSpriteRendererSize = currentSpriteSize;

            Vector3 scale = transform.localScale;

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
