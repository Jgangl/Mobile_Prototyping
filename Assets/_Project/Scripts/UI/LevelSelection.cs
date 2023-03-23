using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Pagination;

public class LevelSelection : MonoBehaviour
{
    const string LEVEL_SELECT_BUTTON_NAME = "LevelSelectSingleButton";
    
    [SerializeField] GameObject levelsPanel;
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] GameObject levelButtonsParent_Page_1;
    [SerializeField] GameObject levelButtonsParent_Page_2;
    [SerializeField] PagedRect paginationRect;
    [SerializeField] Button prevPageButton;
    [SerializeField] Button nextPageButton;
    
    int numLevels;
    List<LevelSelectButton> levelButtons;

    // Start is called before the first frame update
    void Start()
    {
        Level_Manager.Instance.OnLevelCompleted += Level_Manager_OnLevelCompleted;
        numLevels = Level_Manager.Instance.NumLevels;
        levelButtons = new List<LevelSelectButton>();

        AddLevelButtons();
        UpdateLevelCompletionIcons();
        
        paginationRect.SetCurrentPage(1, true);

        paginationRect.PageChangedEvent.AddListener(PageChanged);
    }

    void AddLevelButtons() 
    {
        foreach (LevelSelectButton button in levelButtonsParent_Page_1.GetComponentsInChildren<LevelSelectButton>())
        {
            Destroy(button.gameObject);
        }

        for (int i = 1; i <= numLevels; i++) 
        {
            GameObject levelButtonObject = Instantiate(levelButtonPrefab, levelButtonsParent_Page_1.transform);
            levelButtonObject.name = "Level_" + i;

            LevelSelectButton currentButton = levelButtonObject.GetComponent<LevelSelectButton>();
            currentButton.Setup(i, OnLevelButtonPressed);
            levelButtons.Add(currentButton);
        }
        
        // Unlock level 1
        levelButtons[0].Unlock();
    }

    void OnLevelButtonPressed(int level) 
    {
        Level_Manager.Instance.LoadLevel(level);
    }

    void UpdateLevelCompletionIcons() 
    {
        Level[] levels = Level_Manager.Instance.GetLevels();

        if (levels == null)
            return;

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].completed)
            {
                levelButtons[i].Unlock();
                levelButtons[i].Complete();

                if (i < levels.Length - 1)
                {
                    // Unlock the next level
                    levelButtons[i + 1].Unlock();
                }
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
        GetLevelButton(levelCompleted).Complete();
        
        // Unlock next level
        LevelSelectButton nextLevelButton = GetLevelButton(levelCompleted + 1);
        if (nextLevelButton)
        {
            nextLevelButton.Unlock();
        }
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
        
        UpdatePaginationButtons();
    }

    void UpdatePaginationButtons()
    {
        int numPages = paginationRect.NumberOfPages;
        int currPage = paginationRect.CurrentPage;

        prevPageButton.interactable = currPage > 1;
        nextPageButton.interactable = currPage < numPages;
    }

    void PageChanged(Page prevPage, Page currPage)
    {
        UpdatePaginationButtons();
    }
}
