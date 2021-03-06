using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    GameData gameData;

    private GameState currentGameState;

    bool isMenuOpened = false;

    GameObject player;

    private bool gameOver = false;

    //private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        currentGameState = GameState.Playing;
        gameData = new GameData();

        if (LoadGame())
            Level_Manager.Instance.SetCompletedLevels(gameData.GetCompletedLevels());

        // Find Player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            if (currentGameState == GameState.Paused)
                PauseGame(false);
            else
                PauseGame(true);
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

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        Debug.Log("Game Over");

        Level_Manager.Instance.LoadCurrentLevel();
    }

    public void PauseGame(bool pause) {
        if (pause && currentGameState == GameState.Playing)
        {
            Time.timeScale = 0.0f;
            currentGameState = GameState.Paused;
        }
        else if (!pause && currentGameState == GameState.Paused) 
        {
            Time.timeScale = 1.0f;
            currentGameState = GameState.Playing;
        }
    }

    public void PlayerDied() {
        Debug.Log("Player Died");

        GameOver();
    }

    public void SetMenuOpened(bool menuOpen) {
        isMenuOpened = menuOpen;

        // Enable/Disable player movement if a menu is open
        if (player) {
            player.GetComponent<PlayerController>().EnableMovement(!isMenuOpened);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        player = GameObject.FindGameObjectWithTag("Player");
        gameOver = false;
    }

    public bool SaveGame() {
        List<int> completedLevels = Level_Manager.Instance.GetCompletedLevels();

        if (gameData != null) {
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
            //Debug.Log(gameData.GetCompletedLevels().Count);
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
