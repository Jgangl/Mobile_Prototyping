using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    [SerializeField] private Transform BlackBarPrefab;

    private Camera mainCamera;

    [SerializeField] private Vector3 Offset;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        //Vector3 RightPosition = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        //Vector3 LeftPosition = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));

        Vector3 RightPosition = Offset;
        Vector3 LeftPosition = Offset * -1.0f;

        Transform LeftBar = Instantiate(BlackBarPrefab, LeftPosition, Quaternion.identity);
        LeftBar.localScale = new Vector3(LeftBar.localScale.x * -1, LeftBar.localScale.y, LeftBar.localScale.z);
        Transform RightBar = Instantiate(BlackBarPrefab, RightPosition, Quaternion.identity);
        
        LeftBar.position = new Vector3(LeftBar.position.x, LeftBar.position.y, 0f);
        RightBar.position = new Vector3(RightBar.position.x, RightBar.position.y, 0f);
        //Rect cameraRect = mainCamera.rect;
        //Debug.Log(cameraRect.xMax);
        //cameraRect.center + cameraRect.xMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
