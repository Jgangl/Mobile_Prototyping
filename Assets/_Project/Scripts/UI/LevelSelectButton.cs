using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    int level;

    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Transform completeCheckmark;
    [SerializeField] Transform lockTransform;
    [SerializeField] Color completeColor;
    [SerializeField] Image mainImage;
    [SerializeField] Image bgImage;
    [SerializeField] DOTweenAnimation clickAnimation;
    [SerializeField] DOTweenAnimation idleAnimation;

    Vector3 startScale;

    //Tween idleTween;

    bool bLocked = true;

    void Start()
    {
        EnableLevelLock(bLocked);
    }

    void OnEnable()
    {
        button.interactable = true;
        
        button.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        Level levelObject = Level_Manager.Instance?.GetLevel(level);
        Level prevLevelObject = Level_Manager.Instance?.GetLevel(level - 1);

        idleAnimation.CreateTween(true, true);

        Level currentLevel = Level_Manager.Instance?.GetCurrentLevel();
        
        // Play idle animation on 'current level'
        if (currentLevel != null && currentLevel != levelObject)
        {
            idleAnimation.DOKill(); 
        }
    }

    public void Setup(int level, Action<int> onClickAction)
    {
        this.level = level;
        button.onClick.AddListener(() => onClickAction(level));
        button.onClick.AddListener(OnClick);

        buttonText.text = level.ToString();
    }
    
    public void EnableLevelCompleteCheck(bool enabled) 
    {
        completeCheckmark.gameObject.SetActive(enabled);
    }

    void SetColor(Color newColor)
    {
        if (mainImage)
            mainImage.color = newColor;
    }

    public void Complete()
    {
        SetColor(completeColor);
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

    void OnClick()
    {
        idleAnimation.DORestart();
        idleAnimation.DOKill();
        clickAnimation.CreateTween(true, true);

        AudioManager.Instance.PlayLaunchSound();
        
        
    }

    public void Show()
    {
        button.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        Level levelObject = Level_Manager.Instance?.GetLevel(level);
        Level prevLevelObject = Level_Manager.Instance?.GetLevel(level - 1);

        idleAnimation.CreateTween(true, true);

        Level currentLevel = Level_Manager.Instance?.GetCurrentLevel();
        
        // Play idle animation on 'current level'
        if (currentLevel != null && currentLevel != levelObject)
        {
            idleAnimation.DOKill(); 
        }
    }

    public void Hide()
    {
        
    }

    public void SetInteractable(bool interactable)
    {
        if (!bLocked)
            button.interactable = interactable;
    }
}
