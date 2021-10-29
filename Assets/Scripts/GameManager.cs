using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            GameOver();
        }
    }

    public void GameOver() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartLevel() {

    }
}
