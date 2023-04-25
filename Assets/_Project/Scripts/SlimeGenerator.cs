using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class SlimeGenerator : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] slimePrefabs;
    [SerializeField] int maxNumOverlappingSlimeSprites = 5;
    [SerializeField] float COLLIDER_DIST_THRESHOLD;
    
    int numActiveObjects;

    Transform slimeBallParent;

    ColorManager playerColorManager;

    void Start()
    {
        playerColorManager = GetComponent<ColorManager>();
        slimeBallParent = new GameObject("SlimeBallParent").transform;

        // Remove overlapping colliders that are 'far enough' away
    }

    public void Generate(Vector3 position, GameObject hitObject)
    {
        SpriteRenderer slime = Instantiate(GetRandomSlimePrefab(), position, Quaternion.identity);
        playerColorManager.SetSpriteHue(slime, playerColorManager.GetSelectedHue());

        RemoveOverlappingSlime(slime);

        MovingObject movingObject = hitObject.GetComponentInParent<MovingObject>();
        bool bParentToHitObject = movingObject != null;
        
        slime.transform.position = position;
        slime.transform.parent = bParentToHitObject ? movingObject.transform : slimeBallParent;
    }

    SpriteRenderer GetRandomSlimePrefab()
    {
        int randIndex = Random.Range(0, slimePrefabs.Length);

        return slimePrefabs[randIndex];
    }

    void RemoveOverlappingSlime(SpriteRenderer sprite)
    {
        List<Collider2D> overlappingColliders = GetOverlappingColliders(sprite);

        List<Collider2D> overlappingCollidersWithinDist = GetCollidersWithinDistance(sprite, overlappingColliders);

        if (overlappingColliders.Count > maxNumOverlappingSlimeSprites)
        {
            List<Collider2D> distSortedOverlapColliders = 
                overlappingColliders.OrderByDescending(
                    x => (sprite.transform.position - x.transform.position).sqrMagnitude
                ).ToList();

            List<Collider2D> collidersToRemove = new List<Collider2D>();
            for (int i = maxNumOverlappingSlimeSprites - 1; i < distSortedOverlapColliders.Count; i++)
            {
                collidersToRemove.Add(distSortedOverlapColliders[i]);
            }

            foreach (Collider2D colliderToRemove in collidersToRemove)
            {
                // Fade out image and then delete
                SpriteRenderer spriteToRemove = colliderToRemove.GetComponent<SpriteRenderer>();
                if (spriteToRemove)
                {
                    Destroy(colliderToRemove, 0.01f);
                    spriteToRemove.DOFade(0.0f, 2.0f).OnComplete(() => Destroy(spriteToRemove.gameObject));
                }
            }
        }
    }

    List<Collider2D> GetCollidersWithinDistance(SpriteRenderer spriteToTest, List<Collider2D> overlappingColliders)
    {
        List<Collider2D> farColliders = new List<Collider2D>();
        foreach (Collider2D overlapCollider in overlappingColliders)
        {
            float overlapColliderDist = Vector2.Distance(overlapCollider.transform.position, spriteToTest.transform.position);

            if (overlapColliderDist > COLLIDER_DIST_THRESHOLD)
            {
                farColliders.Add(overlapCollider);
            }
        }

        foreach (Collider2D farCollider in farColliders)
        {
            if (overlappingColliders.Contains(farCollider))
            {
                overlappingColliders.Remove(farCollider);
            }
        }

        return overlappingColliders;
    }

    List<Collider2D> GetOverlappingColliders(SpriteRenderer sprite)
    {
        Collider2D testCollider = sprite.GetComponent<Collider2D>();
        ContactFilter2D slimeFilter = new ContactFilter2D();
        slimeFilter.SetLayerMask(LayerMask.GetMask("Slime"));
        slimeFilter.useTriggers = true;

        List<Collider2D> overlappingColliders = new List<Collider2D>();
        testCollider.OverlapCollider(slimeFilter, overlappingColliders);

        return overlappingColliders;
    }
}
