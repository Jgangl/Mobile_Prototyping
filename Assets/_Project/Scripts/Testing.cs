using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    //[SerializeField] Player

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(Camera.main.aspect);
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Shake the screen");
            ScreenShake.Instance.Shake();
        }
    }
}
