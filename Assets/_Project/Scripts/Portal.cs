using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal connectedPortal;

    private bool canTeleport = true;

    private Vector3 playerVelocityOnEnter;
    private float playerAngleOnEnter;

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
            playerVelocityOnEnter = player.GetObjectAverageVelocity();

            playerAngleOnEnter = Vector3.Angle(playerVelocityOnEnter, transform.up);

            float previousSpeed = playerVelocityOnEnter.magnitude;
            Vector3 newDirection = connectedPortal.transform.up;//Quaternion.Euler(0f, 0f, playerAngleOnEnter) * connectedPortal.transform.up;
            Debug.Log(newDirection);
            Debug.DrawRay(connectedPortal.transform.position, newDirection, Color.yellow, 3f);

            player.transform.position = connectedPortal.transform.position;
            Vector2 newVelocity = newDirection * previousSpeed;
            player.SetVelocity(newVelocity);
            //Debug.Log(playerRb.velocity);

            //Debug.Log(playerRb.velocity);
        }
    }

    private void SetCanTeleport(bool canTeleport) {
        this.canTeleport = canTeleport;
    }
}
