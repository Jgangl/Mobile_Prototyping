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
    [SerializeField] int buttonsPerPage;
    [SerializeField] List<GameObject> objectsToDisable;
    [SerializeField] GameObject viewport;
    [SerializeField] float disableDelay;
    
    int numLevels;
    List<LevelSelectButton> levelButtons;

    int totalNumberOfPages;

    int targetPage = 1;

    bool prevUnlockAllLevels;

    // Start is called before the first frame update
    void Start()
    {
        Level_Manager.Instance.OnLevelCompleted += Level_Manager_OnLevelCompleted;
        numLevels = Level_Manager.Instance.NumLevels;
        levelButtons = new List<LevelSelectButton>();

        AddLevelButtons();
        UpdateLevelCompletionIcons();

        for (int i = 1; i <= paginationRect.Pages.Count; i++)
        {
            paginationRect.SetCurrentPage(i);
        }

        paginationRect.PageChangedEvent.AddListener(PageChanged);
        
        float numLevelsFloat = Level_Manager.Instance.NumLevels / (float)buttonsPerPage;

        totalNumberOfPages = Mathf.CeilToInt(numLevelsFloat);
    }

    void Update()
    {
        if (Level_Manager.Instance.unlockAllLevels && !prevUnlockAllLevels)
        {
            UnlockAllButtons();
        }

        prevUnlockAllLevels = Level_Manager.Instance.unlockAllLevels;
    }

    void AddLevelButtons() 
    {
        // I should really do the pages better but I am not going to
        foreach (Transform t in levelButtonsParent_Page_1.transform)
        {
            Destroy(t.gameObject);
        }
        
        foreach (Transform t in levelButtonsParent_Page_2.transform)
        {
            Destroy(t.gameObject);
        }

        for (int i = 1; i <= numLevels; i++) 
        {
            float numLevelsFloat = i / (float)buttonsPerPage;

            int currPage = Mathf.CeilToInt(numLevelsFloat);

            Transform levelButtonsParentTransform = levelButtonsParent_Page_1.transform;
            
            switch (currPage)
            {
                case 1:
                    levelButtonsParentTransform = levelButtonsParent_Page_1.transform;
                    break;
                case 2:
                    levelButtonsParentTransform = levelButtonsParent_Page_2.transform;
                    break;
                //case 3:
                    //levelButtonsParentTransform = levelButtonsParent_Page_3.transform;
            }
            
            GameObject levelButtonObject = Instantiate(levelButtonPrefab, levelButtonsParentTransform);
            levelButtonObject.name = "Level_" + i;
            
            //levelButtonObject.SetActive(false);

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
            {
                DisableObjects();
                anim.SetTrigger("Close_Instant");
            }
        }
        
        UpdatePaginationButtons();

        if (enabled)
        {
            StartCoroutine("EnableObjectsCoroutine");
            Invoke("UpdateCurrentPage", 0.05f);
        }
        else
        {
            Invoke("DisableObjects", disableDelay);
        }
            
    }

    void UpdateCurrentPage()
    {
        CalculateTargetPage();

        SetCurrentPageToTargetPage();
        
        UpdatePaginationButtons();
    }

    void CalculateTargetPage()
    {
        int currentLevel = Level_Manager.Instance.GetCurrentLevelNumber();
        
        float numLevelsFloat = currentLevel / (float)buttonsPerPage;

        targetPage = Mathf.CeilToInt(numLevelsFloat);
    }

    void SetCurrentPageToTargetPage()
    {
        // Needs to be initial for some reason otherwise the coroutine tanks on Android...
        paginationRect.SetCurrentPage(targetPage, true);
    }

    void UpdatePaginationButtons()
    {
        int numPages = totalNumberOfPages;
        int currPage = paginationRect.CurrentPage;

        prevPageButton.interactable = currPage > 1;
        nextPageButton.interactable = currPage < numPages;
    }

    void PageChanged(Page prevPage, Page currPage)
    {
        UpdatePaginationButtons();
    }

    public IEnumerator EnableObjectsCoroutine()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(true);
            yield return null;
        }
  
        ShowAllPages();

        yield return null;
    }

    public IEnumerator DisableObjectsCoroutine()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
            yield return null;
        }
        
        HideAllPages();
        
        yield return null;
    }

    public void DisableObjects()
    {
        StartCoroutine("DisableObjectsCoroutine");
    }

    public void UnlockAllButtons()
    {
        foreach (LevelSelectButton levelButton in levelButtons)
        {
            levelButton.Unlock();
        }
    }

    public void ShowPage(int pageNumber)
    {
        if (pageNumber < paginationRect.Pages.Count && pageNumber >= 0)
        {   
            Page page = paginationRect.Pages[pageNumber];
            if (page)
            {
                Transform levelButtonsParent = page.transform.Find("LevelButtons");
                if (levelButtonsParent)
                {
                    CanvasGroup pageCanvasGroup = levelButtonsParent.GetComponent<CanvasGroup>();
                    if (pageCanvasGroup)
                    {
                        pageCanvasGroup.alpha = 1;
                        pageCanvasGroup.interactable = true;
                        pageCanvasGroup.blocksRaycasts = true;
                    }
                }

                LevelSelectButton[] pageButtons = levelButtonsParent.gameObject.GetComponentsInChildren<LevelSelectButton>();
                foreach (LevelSelectButton button in pageButtons)
                {
                    button.Show();
                }
            }
        }
    }

    public void HidePage(int pageNumber)
    {
        if (pageNumber < paginationRect.Pages.Count && pageNumber >= 0)
        {   
            Page page = paginationRect.Pages[pageNumber];
            if (page)
            {
                Transform levelButtonsParent = page.transform.Find("LevelButtons");
                if (levelButtonsParent)
                {
                    CanvasGroup pageCanvasGroup = levelButtonsParent.GetComponent<CanvasGroup>();
                    if (pageCanvasGroup)
                    {
                        pageCanvasGroup.alpha = 0;
                        pageCanvasGroup.interactable = false;
                        pageCanvasGroup.blocksRaycasts = false;
                    }
                }
            }
        }
    }

    public void ShowAllPages()
    {
        for (int i = 0; i < paginationRect.Pages.Count; i++)
        {
            ShowPage(i);
        }
    }
    
    public void HideAllPages()
    {
        for (int i = 0; i < paginationRect.Pages.Count; i++)
        {
            HidePage(i);
        }
    }
}
