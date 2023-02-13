using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AfterLevelMenu : Menu
{
    [SerializeField] TextMeshProUGUI currentJumpsText;
    [SerializeField] TextMeshProUGUI bestJumpsText;
    [SerializeField] TextMeshProUGUI timeText;

    void Start()
    {
        Level_Manager.Instance.OnLevelCompleted += OnLevelCompleted;
    }

    void OnLevelCompleted(int levelCompleted)
    {
        UpdateLevelValues();
    }

    public void UpdateLevelValues()
    {
        Level currentLevel = Level_Manager.Instance.GetCurrentLevel();
        
        double timeThisLevel = GameManager.Instance.GetTimeThisLevel();
        int jumpsThisLevel = GameManager.Instance.GetNumJumpsThisLevel();

        SetCurrentJumpsText(jumpsThisLevel);
        SetBestJumpsText(currentLevel.bestNumOfJumps);
        SetTimeText(timeThisLevel);
    }

    public void SetCurrentJumpsText(int newCurrentJumps)
    {
        currentJumpsText.text = newCurrentJumps.ToString();
    }
    
    public void SetBestJumpsText(int newBestJumps)
    {
        bestJumpsText.text = newBestJumps.ToString();
    }
    
    public void SetTimeText(double newTime)
    {
        // TODO: Format time properly

        string time = string.Format("{0}", newTime.ToString("F1"));
        timeText.text = time;
    }
}
