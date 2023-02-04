using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone_Softbody : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (player == null) 
        {
            return;
        }

        player.OnChildCollisionEnter2D(this, collision);
    }

    void OnCollisionStay2D(Collision2D collision) 
    {
        if (player == null) 
        {
            return;
        }

        player.OnChildCollisionStay2D(this, collision);
    }
    void OnCollisionExit2D(Collision2D collision) 
    {
        if (player == null) {
            return;
        }

        player.OnChildCollisionExit2D(this, collision);
    }

    public void SetRigidbodyIsKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}
