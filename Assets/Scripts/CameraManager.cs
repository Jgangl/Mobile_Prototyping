using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float cameraX = 0.0f;

    GameObject player;

    public GameObject boundaryYMarker;

    public float deadZoneTop = 3f;

    public float smoothing = 5f;

    public GameObject deadZoneMarker;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //if (player.transform.position.y <= boundaryYMarker.transform.position.y) {
        //    GameManager.Instance.GameOver();
        //}
    }

    private void LateUpdate() {
        //float targetY = player.transform.position.y + deadZoneTop;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);

        //deadZoneMarker.transform.position
        /*
        if (player.transform.position.y >= transform.position.y + deadZoneTop) {
            // Move camera up
            Vector2 startPos = transform.position;
            Vector2 targetPos = new Vector2(transform.position.x, targetY);

            transform.position = Vector2.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
        //transform.position = new Vector3(cameraX, player.transform.position.y, transform.position.z);
        */
    }
}
