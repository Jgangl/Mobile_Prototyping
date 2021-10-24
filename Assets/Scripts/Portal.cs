using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal connectedPortal;

    private bool canTeleport = true;

    private Vector3 playerVelocityOnEnter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null) {
            TeleportPlayer(player);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null) {
            SetCanTeleport(true);
        }
    }

    private void TeleportPlayer(PlayerController player) {
        Debug.Log("Teleported player");
        if (connectedPortal != null && canTeleport) {
            connectedPortal.SetCanTeleport(false);

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            playerVelocityOnEnter = playerRb.velocity;

            float speed = playerVelocityOnEnter.magnitude;
            Vector3 newDirection = connectedPortal.transform.up;

            player.transform.position = connectedPortal.transform.position;
            playerRb.velocity = newDirection * speed;
        }

    }

    private void SetCanTeleport(bool canTeleport) {
        this.canTeleport = canTeleport;
    }
}
