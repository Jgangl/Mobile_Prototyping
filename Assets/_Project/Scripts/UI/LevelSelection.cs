using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    const string LEVEL_SELECT_BUTTON_NAME = "LevelSelectSingleButton";
    
    [SerializeField] GameObject levelsPanel;
    [SerializeField] GameObject levelButtonPrefab;
    int numLevels;
    List<LevelSelectButton> levelButtons;

    // Start is called before the first frame update
    void Start()
    {
        Level_Manager.Instance.OnLevelCompleted += Level_Manager_OnLevelCompleted;
        numLevels = Level_Manager.Instance.NumLevels;
        levelButtons = new List<LevelSelectButton>();

        AddLevelButtons();
    }

    void AddLevelButtons() 
    {
        foreach (LevelSelectButton button in levelsPanel.GetComponentsInChildren<LevelSelectButton>())
        {
            Destroy(button.gameObject);
        }

        for (int i = 1; i <= numLevels; i++) 
        {
            GameObject levelButtonObject = Instantiate(levelButtonPrefab, levelsPanel.transform);
            levelButtonObject.name = "Level_" + i;

            LevelSelectButton currentButton = levelButtonObject.GetComponent<LevelSelectButton>();
            currentButton.Setup(i, OnLevelButtonPressed);
            levelButtons.Add(currentButton);
        }
        
        // Unlock level 1
        Debug.Log("Unlock level 1");
        levelButtons[0].Unlock();
    }

    void OnLevelButtonPressed(int level) 
    {
        Level_Manager.Instance.LoadLevel(level);
    }

    void UpdateLevelCompletionIcons() 
    {
        Debug.Log("updating level complete icons");
        List<int> completedLevels = Level_Manager.Instance.GetCompletedLevels();

        if (completedLevels == null)
            return;

        foreach(LevelSelectButton button in levelButtons) {
            if (completedLevels.Contains(button.GetLevel())) {
                 button.Unlock();
                 button.Complete();
            }
        }
    }

    LevelSelectButton GetLevelButton(int level)
    {
        if (levelButtons.Count > level - 1)
            return levelButtons[level - 1];

        return null;
    }

    void Level_Manager_OnLevelCompleted(int levelCompleted)
    {
        Debug.Log("OnLevelCompleted: " + levelCompleted);
        GetLevelButton(levelCompleted).Complete();
        
        // Unlock next level
        LevelSelectButton nextLevelButton = GetLevelButton(levelCompleted + 1);
        if (nextLevelButton)
            nextLevelButton.Unlock();
        else
        {
            Debug.Log("Next Level Button null");
        }
    }

    public void EnableLevelSelectionMenu(bool enabled, bool withAnimation)
    {
        Animator anim = GetComponent<Animator>();
        if (!anim)
            return;
        
        if (withAnimation)
        {
            if (enabled)
                anim.SetTrigger("Open");
            else
                anim.SetTrigger("Close");
        }
        else
        {
            if (enabled)
                anim.SetTrigger("Open");
            else
                anim.SetTrigger("Close_Instant");
        }
    }
}
