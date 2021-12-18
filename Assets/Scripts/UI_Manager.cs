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

    GameObject levelSelectionButtonObj;
    GameObject settingsButtonObj;
    GameObject settingsButtonLevelObj;
    GameObject restartButtonObj;
    GameObject levelSelectExitButtonObj;
    GameObject settingsExitButtonObj;
    GameObject musicSwitchObj;
    GameObject soundFXSwitchObj;

    bool isLevelSelectionMenuOpen = false;
    bool isSettingsMenuOpen = false;
    bool isMenuOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneLoad();
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
            print("Settings menu enabled: " + enabled);

            isSettingsMenuOpen = enabled;
            UpdateMenuStatus();
        }
    }

    private void EnableLevelSelectionMenu(bool enabled) {
        if (levelSelectionMenu) {
            levelSelectionMenu.SetActive(enabled);

            isLevelSelectionMenuOpen = enabled;
            UpdateMenuStatus();
        }
    }

    public void OpenSettingsMenu() {
        EnableSettingsMenu(true);
    }

    public void OpenLevelSelectionMenu() {
        EnableLevelSelectionMenu(true);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneLoad();
    }

    void SceneLoad() {
        // Find menu objects

        FindMenuObjects();
        FindButtons();

        DisableMenuObjects();

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

        EnableSettingsButton(inLevel);
        EnableRestartButton(inLevel);
    }

    private void UpdateMenuStatus() {
        bool menuOpen = isSettingsMenuOpen || isLevelSelectionMenuOpen;
        if (isMenuOpened != menuOpen) {
            isMenuOpened = menuOpen;
            GameManager.Instance.SetMenuOpened(isMenuOpened);
        }
    }

    private void FindMenuObjects() {
        ClearMenuObjects();

        settingsMenu = GameObject.Find("SettingsMenu");
        levelSelectionMenu = GameObject.Find("LevelSelectionMenu");
    }

    private void FindButtons() {
        levelSelectionButtonObj = GameObject.Find("LevelSelectionButton");
        settingsButtonObj = GameObject.Find("SettingsButton");
        settingsButtonLevelObj = GameObject.Find("SettingsButtonLevel");
        restartButtonObj = GameObject.Find("RestartButton");
        levelSelectExitButtonObj = GameObject.Find("LevelSelectExitButton");
        settingsExitButtonObj = GameObject.Find("SettingsExitButton");
        musicSwitchObj = GameObject.Find("Music_Switch");
        soundFXSwitchObj = GameObject.Find("SoundFX_Switch");

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

        Button settingsButtonLevel;
        if (settingsButtonLevelObj) {
            settingsButtonLevel = settingsButtonLevelObj.GetComponent<Button>();
            if (settingsButtonLevel)
                settingsButtonLevel.onClick.AddListener(OpenSettingsMenu);
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
        EnableSettingsMenu(false);
        EnableLevelSelectionMenu(false);
    }

    private void ClearMenuObjects() {
        settingsMenu = null;
        levelSelectionMenu = null;
    }

    public void OnCloseMenuButton() {
        DisableMenuObjects();
    }

    public void EnableSettingsButton(bool enabled) {
        if (settingsButtonLevelObj) {
            settingsButtonLevelObj.SetActive(enabled);
        }
    }

    public void EnableRestartButton(bool enabled) {
        if (restartButtonObj)
            restartButtonObj.SetActive(enabled);
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
