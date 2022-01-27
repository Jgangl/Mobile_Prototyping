using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class CameraMatchWidth : MonoBehaviour {

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 10;

    CinemachineVirtualCamera _cmCam;

    private void Awake()
    {
        _cmCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update() {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        _cmCam.m_Lens.OrthographicSize = desiredHalfHeight;
    }
}
