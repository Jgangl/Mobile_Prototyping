using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class LevelSelectButton : MonoBehaviour
{
    int level;

    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Transform completeCheckmark;
    [SerializeField] Transform lockTransform;
    [SerializeField] Image bgImage;
    [SerializeField] DOTweenAnimation clickAnimation;

    bool bLocked = true;

    void Start()
    {
        EnableLevelLock(bLocked);
    }

    public void Setup(int level, Action<int> onClickAction)
    {
        this.level = level;
        button.onClick.AddListener(() => onClickAction(level));
        button.onClick.AddListener(() => clickAnimation.DORestart());
        button.onClick.AddListener(() => AudioManager.Instance.PlayLaunchSound());
        
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
