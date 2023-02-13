using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class LevelSelectButton : MonoBehaviour
{
    private int level;

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Transform completeCheckmark;
    [SerializeField] private Transform lockTransform;
    [SerializeField] private Image bgImage;

    bool bLocked = true;

    void Start()
    {
        EnableLevelLock(bLocked);
    }

    public void Setup(int level, Action<int> onClickAction)
    {
        this.level = level;
        button.onClick.AddListener(() => onClickAction(level));
        
        buttonText.text = level.ToString();
    }
    
    public void EnableLevelCompleteCheck(bool enabled) 
    {
        completeCheckmark.gameObject.SetActive(enabled);
    }

    public void Complete()
    {
        EnableLevelCompleteCheck(true);
    }

    public void Unlock()
    {
        bLocked = false;
        EnableLevelLock(false);
    }
    
    void EnableLevelLock(bool enabled) 
    {
        lockTransform.gameObject.SetActive(enabled);

        // Disable button while locked
        button.interactable = !enabled;
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetLocked(bool isLocked)
    {
        bLocked = isLocked;
    }
}
