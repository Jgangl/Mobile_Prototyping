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
    private PlayerController player;

    bool isLevelSelectionMenuOpen = false;
    bool isSettingsMenuOpen = false;
    bool isMenuOpened = false;
    bool isMainMenuOpened = true;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Level_Manager.Instance.OnLevelCompleted += LevelManager_OnLevelCompleted;
    }

    public void OnStartButtonPressed() {
        GameManager.Instance.StartGame();
    }

    public void OnLevelSettingsButtonPressed()
    {
        DisablePlayerInput();
        
        GameManager.Instance.PauseGame(true);
        
        settingsMenu.Open();
        settingsMenu.EnableHomeButton(true);
    }

    public void OnQuitButtonPressed() {
        GameManager.Instance.QuitGame();
    }

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
    
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null) Debug.Log("Player was null");
        
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

        yield return Fader.Instance.FadeToCoroutine(0.35f, 1f);

        afterLevelMenu.Open();
        
        Fader.Instance.FadeInInstant();
    }

    private void EnableMainMenu(bool enabled)
    {
        isMainMenuOpened = enabled;
        mainMenu.SetActive(enabled);
    }

    private void UpdateMenuStatus(bool menuOpened) {
        isMenuOpened = menuOpened;
        GameManager.Instance.SetMenuOpened(isMenuOpened);
    }
    
    public void OpenLevelSelectionMenu() {
        EnableLevelSelectionMenu(true, true);
    }
    
    public void CloseLevelSelectionMenu() {
        EnableLevelSelectionMenu(false, true);
    }

    public void OpenSettingsMenu()
    {
        DisablePlayerInput();
        
        settingsMenu.Open();
        settingsMenu.EnableHomeButton(!isMainMenuOpened);
        UpdateMenuStatus(true);
    }

    public void CloseSettingsMenu()
    {
        EnablePlayerInput();
        
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
        Level_Manager.Instance.RestartCurrentLevel();
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
    
    public void OnCameraShakeSwitch_On() {
        print("Camera Shake Switch ON");
        CinemachineShake.Instance.EnableCameraShake();
    }

    public void OnCameraShakeSwitch_Off() {
        print("Camera Shake Switch OFF");
        CinemachineShake.Instance.DisableCameraShake();
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
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(Fader.Instance.FadeInCoroutine(1f));
    }

    public void SetCurrentOpenMenu(Menu openMenu)
    {
        currentOpenMenu = openMenu;
    }

    private void EnablePlayerInput()
    {
        if (player)
            player.EnableMovement(true);
    }

    private void DisablePlayerInput()
    {
        if (player)
        {
            player.EnableMovement(false);
        }
    }
}
