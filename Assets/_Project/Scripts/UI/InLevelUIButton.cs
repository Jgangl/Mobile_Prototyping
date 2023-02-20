using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InLevelUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static bool isMouseOverButton;

    void Start()
    {
        Level_Manager.Instance.OnLevelReset += OnLevelReset;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverButton = false;
    }

    public static bool IsMouseOverButton()
    {
        return isMouseOverButton;
    }

    void OnLevelReset()
    {
        isMouseOverButton = false;
    }
}
