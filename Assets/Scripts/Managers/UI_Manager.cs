using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject levelSelectionMenu;
    [SerializeField] GameObject inLevelUI;
    GameObject mainMenu;

    bool isLevelSelectionMenuOpen = false;
    bool isSettingsMenuOpen = false;
    bool isMenuOpened = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void Start()
    {

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void OnStartButtonPressed() {
        print("start game");
        GameManager.Instance.StartGame();
    }

    public void OnLevelSettingsButtonPressed()
    {
        // Pause game
        GameManager.Instance.PauseGame(true);
        EnableSettingsMenu(true);
    }

    public void OnQuitButtonPressed() {
        GameManager.Instance.QuitGame();
    }

    private void EnableSettingsMenu(bool enabled) {
        if (settingsMenu) {
            Animator anim = settingsMenu.GetComponent<Animator>();
            if (anim) {
                if (enabled)
                    anim.SetTrigger("Open");
                else
                    anim.SetTrigger("Close");
            }

            isSettingsMenuOpen = enabled;
            UpdateMenuStatus();
        }
    }

    private void EnableLevelSelectionMenu(bool enabled) {
        if (levelSelectionMenu) {
            Animator anim = levelSelectionMenu.GetComponent<Animator>();
            if (anim) {
                if (enabled)
                    anim.SetTrigger("Open");
                else
                    anim.SetTrigger("Close");
            }

            isLevelSelectionMenuOpen = enabled;
            UpdateMenuStatus();
        }
    }

    private void EnableInLevelUI(bool enabled)
    {
        inLevelUI.SetActive(enabled);
    }
    
    void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        string[] splitName = SceneManager.GetActiveScene().name.Split('_');
        bool inLevel = false;
        foreach(string val in splitName) {
            int levelNum = -1;
            if (int.TryParse(val, out levelNum)) {
                if (levelNum != -1) {
                    inLevel = true;
                    break;
                }
            }
        }

        EnableInLevelUI(inLevel);
    }

    private void UpdateMenuStatus() {
        bool menuOpen = isSettingsMenuOpen || isLevelSelectionMenuOpen;
        if (isMenuOpened != menuOpen) {
            isMenuOpened = menuOpen;
            GameManager.Instance.SetMenuOpened(isMenuOpened);
        }
    }
    
    public void OpenLevelSelectionMenu() {
        EnableLevelSelectionMenu(true);
    }
    
    public void CloseLevelSelectionMenu() {
        EnableLevelSelectionMenu(false);
    }

    public void OpenSettingsMenu()
    {
        EnableSettingsMenu(true);
    }

    public void CloseSettingsMenu() {
        GameManager.Instance.PauseGame(false);
        EnableSettingsMenu(false);
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
}
