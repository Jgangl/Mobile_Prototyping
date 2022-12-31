using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Manager : Singleton<Level_Manager> {
    
    [SerializeField]
    int numLevels  = 2;
    public int NumLevels {
        get => numLevels;
    }

    private int currentLevel = 1;
    private int highestCompletedLevel = 0;
    private List<int> completedLevels;
    private Scene managersUIScene;
    private bool loadingLevel = false;
    private bool levelCompleted = false;
    public float transitionTime = 1f;
    public GameObject fadeCanvasPrefab;

    public Action OnLevelLoaded;
    public Action<int> OnLevelCompleted;
    public Action OnLevelReset;

    private Transform playerSpawnPoint;
    private PlayerController player;

    private void Start() 
    {
        completedLevels = new List<int>();

        managersUIScene = SceneManager.GetSceneAt(0);

        Scene currentOpenScene = managersUIScene;
        if (SceneManager.sceneCount > 2)
            currentOpenScene = SceneManager.GetSceneAt(1);
        
        if (currentOpenScene != managersUIScene)
        {
            currentLevel = currentOpenScene.buildIndex;
        }
        else
        {
            currentLevel = 1;
        }
        
        Initialize();

        //UpdateCurrentLevel();
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.N)) {
            LoadNextLevel();
        }
    }

    public void CompleteLevel(int level) 
    {
        Debug.Log("Level " + level + " completed");
        if (level > highestCompletedLevel)
        {
            UpdateHighestCompletedLevel(level);
        }

        OnLevelCompleted?.Invoke(level);
        completedLevels.Add(level);
        
        levelCompleted = true;
    }

    private void UpdateHighestCompletedLevel(int newHighLevel)
    {
        highestCompletedLevel = newHighLevel;
        // Update level selection buttons
    }

    public void CompleteCurrentLevel() {
        CompleteLevel(currentLevel);
    }

    public List<int> GetCompletedLevels() {
        return completedLevels;
    }

    public void SetCompletedLevels(List<int> completedLevels) {
        this.completedLevels = completedLevels;
    }

    public void LoadNextLevel() {
        if (currentLevel < numLevels) {
            currentLevel++;
            // Load scene at the next build index
            LoadLevel(currentLevel);
        }
        else {
            Debug.Log("No more levels");
        }
    }

    public void LoadCurrentLevel() {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelIndex) {
        currentLevel = levelIndex;
        if (currentLevel > numLevels)
            return;

        print("Load level: " + levelIndex);
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    public bool IsLoadingLevel()
    {
        return loadingLevel;
    }

    private int GetCurrentOpenLevel()
    {
        if (SceneManager.sceneCount >= 2)
            return GetLevelFromSceneName(SceneManager.GetSceneAt(1).name);

        return -1;
    }

    private int GetLevelFromSceneName(string name)
    {
        string[] splitName = name.Split('_');
        foreach(string val in splitName) 
        {
            int levelNum = -1;
            if (int.TryParse(val, out levelNum)) 
            {
                if (levelNum != -1)
                {
                    return levelNum;
                }
            }
        }

        return -1;
    }

    private void UpdateCurrentLevel()
    {
        int level = GetCurrentOpenLevel();
        if (level != -1)
        {
            currentLevel = level;
            OnLevelLoaded.Invoke();
        }
    }

    public bool IsLevelCompleted()
    {
        return levelCompleted;
    }

    public void UnloadPreviousScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == managersUIScene) continue;
            
            UnloadScene(scene);
        }
    }
    
    public Scene GetCurrentOpenScene()
    {
        Scene scene = new Scene();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene currScene = SceneManager.GetSceneAt(i);
            if (currScene == managersUIScene) continue;

            scene = currScene;
            return scene;
        }

        return scene;
    }
    
    private void UnloadScene(Scene sceneToUnload)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
    }
    
    private void LoadScene(int sceneToLoad)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncOperation.completed += OnSceneDoneLoading;
    }

    private void OnSceneDoneLoading(AsyncOperation asyncOperation)
    {
        asyncOperation.completed -= OnSceneDoneLoading;
        
        Scene openScene = GetCurrentOpenScene();
        if (openScene.rootCount > 0)
        {
            SceneManager.SetActiveScene(openScene);
        }
        
        // Get player spawn point
        Initialize();
    }

    private void Initialize()
    {
        GameObject spawnPoint = GameObject.Find("SpawnPoint");
        if (spawnPoint)
            playerSpawnPoint = GameObject.Find("SpawnPoint").transform;

        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if (localPlayer)
            player = localPlayer.GetComponent<PlayerController>();
    }

    IEnumerator LoadLevelCoroutine(int levelIndex)
    {
        loadingLevel = true;

        yield return Fader.Instance.FadeOutCoroutine(2f);

        // Unload previous scenes
        UnloadPreviousScenes();
        
        // Load new scene
        LoadScene(levelIndex);

        yield return new WaitForSeconds(0.05f);
        UpdateCurrentLevel();

        levelCompleted = false;
        loadingLevel = false;

        TimeDilator.ResumeNormalTime();
        
        Fader.Instance.FadeIn(1f);

        // Wait time for fade in
        yield return new WaitForSeconds(transitionTime);
    }

    public void RestartCurrentLevel()
    {
        StartCoroutine(RestartLevelRoutine());
    }

    private IEnumerator RestartLevelRoutine()
    {
        yield return Fader.Instance.FadeOutCoroutine(2f);

        // Reset player position
        player.Reset(playerSpawnPoint.position);
        
        TimeDilator.ResumeNormalTime();
        
        OnLevelReset?.Invoke();

        yield return Fader.Instance.FadeInCoroutine(1.5f);
    }

    public int GetHighestCompletedLevel()
    {
        return highestCompletedLevel;
    }
}
