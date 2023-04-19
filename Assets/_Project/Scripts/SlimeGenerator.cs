using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Pool;

public class SlimeGenerator : MonoBehaviour
{
    [SerializeField] ParticleSystem slimeParticleSystem;
    [SerializeField] Material[] slimeMaterials;

    int numActiveObjects;

    Transform slimeBallParent;

    ColorManager playerColorManager;

    void Start()
    {
        playerColorManager = GetComponent<ColorManager>();
        slimeBallParent = new GameObject("SlimeBallParent").transform;
    }

    public void Generate(Vector3 position, GameObject hitObject)
    {
        ParticleSystem slime = Instantiate(slimeParticleSystem, position, Quaternion.identity);
        slime.GetComponent<ParticleSystemRenderer>().material = GetRandomSlimeMaterial();
        
        playerColorManager.SetParticleSystemStartColorHue(slime, playerColorManager.GetSelectedHue());
        
        MovingObject movingObject = hitObject.GetComponentInParent<MovingObject>();
        bool bParentToHitObject = movingObject != null;
        
        slime.transform.position = position;
        slime.transform.parent = bParentToHitObject ? movingObject.transform : slimeBallParent;
    }

    Material GetRandomSlimeMaterial()
    {
        int randIndex = Random.Range(0, slimeMaterials.Length);

        return slimeMaterials[randIndex];
    }
}
