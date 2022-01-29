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

    int currentLevel = 1;

    List<int> completedLevels;

    public float transitionTime = 1f;

    public GameObject fadeCanvasPrefab;
    private GameObject fadeCanvas;

    private Scene managersUIScene;
    private Scene openLevelScene;

    private void Start() {
        completedLevels = new List<int>();

        managersUIScene = SceneManager.GetSceneAt(0);
        openLevelScene = GetCurrentOpenScene();
        currentLevel = openLevelScene.buildIndex;
        
        print(currentLevel);

        //StartCoroutine(LoadLevelCoroutine(3));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            LoadNextLevel();
        }
    }

    public void CompleteLevel(int level) {
        Debug.Log("Level " + level + " completed");
        completedLevels.Add(level);
        LoadNextLevel();
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
        print(currentLevel);
        if (currentLevel < numLevels) {
            currentLevel++;
            // Load scene at the next build index
            LoadLevel(currentLevel);
        }
        else {
            Debug.Log("No more levels");
        }
    }

    public void RestartLevel() {
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel() {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelIndex) {
        currentLevel = levelIndex;
        if (currentLevel > numLevels)
            return;
        
        print(currentLevel);

        print("Load level: " + levelIndex);
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    private Scene GetCurrentOpenScene()
    {
        if (SceneManager.sceneCount > 2)
            return SceneManager.GetSceneAt(1);

        return new Scene();
    }

    private void UnloadPreviousScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == managersUIScene) continue;
            
            UnloadScene(scene);
        }
    }
    
    private void UnloadScene(Scene sceneToUnload)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
        //asyncOperation.completed += 
    }

    IEnumerator LoadLevelCoroutine(int levelIndex) {
        GameObject fadeCanvas = CreateFadeObject();

        Animator fadeAnim = fadeCanvas.GetComponent<Animator>();

        // Fade Out
        if (fadeAnim)
            fadeAnim.SetTrigger("FadeOut");

        // Wait time for fade out
        yield return new WaitForSeconds(transitionTime);
        
        // Unload previous scenes
        UnloadPreviousScenes();

        // Load new scene
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Additive);

        openLevelScene = GetCurrentOpenScene();

        //UI_Manager.Instance.EnableSettingsButton(true);
        //UI_Manager.Instance.EnableRestartButton(true);

        // Fade In
        if (fadeAnim)
            fadeAnim.SetTrigger("FadeIn");

        // Wait time for fade in
        yield return new WaitForSeconds(transitionTime);
        // Destroy fadeCanvas
        Destroy(fadeCanvas);
    }

    private GameObject CreateFadeObject() {
        GameObject fadeCanvas = Instantiate(fadeCanvasPrefab);
        // Ensure the fadeCanvas doesn't get destroyed when loading new scene
        DontDestroyOnLoad(fadeCanvas);

        return fadeCanvas;
    }

    private void DestroyFadeObject() {
        if (fadeCanvas)
            Destroy(fadeCanvas);
    }
}
