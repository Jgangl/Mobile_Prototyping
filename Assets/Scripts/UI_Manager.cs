using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : Singleton<UI_Manager>
{
    GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton() {
        GameManager.Instance.StartGame();
    }

    public void SettingsButton() {
        
    }

    public void LevelSelectionButton() {

    }

    public void QuitButton() {
        GameManager.Instance.QuitGame();
    }

    private void OpenSettingsMenu() {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Find menu objects

        settingsMenu = GameObject.Find("SettingsMenu");
    }
}
