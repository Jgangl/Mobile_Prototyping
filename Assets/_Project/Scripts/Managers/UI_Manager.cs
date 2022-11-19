using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField] SettingsMenu settingsMenu;
    [SerializeField] AfterLevelMenu afterLevelMenu;
    [SerializeField] LevelSelection levelSelection;
    [SerializeField] GameObject inLevelUI;
    [SerializeField] GameObject mainMenu;

    private Menu currentOpenMenu;

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
        settingsMenu.Open();
        settingsMenu.EnableHomeButton(true);
        //EnableSettingsMenu(true, true);
    }

    public void OnQuitButtonPressed() {
        GameManager.Instance.QuitGame();
    }
/*
    private void EnableSettingsMenu(bool enabled, bool homeButtonEnabled) {
        settingsMenu.Enable(enabled);
        settingsMenu.EnableHomeButton(homeButtonEnabled);
        isSettingsMenuOpen = enabled;
        UpdateMenuStatus();
    }
*/
    private void EnableLevelSelectionMenu(bool enabled, bool withAnimation)
    {
        levelSelection.EnableLevelSelectionMenu(enabled, withAnimation);
        
        isLevelSelectionMenuOpen = enabled;
        UpdateMenuStatus(enabled);
    }

    private void EnableInLevelUI(bool enabled)
    {
        inLevelUI.SetActive(enabled);
    }
    
    void OnLevelLoaded() 
    {
        EnableMainMenu(false);
        EnableLevelSelectionMenu(false, false);
        afterLevelMenu.CloseInstant();
        EnableInLevelUI(true);
    }

    void LevelManager_OnLevelCompleted(int levelCompleted)
    {
        StartCoroutine("LevelCompleteRoutine");
    }

    private IEnumerator LevelCompleteRoutine()
    {
        UpdateMenuStatus(true);

        TimeDilator.SlowTimeIndefinitely(0.25f);

        yield return Fader.Instance.FadeToCoroutine(0.35f, 0.75f);

        afterLevelMenu.Open();
        
        Fader.Instance.FadeInInstant();
    }

    private void EnableMainMenu(bool enabled)
    {
        isMainMenuOpened = enabled;
        mainMenu.SetActive(enabled);
    }

    private void UpdateMenuStatus(bool menuOpened) {
        //bool menuOpen = isSettingsMenuOpen || isLevelSelectionMenuOpen;
        //if (isMenuOpened != menuOpen) {
        isMenuOpened = menuOpened;
        GameManager.Instance.SetMenuOpened(isMenuOpened);
        //}
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
        settingsMenu.Open();
        settingsMenu.EnableHomeButton(!isMainMenuOpened);
        //EnableSettingsMenu(true, !isMainMenuOpened);
        UpdateMenuStatus(true);
    }

    public void CloseSettingsMenu()
    {
        if (settingsMenu)
        {
            GameManager.Instance.PauseGame(false);
            settingsMenu.Close();
            settingsMenu.EnableHomeButton(!isMainMenuOpened);
            UpdateMenuStatus(false);
        }
    }

    public void OnRestartButtonPressed() {
        Debug.Log("RESTART BUTTON");
        Level_Manager.Instance.RestartLevel();
    }

    public void OnMusicSwitch_On() {
        print("Music Switch ON");
        AudioManager.Instance.OnMusicChecked(true);
    }

    public void OnMusicSwitch_Off() {
        print("Music Switch OFF");
        AudioManager.Instance.OnMusicChecked(false);
    }

    public void OnSoundFXSwitch_On() {
        print("SFX Switch ON");
        AudioManager.Instance.OnSoundFXChecked(true);
    }

    public void OnSoundFXSwitch_Off() {
        print("SFX Switch OFF");
        AudioManager.Instance.OnSoundFXChecked(false);
    }

    public void OnMainMenuButtonPressed()
    {
        StartCoroutine(ReturnToMainMenuRoutine());
    }

    private IEnumerator ReturnToMainMenuRoutine()
    {
        yield return Fader.Instance.FadeOutCoroutine(0.5f);

        currentOpenMenu.CloseInstant();
        // Unload previous scenes
        Level_Manager.Instance.UnloadPreviousScenes();
        
        EnableMainMenu(true);
        EnableLevelSelectionMenu(false, false);
        EnableInLevelUI(false);
        TimeDilator.ResumeNormalTime();
        
        yield return new WaitForSeconds(0.2f);


        
        yield return StartCoroutine(Fader.Instance.FadeInCoroutine(1f));
    }

    public void SetCurrentOpenMenu(Menu openMenu)
    {
        currentOpenMenu = openMenu;
    }
}
