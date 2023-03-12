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
    [SerializeField] Color completeColor;
    [SerializeField] Image bgImage;
    [SerializeField] DOTweenAnimation clickAnimation;
    [SerializeField] DOTweenAnimation idleAnimation;

    bool bLocked = true;

    void Start()
    {
        EnableLevelLock(bLocked);
    }

    void OnEnable()
    {
        Level levelObject = Level_Manager.Instance.GetLevel(level);
        Level prevLevelObject = Level_Manager.Instance.GetLevel(level - 1);
        Debug.Log("On Enable");
        //idleAnimation.DOKill();
        
        idleAnimation.CreateTween(true, false);
        
        if (prevLevelObject.levelNumber != -1 && prevLevelObject.completed && !levelObject.completed)
        {
            
            //idleAnimation.DOPlay();
            idleAnimation.DORestart();
            //idleAnimation.DOPlayForward();
            //idleAnimation.DOPlayNext();
        }
        else
        {
            idleAnimation.DOComplete();
        }
    }

    public void Setup(int level, Action<int> onClickAction)
    {
        this.level = level;
        button.onClick.AddListener(() => onClickAction(level));
        button.onClick.AddListener(() => clickAnimation.DORestart());
        button.onClick.AddListener(() => AudioManager.Instance.PlayLaunchSound());
        
        buttonText.text = level.ToString();
        
        //idleAnimation.DORestart();
        //idleAnimation.DOPlay();
        //Debug.Log(idleAnimation.isActive);
    }
    
    public void EnableLevelCompleteCheck(bool enabled) 
    {
        completeCheckmark.gameObject.SetActive(enabled);
    }

    void SetColor(Color newColor)
    {
        Image image = button.GetComponent<Image>();
        if (image)
            image.color = newColor;
    }

    public void Complete()
    {
        SetColor(completeColor);
        //idleAnimation.DOKill();
        //EnableLevelCompleteCheck(true);
    }

    public void Unlock()
    {
        bLocked = false;
        EnableLevelLock(false);
        //PlayIdleAnimation();
    }

    public void PlayIdleAnimation()
    {
        Debug.Log("Play idle anim for level: " + level);
        //idleAnimation.DORestart();
        //idleAnimation.DOPlay();
        //idleAnimation.DOTogglePause();
    }
    
    public void StopIdleAnimation()
    {
        Debug.Log("Stop idle anim for level: " + level);
        //idleAnimation.DOKill();
        //idleAnimation.DOComplete();
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
