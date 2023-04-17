using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField] SettingsMenu settingsMenu;
    [SerializeField] AfterLevelMenu afterLevelMenu;
    [SerializeField] LevelSelection levelSelection;
    [SerializeField] GameObject levelBanner;
    [SerializeField] TextMeshProUGUI levelBannerText;
    [SerializeField] GameObject inLevelUI;
    [SerializeField] GameObject mainMenu;
    [SerializeField] Button startButton;
    [SerializeField] float levelBannerShowTime = 1.5f;

    Menu currentOpenMenu;
    PlayerController player;

    bool isLevelSelectionMenuOpen = false;
    //bool isSettingsMenuOpen = false;
    bool isMenuOpened = false;
    bool isMainMenuOpened = true;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Level_Manager.Instance.OnLevelCompleted += LevelManager_OnLevelCompleted;
        Level_Manager.Instance.OnLevelReset += LevelManager_OnLevelReset;

        startButton.onClick.AddListener(OnStartButtonPressed);
        UpdateStartButton();
    }

    public void OnStartButtonPressed()
    {
        startButton.interactable = false;
        GameManager.Instance.StartGame();
    }

    public void OnLevelSettingsButtonPressed()
    {
        DisablePlayerInput();
        
        GameManager.Instance.PauseGame(true);
        
        settingsMenu.Open();
        settingsMenu.EnableHomeButton(true);
    }

    public void OnQuitButtonPressed() 
    {
        GameManager.Instance.QuitGame();
    }

    public void EnableLevelSelectionMenu(bool enabled, bool withAnimation)
    {
        if (enabled != isLevelSelectionMenuOpen)
            levelSelection.EnableLevelSelectionMenu(enabled, withAnimation);
        
        isLevelSelectionMenuOpen = enabled;
        UpdateMenuStatus(enabled);
    }

    void EnableInLevelUI(bool enabled)
    {
        inLevelUI.SetActive(enabled);
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null) Debug.Log("Player was null");

        ResetLevelBanner();

        levelBannerText.fontSize = 100.0f;
        levelBannerText.text = "Level " + Level_Manager.Instance.GetCurrentLevel()?.levelNumber;
        // Show level banner
        Invoke("OpenLevelBanner", 0.5f);
        // Close banner after delay
        Invoke("CloseLevelBanner", levelBannerShowTime);
        
        EnableMainMenu(false);
        EnableLevelSelectionMenu(false, false);
        afterLevelMenu.CloseInstant();
        EnableInLevelUI(true);
        SetInLevelUIButtonsClickable(true);
    }

    void LevelManager_OnLevelCompleted(int levelCompleted)
    {
        SetInLevelUIButtonsClickable(false);
        
        StartCoroutine("LevelCompleteRoutine");
    }
    
    void LevelManager_OnLevelReset()
    {
        afterLevelMenu.CloseInstant();
        isMenuOpened = false;
        UpdateMenuStatus(false);
    }

    IEnumerator LevelCompleteRoutine()
    {
        UpdateMenuStatus(true);

        TimeDilator.SlowTimeIndefinitely(0.1f);
        
        // Show level complete banner
        levelBannerText.fontSize = 80.0f;
        levelBannerText.text = "Level Complete";
        OpenLevelBanner();

        yield return CanvasFader.Instance.FadeToCoroutine(0.35f, 1f);

        // Go to next level

        yield return new WaitForSecondsRealtime(2.0f);

        Level_Manager.Instance.LoadNextLevel();

        //afterLevelMenu.Open();

        //CanvasFader.Instance.FadeInInstant();
    }

    void EnableMainMenu(bool enabled)
    {
        isMainMenuOpened = enabled;
        mainMenu.SetActive(enabled);
        
        startButton.interactable = enabled;
        UpdateStartButton();
    }

    void UpdateMenuStatus(bool menuOpened) 
    {
        isMenuOpened = menuOpened;
        GameManager.Instance.SetMenuOpened(isMenuOpened);
    }
    
    void OpenLevelBanner()
    {
        levelBanner.GetComponent<Animator>()?.SetTrigger("Start");
    }

    void CloseLevelBanner()
    {
        levelBanner.GetComponent<Animator>()?.SetTrigger("Close");
    }

    void ResetLevelBanner()
    {
        Animator anim = levelBanner.GetComponent<Animator>();
        anim?.SetTrigger("Reset");
        anim?.ResetTrigger("Close");
    }

    public void SetInLevelUIButtonsClickable(bool clickable)
    {
        Button[] inlevelUIButtons = inLevelUI.GetComponentsInChildren<Button>();

        foreach (Button inLevelUIButton in inlevelUIButtons)
        {
            inLevelUIButton.interactable = clickable;
        }
    }
    
    public void OpenLevelSelectionMenu() 
    {
        EnableLevelSelectionMenu(true, true);
    }
    
    public void CloseLevelSelectionMenu() 
    {
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

    void UpdateStartButton()
    {
        int numCompletedLevels = Level_Manager.Instance.GetNumCompletedLevels();

        TextMeshProUGUI startButtonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        if (!startButtonText)
        {
            return;
        }

        if (numCompletedLevels == 0)
        {
            startButtonText.text = "Start";
            startButtonText.fontSize = 75;
        }
        else
        {
            startButtonText.text = "Continue";
            startButtonText.fontSize = 60;
        }
    }

    public void OnRestartButtonPressed() 
    {
        Level_Manager.Instance.RestartCurrentLevel();
    }

    public void OnMusicSwitch_On() 
    {
        print("Music Switch ON");
        AudioManager.Instance.OnMusicChecked(true);
    }

    public void OnMusicSwitch_Off() 
    {
        print("Music Switch OFF");
        AudioManager.Instance.OnMusicChecked(false);
    }

    public void OnSoundFXSwitch_On() 
    {
        print("SFX Switch ON");
        AudioManager.Instance.OnSoundFXChecked(true);
    }

    public void OnSoundFXSwitch_Off() 
    {
        print("SFX Switch OFF");
        AudioManager.Instance.OnSoundFXChecked(false);
    }
    
    public void OnCameraShakeSwitch_On() 
    {
        print("Camera Shake Switch ON");
        CinemachineShake.Instance.EnableCameraShake();
    }

    public void OnCameraShakeSwitch_Off() 
    {
        print("Camera Shake Switch OFF");
        CinemachineShake.Instance.DisableCameraShake();
    }

    public void OnMainMenuButtonPressed()
    {
        StartCoroutine(ReturnToMainMenuRoutine());
    }

    IEnumerator ReturnToMainMenuRoutine()
    {
        yield return CanvasFader.Instance.FadeOutCoroutine(0.5f);

        if (currentOpenMenu)
            currentOpenMenu.CloseInstant();

        ResetLevelBanner();
        
        // Unload previous scenes
        Level_Manager.Instance.UnloadPreviousScenes();
        
        EnableMainMenu(true);
        EnableLevelSelectionMenu(false, false);
        EnableInLevelUI(false);
        TimeDilator.ResumeNormalTime();
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(CanvasFader.Instance.FadeInCoroutine(1f));
    }

    public void SetCurrentOpenMenu(Menu openMenu)
    {
        currentOpenMenu = openMenu;
    }

    void EnablePlayerInput()
    {
        if (player)
            player.EnableInput(true);
    }

    void DisablePlayerInput()
    {
        if (player)
        {
            player.EnableInput(false);
        }
    }

    public bool IsPositionWithinLevelUI(Vector2 screenPosition)
    {
        RectTransform InLevelUIRect = inLevelUI.GetComponent<RectTransform>();
        if (InLevelUIRect)
        {
            Vector2 worldPosiion = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 localSpacePosition = InLevelUIRect.InverseTransformPoint(worldPosiion);

            return InLevelUIRect.rect.Contains(localSpacePosition);
        }

        return false;
    }
}
