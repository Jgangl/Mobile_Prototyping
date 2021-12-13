using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class UI_Manager : Singleton<UI_Manager>
{
    GameObject settingsMenu;
    GameObject levelSelectionMenu;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        FindMenuObjects();
        FindButtons();
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
        FindButtons();

        DisableMenuObjects();
    }

    private void FindMenuObjects() {
        ClearMenuObjects();

        settingsMenu = GameObject.Find("SettingsMenu");
        levelSelectionMenu = GameObject.Find("LevelSelectionMenu");
    }

    private void FindButtons() {
        GameObject levelSelectionButtonObj = GameObject.Find("LevelSelectionButton");
        GameObject settingsButtonObj = GameObject.Find("SettingsButton");
        GameObject restartButtonObj = GameObject.Find("RestartButton");
        GameObject levelSelectExitButtonObj = GameObject.Find("LevelSelectExitButton");
        GameObject settingsExitButtonObj = GameObject.Find("SettingsExitButton");
        GameObject musicSwitchObj = GameObject.Find("Music_Switch");
        GameObject soundFXSwitchObj = GameObject.Find("SoundFX_Switch");

        Button levelSelectionButton;
        if (levelSelectionButtonObj) {
            levelSelectionButton = levelSelectionButtonObj.GetComponent<Button>();
            if (levelSelectionButton) {
                levelSelectionButton.onClick.AddListener(OpenLevelSelectionMenu);
            }
        }

        Button settingsButton;
        if (settingsButtonObj) {
            settingsButton = settingsButtonObj.GetComponent<Button>();
            if (settingsButton)
                settingsButton.onClick.AddListener(OpenSettingsMenu);
        }

        Button restartButton;
        if (restartButtonObj) {
            restartButton = restartButtonObj.GetComponent<Button>();
            if (restartButton) {
                restartButton.onClick.AddListener(OnRestartButton);
            }
        }

        Button levelSelectExitButton;
        if (levelSelectExitButtonObj) {
            levelSelectExitButton = levelSelectExitButtonObj.GetComponent<Button>();
            if (levelSelectExitButton)
                levelSelectExitButton.onClick.AddListener(DisableMenuObjects);
        }

        Button settingsExitButton;
        if (settingsExitButtonObj) {
            settingsExitButton = settingsExitButtonObj.GetComponent<Button>();
            if (settingsExitButton)
                settingsExitButton.onClick.AddListener(DisableMenuObjects);
        }

        SwitchManager musicSwitch;
        if (musicSwitchObj) {
            musicSwitch = musicSwitchObj.GetComponent<SwitchManager>();
            if (musicSwitch) {
                musicSwitch.OnEvents.AddListener(OnMusicSwitch_On);
                musicSwitch.OffEvents.AddListener(OnMusicSwitch_Off);
                Sound_Manager.Instance.UpdateMusicEnabled(musicSwitch.isOn);
            }
        }

        SwitchManager soundFXSwitch;
        if (soundFXSwitchObj) {
            soundFXSwitch = soundFXSwitchObj.GetComponent<SwitchManager>();
            if (soundFXSwitch) {
                soundFXSwitch.OnEvents.AddListener(OnSoundFXSwitch_On);
                soundFXSwitch.OffEvents.AddListener(OnSoundFXSwitch_Off);
                Sound_Manager.Instance.UpdateSoundFXEnabled(soundFXSwitch.isOn);
            }
        }
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
        Debug.Log("RESTART BUTTON");
        Level_Manager.Instance.RestartLevel();
    }

    public void OnMusicSwitch_On() {
        Sound_Manager.Instance.OnMusicChecked(true);
    }

    public void OnMusicSwitch_Off() {
        Sound_Manager.Instance.OnMusicChecked(false);
    }

    public void OnSoundFXSwitch_On() {
        Sound_Manager.Instance.OnSoundFXChecked(true);
    }

    public void OnSoundFXSwitch_Off() {
        Sound_Manager.Instance.OnSoundFXChecked(false);
    }
}
