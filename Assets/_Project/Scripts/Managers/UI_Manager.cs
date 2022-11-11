using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] SettingsMenu settingsMenu;
    [SerializeField] AfterLevelMenu afterLevelMenu;
    [SerializeField] LevelSelection levelSelection;
    [SerializeField] GameObject inLevelUI;
    [SerializeField] GameObject mainMenu;

    bool isLevelSelectionMenuOpen = false;
    bool isSettingsMenuOpen = false;
    bool isMenuOpened = false;

    bool isMainMenuOpened = true;

    private void Start()
    {
        Level_Manager.Instance.OnLevelLoaded += OnLevelLoaded;
        Level_Manager.Instance.OnLevelCompleted += LevelManager_OnLevelCompleted;
    }

    public void OnStartButtonPressed() {
        GameManager.Instance.StartGame();
    }

    public void OnLevelSettingsButtonPressed()
    {
        // Pause game
        GameManager.Instance.PauseGame(true);
        EnableSettingsMenu(true, true);
    }

    public void OnQuitButtonPressed() {
        GameManager.Instance.QuitGame();
    }

    private void EnableSettingsMenu(bool enabled, bool homeButtonEnabled) {
        settingsMenu.Enable(enabled, homeButtonEnabled);
        
        isSettingsMenuOpen = enabled;
        UpdateMenuStatus();
    }

    private void EnableLevelSelectionMenu(bool enabled, bool withAnimation)
    {
        levelSelection.EnableLevelSelectionMenu(enabled, withAnimation);
        
        isLevelSelectionMenuOpen = enabled;
        UpdateMenuStatus();
    }

    private void EnableInLevelUI(bool enabled)
    {
        inLevelUI.SetActive(enabled);
    }
    
    void OnLevelLoaded() {
        EnableMainMenu(false);
        EnableLevelSelectionMenu(false, false);
        EnableInLevelUI(true);
    }

    void LevelManager_OnLevelCompleted(int levelCompleted)
    {
        afterLevelMenu.Open();
    }

    private void EnableMainMenu(bool enabled)
    {
        isMainMenuOpened = enabled;
        mainMenu.SetActive(enabled);
    }

    private void UpdateMenuStatus() {
        bool menuOpen = isSettingsMenuOpen || isLevelSelectionMenuOpen;
        if (isMenuOpened != menuOpen) {
            isMenuOpened = menuOpen;
            GameManager.Instance.SetMenuOpened(isMenuOpened);
        }
    }
    
    public void OpenLevelSelectionMenu() {
        EnableLevelSelectionMenu(true, true);
    }
    
    public void CloseLevelSelectionMenu() {
        EnableLevelSelectionMenu(false, true);
    }

    public void OpenAfterLevelMenu()
    {
        afterLevelMenu.Open();
    }

    public void CloseAfterLevelMenu()
    {
        afterLevelMenu.Close();
    }

    public void OpenSettingsMenu()
    {
        EnableSettingsMenu(true, !isMainMenuOpened);
    }

    public void CloseSettingsMenu() {
        GameManager.Instance.PauseGame(false);
        EnableSettingsMenu(false, !isMainMenuOpened);
    }

    public void OnExitButtonPressed()
    {
        
    }

    public void OnRestartButtonPressed() {
        Debug.Log("RESTART BUTTON");
        Level_Manager.Instance.RestartLevel();
    }

    public void OnMusicSwitch_On() {
        print("Music Switch ON");
        Sound_Manager.Instance.OnMusicChecked(true);
    }

    public void OnMusicSwitch_Off() {
        print("Music Switch OFF");
        Sound_Manager.Instance.OnMusicChecked(false);
    }

    public void OnSoundFXSwitch_On() {
        print("SFX Switch ON");
        Sound_Manager.Instance.OnSoundFXChecked(true);
    }

    public void OnSoundFXSwitch_Off() {
        print("SFX Switch OFF");
        Sound_Manager.Instance.OnSoundFXChecked(false);
    }

    public void OnMainMenuButtonPressed()
    {
        CloseSettingsMenu();
        EnableMainMenu(true);
        EnableLevelSelectionMenu(false, false);
        EnableInLevelUI(false);
        Level_Manager.Instance.UnloadPreviousScenes();
    }
}
