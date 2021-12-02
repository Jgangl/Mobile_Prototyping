using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : Singleton<UI_Manager>
{
    GameObject settingsMenu;
    GameObject levelSelectionMenu;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        FindMenuObjects();
        DisableMenuObjects();
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

    private void EnableSettingsMenu(bool enabled) {
        if (settingsMenu) {
            settingsMenu.SetActive(enabled);
        }
    }

    private void EnableLevelSelectionMenu(bool enabled) {
        if (levelSelectionMenu) {
            levelSelectionMenu.SetActive(enabled);
        }
    }

    public void OpenSettingsMenu() {
        EnableSettingsMenu(true);
    }

    public void OpenLevelSelectionMenu() {
        EnableLevelSelectionMenu(true);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Find menu objects

        FindMenuObjects();

        DisableMenuObjects();
    }

    private void FindMenuObjects() {
        ClearMenuObjects();

        settingsMenu = GameObject.Find("SettingsMenu");
        levelSelectionMenu = GameObject.Find("LevelSelectionMenu");
    }

    private void DisableMenuObjects() {
        if (settingsMenu)
            settingsMenu.SetActive(false);

        if (levelSelectionMenu)
            levelSelectionMenu.SetActive(false);
    }

    private void ClearMenuObjects() {
        settingsMenu = null;
        levelSelectionMenu = null;
    }

    public void OnCloseMenuButton() {
        DisableMenuObjects();
    }

    public void OnRestartButton() {
        Level_Manager.Instance.RestartLevel();
    }
}
