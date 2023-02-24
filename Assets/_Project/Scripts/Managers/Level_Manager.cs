using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Level_Manager : Singleton<Level_Manager> {
    
    [SerializeField]
    int numLevels  = 2;
    public int NumLevels 
    {
        get => numLevels;
    }

    Level[] levels;
    int currentLevel = 1;
    int highestCompletedLevel = 0;
    List<int> completedLevels;
    Scene managersUIScene;
    bool loadingLevel = false;
    bool levelCompleted = false;
    public float transitionTime = 1f;
    public GameObject fadeCanvasPrefab;

    public Action OnLevelLoaded;
    public Action<int> OnLevelCompleted;
    public Action OnLevelReset;

    Transform playerSpawnPoint;
    PlayerController player;

    protected override void Awake()
    {
        base.Awake();
        
        managersUIScene = SceneManager.GetSceneAt(0);

        Scene currentOpenScene = managersUIScene;
        if (SceneManager.sceneCount >= 2)
            currentOpenScene = SceneManager.GetSceneAt(1);
        
        if (currentOpenScene != managersUIScene)
        {
            currentLevel = currentOpenScene.buildIndex;
        }
        else
        {
            currentLevel = 1;
        }
        
        numLevels = SceneManager.sceneCountInBuildSettings - 1;
        
        InitializeLevels();
    }

    void Start()
    {
        completedLevels = new List<int>();

        Initialize();
    }

    public bool TryLoadLevels(Level[] savedLevels)
    {
        if (savedLevels == null)
        {
            return false;
        }

        foreach (Level level in savedLevels)
        {
            if (level.levelNumber <= levels.Length)
            {
                levels[level.levelNumber - 1] = level;
            }
        }
        
        levels = savedLevels;
        return true;
    }

    public void InitializeLevels()
    {
        levels = new Level[numLevels];
        
        // Try to load, otherwise initialize default values
        
        for (int i = 0; i < numLevels; i++)
        {
            levels[i] = new Level(i + 1);
        }
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.N)) 
        {
            LoadNextLevel();
        }
    }

    public void CompleteLevel(int level) 
    {
        Debug.Log("Level " + level + " completed");

        int currNumJumps = GameManager.Instance.GetNumJumpsThisLevel();
        double currTime = GameManager.Instance.GetTimeThisLevel();
        
        Level currLevel = GetLevel(level);
        int bestNumJumps = currLevel.bestNumOfJumps;
        double bestTime = currLevel.bestTime;

        if (currNumJumps < bestNumJumps)
        {
            // New Jump high score
            SetLevelBestNumJumps(level, currNumJumps);
        }

        if (currTime < bestTime)
        {
            // New time high score
            SetLevelBestTime(level, currTime);
        }

        SetLevelCompleted(level, true);
        levelCompleted = true;
        
        AudioManager.Instance.PlayWinSound();
        
        // Save game after a level is completed
        GameManager.Instance.SaveGame();

        OnLevelCompleted?.Invoke(level);
    }

    public void CompleteCurrentLevel() 
    {
        CompleteLevel(currentLevel);
    }

    public int GetNumCompletedLevels()
    {
        int numCompletedLevels = 0;
        
        foreach (Level level in levels)
        {
            if (level.completed)
            {
                numCompletedLevels++;
            }
        }

        return numCompletedLevels;
    }

    public void SetCompletedLevels(List<int> completedLevels) 
    {
        this.completedLevels = completedLevels;
    }

    public void LoadNextLevel() 
    {
        if (currentLevel < numLevels) 
        {
            currentLevel++;
            // Load scene at the next build index
            LoadLevel(currentLevel);
        }
        else {
            Debug.Log("No more levels");
        }
    }

    public void LoadCurrentLevel() 
    {
        LoadLevel(currentLevel);
    }

    public void LoadFirstUncompletedLevel()
    {
        int level = GetFirstUncompletedLevel();
        
        LoadLevel(level);
    }

    public void LoadLevel(int levelIndex) 
    {
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

    int GetCurrentOpenLevel()
    {
        if (SceneManager.sceneCount >= 2)
            return GetLevelFromSceneName(SceneManager.GetSceneAt(1).name);

        return -1;
    }

    int GetLevelFromSceneName(string name)
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

    void UpdateCurrentLevel()
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
    
    public bool LevelJustStarted()
    {
        return GameManager.Instance.GetTimeThisLevel() < 0.5f;
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
    
    void UnloadScene(Scene sceneToUnload)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
    }
    
    void LoadScene(int sceneToLoad)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncOperation.completed += OnSceneDoneLoading;
    }

    void OnSceneDoneLoading(AsyncOperation asyncOperation)
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

    void Initialize()
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

    IEnumerator RestartLevelRoutine()
    {
        yield return Fader.Instance.FadeOutCoroutine(1f);

        // Reset player position
        player.Reset(playerSpawnPoint.position);
        
        TimeDilator.ResumeNormalTime();

        OnLevelReset?.Invoke();
        
        levelCompleted = false;

        yield return Fader.Instance.FadeInCoroutine(1.5f);
    }

    [Button("Update Build Levels")]
    public void UpdateBuildLevels()
    {
        Debug.Log("Updating build scenes");
        
        string scenesDir = "Assets/_Project/Scenes/";
        string levelsDir = scenesDir + "Levels/";
        string uiScenePath = scenesDir + "Managers_UI.unity";

        DirectoryInfo levelsInfo = new DirectoryInfo(levelsDir);
        FileInfo[] levelsFileInfo = levelsInfo.GetFiles();
        List<FileInfo> levelFiles = new List<FileInfo>();

        foreach (FileInfo fileInfo in levelsFileInfo)
        {
            bool metaFile = fileInfo.Name.Contains(".meta");

            if (metaFile) continue;
            
            levelFiles.Add(fileInfo);
        }

        // Number of levels + UI scene
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[levelFiles.Count + 1];
        
        scenes[0] = new EditorBuildSettingsScene(uiScenePath, true);
        Debug.Log("  " + uiScenePath);

        for (int i = 1; i < scenes.Length; i++)
        {
            string levelPath = levelsDir + levelFiles[i - 1].Name;
            scenes[i] = new EditorBuildSettingsScene(levelPath, true);
            
            Debug.Log("  " + levelPath);
        }

        // Set editor build settings scenes
        EditorBuildSettings.scenes = scenes;
    }

    [Title("Scene Renaming (CAUTION)", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [Button("Increment Levels")]
    public void InrementLevelNumbers()
    {
        Debug.Log("Incrementing levels");

        string projectDirectory = Application.dataPath;
        
        // Remove Assets folder
        projectDirectory = projectDirectory.Replace("/Assets", "");

        List<string> levelsToIncrement = new List<string>();
        
        foreach (UnityEngine.Object o in Selection.objects)
        {
            if (o.GetType() == typeof(SceneAsset))
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                string fullFilePath = projectDirectory + "/" + assetPath;
                
                levelsToIncrement.Add(fullFilePath);
            }
        }

        for (int i = levelsToIncrement.Count - 1; i >= 0; i--)
        {
            string fileName = levelsToIncrement[i];
            // Remove .unity from end of file
            if (fileName.Contains(".unity"))
                fileName = fileName.Remove(fileName.Length - 6);
            
            int lastIndexOfSplit = fileName.LastIndexOf("/");
            string levelName     = fileName.Substring(lastIndexOfSplit);
            string filePath      = fileName.Substring(0, lastIndexOfSplit);

            string[] levelAndNumber = levelName.Split("_");

            if (!int.TryParse(levelAndNumber[1], out int levelNum))
            {
                Debug.Log(levelAndNumber[1] + " is not a valid number");
                return;
            }

            int numChange = 1;
            levelNum = levelNum + numChange;
                
            string newLevelName = levelAndNumber[0] + "_" + levelNum.ToString();

            string newFullUnityFileName = filePath + newLevelName + ".unity";
            string newFullMetaFileName = newFullUnityFileName + ".meta";

            if (File.Exists(newFullUnityFileName) || File.Exists(newFullMetaFileName))
            {
                Debug.LogError("Scene or meta file already exists: " + newFullUnityFileName);
                return;
            }
            
            // Rename scene file
            File.Move(levelsToIncrement[i], newFullUnityFileName);
            
            // Rename .meta file
            File.Move(levelsToIncrement[i] + ".meta", newFullMetaFileName);
        }
        
        AssetDatabase.Refresh();
    }
    
    [Title("Scene Renaming (CAUTION)", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [Button("Decrement Levels")]
    public void DecrementLevelNumbers()
    {
        Debug.Log("Deccrementing levels");

        string projectDirectory = Application.dataPath;
        
        // Remove Assets folder
        projectDirectory = projectDirectory.Replace("/Assets", "");

        List<string> levelsToIncrement = new List<string>();
        
        foreach (UnityEngine.Object o in Selection.objects)
        {
            if (o.GetType() == typeof(SceneAsset))
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                string fullFilePath = projectDirectory + "/" + assetPath;
                
                levelsToIncrement.Add(fullFilePath);
            }
        }

        for (int i = 0; i < levelsToIncrement.Count; i++)
        {
            string fileName = levelsToIncrement[i];
            // Remove .unity from end of file
            if (fileName.Contains(".unity"))
                fileName = fileName.Remove(fileName.Length - 6);
            
            int lastIndexOfSplit = fileName.LastIndexOf("/");
            string levelName     = fileName.Substring(lastIndexOfSplit);
            string filePath      = fileName.Substring(0, lastIndexOfSplit);

            string[] levelAndNumber = levelName.Split("_");

            if (!int.TryParse(levelAndNumber[1], out int levelNum))
            {
                Debug.Log(levelAndNumber[1] + " is not a valid number");
                return;
            }

            int numChange = -1;
            levelNum = levelNum + numChange;
                
            string newLevelName = levelAndNumber[0] + "_" + levelNum.ToString();

            string newFullUnityFileName = filePath + newLevelName + ".unity";
            string newFullMetaFileName = newFullUnityFileName + ".meta";

            if (File.Exists(newFullUnityFileName) || File.Exists(newFullMetaFileName))
            {
                Debug.LogError("Scene or meta file already exists: " + newFullUnityFileName);
                return;
            }
            
            // Rename scene file
            File.Move(levelsToIncrement[i], newFullUnityFileName);
            
            // Rename .meta file
            File.Move(levelsToIncrement[i] + ".meta", newFullMetaFileName);
        }
        
        AssetDatabase.Refresh();
    }

    public int GetFirstUncompletedLevel()
    {
        foreach (Level level in levels)
        {
            if (!level.completed)
            {
                return level.levelNumber;
            }
        }
        
        return 1;
    }

    private bool SetLevelCompleted(int level, bool completed)
    {
        Level currLevel = GetLevel(level);
        currLevel.completed = completed;
        SetLevel(level, currLevel);
        
        return true;
    }
    
    private bool SetLevelBestNumJumps(int level, int numOfJumps)
    {
        Level currLevel = GetLevel(level);
        currLevel.bestNumOfJumps = numOfJumps;
        SetLevel(level, currLevel);
        
        return true;
    }
    
    private bool SetLevelBestTime(int level, double bestTime)
    {
        Level currLevel = GetLevel(level);
        currLevel.bestTime = bestTime;
        SetLevel(level, currLevel);
        
        return true;
    }

    public Level[] GetLevels()
    {
        return levels;
    }
    
    public Level GetLevel(int level)
    {
        int index = level - 1;

        if (levels == null || index > levels.Length || index < 0)
        {
            return new Level();
        }
        
        return levels[index];
    }
    
    public bool SetLevel(int level, Level newLevel)
    {
        int index = level - 1;

        if (levels == null || index > levels.Length || index < 0)
        {
            return false;
        }

        levels[index] = newLevel;
        return true;
    }

    public int GetCurrentLevelNumber()
    {
        return currentLevel;
    }

    public Level GetCurrentLevel()
    {
        return GetLevel(currentLevel);
    }
}
