using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StoreMenu : Menu
{
    [SerializeField] ColorManager playerPreviewColorManager;
    [SerializeField] ColorPicker colorPicker;
    [SerializeField] SliderBase hueSlider;
    [SerializeField] ParticleSystem previewSlimeParticleSystem;

    ColorButton prevSelectedColorButton;
    PlayerColor selectedPlayerColor;

    Color currSliderColor;

    void Start()
    {
        colorPicker.Show(true);
        //colorPicker.Show(false);
        
        Debug.Log("Default Color: " + ColorUtility.ToHtmlStringRGB(playerPreviewColorManager.GetDefaultColor()));
        string colorHtmlString = PlayerPrefs.GetString("PlayerColor", "#" + ColorUtility.ToHtmlStringRGB(playerPreviewColorManager.GetDefaultColor()));
        Debug.Log("Read color: " + colorHtmlString);
        ColorUtility.TryParseHtmlString(colorHtmlString, out Color savedColor);
        Color.RGBToHSV(savedColor, out float hue, out float sat, out float val);

        hueSlider.ForceSetValue(hue * hueSlider.maxValue, false);

        colorPicker.OnColorValueChanged += ColorChanged;
        hueSlider.ActionToInvokeOnPointerRelease += OnSliderReleased;
    }
/*
    public void ColorButtonClicked(ColorButton clickedColorButton)
    {
        if (prevSelectedColorButton != clickedColorButton)
        {
            if (prevSelectedColorButton != null)
            {
                prevSelectedColorButton.StopIdleAnimation();
            }
            
            selectedPlayerColor = clickedColorButton.GetColor();
            clickedColorButton.PlayIdleAnimation();
            
            //SelectColor(selectedPlayerColor);
            
            prevSelectedColorButton = clickedColorButton;
        }
    }
*/
    public void SelectColor(float hue)
    {
        playerPreviewColorManager.SetColor(hue);
        
        playerPreviewColorManager.SetParticleSystemStartColorHue(previewSlimeParticleSystem, hue);
        
        //PlayerPrefs.SetString("PlayerColor", newColor.ToString());
    }

    void OnSliderReleased(float val)
    {
        string colorHtmlString = "#" + ColorUtility.ToHtmlStringRGB(currSliderColor);

        Debug.Log("Saving color: " + colorHtmlString);
        // Save color when slider is released
        PlayerPrefs.SetString("PlayerColor", colorHtmlString);
    }

    public void ColorChanged(Color newColor)
    {
        currSliderColor = newColor;
        
        Color.RGBToHSV(newColor, out float hue, out float sat, out float val);
        
        Debug.Log("Color Changed: " + newColor);
        SelectColor(hue);
    }
}

public enum PlayerColor
{
    Green,
    Red,
    Yellow,
    Blue
}
