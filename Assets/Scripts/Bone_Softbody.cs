using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone_Softbody : MonoBehaviour
{
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (player == null) {
            return;
        }

        player.OnChildCollisionEnter2D(this, collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (player == null) {
            return;
        }

        player.OnChildCollisionStay2D(this, collision);
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (player == null) {
            return;
        }

        player.OnChildCollisionExit2D(this, collision);
    }
}
