using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bouncer))]
public class BouncerEditor : Editor
{
    Bouncer bouncer;

    Transform leftCircle;
    Transform rightCircle;
    Transform middleSquare;

    private void OnEnable() {
        bouncer = (Bouncer)target;

        leftCircle = bouncer.transform.Find("LeftCircle");
        rightCircle = bouncer.transform.Find("RightCircle");
        middleSquare = bouncer.transform.Find("MiddleSquare");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Transform bouncerTransform = bouncer.transform;

        Vector3 bouncerScale = bouncerTransform.localScale;




    }

    
}
