using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Manager : Singleton<Level_Manager>
{
    [SerializeField]
    int numLevels = 2;

    int currentLevel = 1;

    List<int> completedLevels;

    public float transitionTime = 1f;

    public GameObject fadeCanvasPrefab;
    private GameObject fadeCanvas;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            LoadNextLevel();
        }
    }

    public void LevelCompleted(int level) {
        completedLevels.Add(level);
    }

    public List<int> GetCompletedLevels() {
        return completedLevels;
    }

    public void LoadNextLevel() {
        Debug.Log("Loading next level");
        if (currentLevel < numLevels)
            currentLevel++;

        // Load scene at the next build index
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadCurrentLevel() {
        Debug.Log("LOADING CURRENT LEVEL");
        StartCoroutine(LoadLevel(currentLevel));
    }

    IEnumerator LoadLevel(int levelIndex) {
        Debug.Log("Loading Level: " + levelIndex);
        GameObject fadeCanvas = CreateFadeObject();

        Animator fadeAnim = fadeCanvas.GetComponent<Animator>();

        // Fade Out
        if (fadeAnim)
            fadeAnim.SetTrigger("FadeOut");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(levelIndex);

        // Fade In
        if (fadeAnim)
            fadeAnim.SetTrigger("FadeIn");

        // Wait
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
