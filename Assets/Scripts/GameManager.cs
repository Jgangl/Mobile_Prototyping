using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    GameData gameData;

    private GameState currentGameState;

    // Start is called before the first frame update
    void Start()
    {
        currentGameState = GameState.Playing;
        gameData = new GameData();

        LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            PauseGame();
        }
    }

    public void StartGame() {
        // Start game at current uncompleted level
        Debug.Log("Started Game");
        Level_Manager.Instance.LoadCurrentLevel();
    }

    public void QuitGame() {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void GameOver() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame() {
        if (currentGameState == GameState.Playing) {
            Time.timeScale = 0.0f;
            currentGameState = GameState.Paused;
        }
        else if (currentGameState == GameState.Paused) {
            Time.timeScale = 1.0f;
            currentGameState = GameState.Playing;
        }
    }

    public bool SaveGame() {
        List<int> completedLevels = Level_Manager.Instance.GetCompletedLevels();

        if (gameData != null) {
            //Debug.Log("Saved Game");
            // Insert save data
            gameData.SetCompletedLevels(completedLevels);
            Save_Manager.Instance.Save(gameData);
            return true;
        }
        else {
            //Debug.Log("GAME NOT SAVED");
            return false;
        }
    }

    public bool LoadGame() {
        object saveData = Save_Manager.Instance.LoadGame();

        if (saveData != null) {
            //Debug.Log("Loaded Game");
            gameData = (GameData)saveData;
            return true;
        }
        else {
            //Debug.Log("DID NOT LOAD Game");
            return false;
        }

    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    private void OnApplicationPause(bool pause) {
        if (pause)
            SaveGame();
        else
            LoadGame();
    }
}

public enum GameState {
    Playing,
    Paused,
    Menu
}
