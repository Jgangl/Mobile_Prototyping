using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        gameData = new GameData();

        LoadGame();
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

    public void CompleteLevel(int level) {
        Level_Manager.Instance.LevelCompleted(level);
    }

    public void RestartLevel() {

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
