using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Button))]
public class ColorButton : MonoBehaviour
{
    [SerializeField] PlayerColor color;
    [SerializeField] StoreMenu store;
    [SerializeField] DOTweenAnimation clickAnimation;
    [SerializeField] DOTweenAnimation selectedAnimation;
    
    Tween selectedTween;
    Tween scaleTween;
    
    Vector3 neutralScale;
/*
    void Start()
    {
        selectedTween = selectedAnimation.tween;

        neutralScale = transform.localScale;
        
        GetComponent<Button>().onClick.AddListener(() => store.ColorButtonClicked(this));
        GetComponent<Button>().onClick.AddListener(PlayClickAnimation);
        
        string selectedColorString = PlayerPrefs.GetString("PlayerColor", PlayerColor.Green.ToString());
        
        PlayerColor selectedColor = (PlayerColor) Enum.Parse(typeof(PlayerColor), selectedColorString);
        if (selectedColor == color)
        {
            store.ColorButtonClicked(this);
        }
    }

    void PlayClickAnimation()
    {
        Vector3 scaleBefore = transform.localScale;
        selectedTween.Pause();
        clickAnimation.RewindThenRecreateTweenAndPlay();
        clickAnimation.onComplete.RemoveAllListeners();
        clickAnimation.onComplete.AddListener(() => scaleTween = transform.DOScale(scaleBefore, 0.25f).OnComplete(() => selectedTween.Play()));

        //clickAnimation.onComplete.AddListener(() => selectedTween.Play());
    }

    public void PlayIdleAnimation()
    {
        selectedTween.Restart();
    }

    public void StopIdleAnimation()
    {
        clickAnimation.onComplete.RemoveAllListeners();
        if (scaleTween != null)
            scaleTween.onComplete = () => { };
        
        selectedTween.Pause();

        transform.localScale = neutralScale;
    }

    public PlayerColor GetColor()
    {
        return color;
    }
*/
}
