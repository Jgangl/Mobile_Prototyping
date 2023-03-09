using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    GameData gameData;
    PlayerController player;
    GameState currentGameState;
    bool isMenuOpened = false;
    bool gameOver = false;
    bool isPlaying = false;

    int numJumpsThisLevel = 0;
    double timeThisLevel = 0.0f;

    [SerializeField] bool disableLoadingSaving;

    public Action OnLoadedSave;

    protected override void Awake()
    {
        base.Awake();
        
        currentGameState = GameState.Playing;
        gameData = new GameData();
        
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            player = playerObj.GetComponent<PlayerController>();
        }

        Level_Manager.Instance.OnLevelReset += OnLevelReset;
        Level_Manager.Instance.OnLevelLoaded += OnLevelLoaded;

        PlayerController.OnJumped += OnPlayerJumped;
        
        LoadGame();
    }

    void Update()
    {
        if (isPlaying)
            timeThisLevel += Time.deltaTime;
    }  

    public void StartGame() 
    {
        Level_Manager.Instance.LoadFirstUncompletedLevel();
    }

    public void QuitGame() 
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void GameOver()
    {
        if (gameOver || 
            Level_Manager.Instance.IsLoadingLevel() || 
            Level_Manager.Instance.IsLevelCompleted())
        {
            return;
        }

        gameOver = true;
        
        TimeDilator.SlowTime(this, 0.3f, 2f);
        Level_Manager.Instance.RestartCurrentLevel();
    }

    public void CompleteLevel()
    {
        if (gameOver) return;

        isPlaying = false;

        Level_Manager.Instance.CompleteCurrentLevel();
    }

    public void PauseGame(bool pause) 
    {
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

    public void PlayerDied() 
    {
        Debug.Log("Player Died");
        GameOver();
    }

    public void SetMenuOpened(bool menuOpen) 
    {
        isMenuOpened = menuOpen;

        // Enable/Disable player movement if a menu is open
        if (player) 
        {
            player.EnableInput(!isMenuOpened);
        }
    }

    public void OnLevelReset()
    {
        isPlaying = true;
        gameOver = false;
        numJumpsThisLevel = 0;
        timeThisLevel = 0.0f;
    }

    void OnLevelLoaded() 
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            player = playerObj.GetComponent<PlayerController>();
        }

        gameOver = false;
        isPlaying = true;
        numJumpsThisLevel = 0;
        timeThisLevel = 0.0f;
    }
    
    public bool LoadGame()
    {
        if (disableLoadingSaving)
            return false;
        
        object saveData = Save_Manager.Instance.LoadGame();

        // A save file already exists
        if (saveData != null) 
        {
            Debug.Log("Loaded Game");
            gameData = (GameData)saveData;

            Level_Manager.Instance.TryLoadLevels(gameData.GetLevels());

            OnLoadedSave?.Invoke();
            
            return true;
        }
        else 
        {
            Debug.Log("No save file found");
            return false;
        }
    }

    public bool SaveGame() 
    {
        if (disableLoadingSaving)
            return false;
        
        if (gameData != null) 
        {
            // Insert save data
            gameData.SetLevels(Level_Manager.Instance.GetLevels());
            bool saved = Save_Manager.Instance.Save(gameData);

            return saved;
        }
        else 
        {
            //Debug.Log("GAME NOT SAVED");
            return false;
        }
    }

    public void EnablePlayerMovement()
    {
        if (player) 
        {
            player.EnableInput(true);
        }
    }

    public void DisablePlayerMovement()
    {
        if (player) 
        {
            player.EnableInput(false);
        }
    }

    void OnApplicationQuit() 
    {
        SaveGame();
    }

    void OnPlayerJumped()
    {
        Debug.Log("Player Jumped");
        numJumpsThisLevel++;
    }

    public int GetNumJumpsThisLevel()
    {
        return numJumpsThisLevel;
    }

    public double GetTimeThisLevel()
    {
        return timeThisLevel;
    }
}

public enum GameState 
{
    Playing,
    Paused,
    Menu
}
