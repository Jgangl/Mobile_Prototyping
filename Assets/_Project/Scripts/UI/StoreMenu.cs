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

    [SerializeField] Image previewSlimeImage;

    ColorButton prevSelectedColorButton;
    PlayerColor selectedPlayerColor;

    Color currSliderColor;

    void Start()
    {
        colorPicker.Show(true);
        
        string colorHtmlString = PlayerPrefs.GetString("PlayerColor", "#" + ColorUtility.ToHtmlStringRGB(playerPreviewColorManager.GetDefaultColor()));
        ColorUtility.TryParseHtmlString(colorHtmlString, out Color savedColor);
        Color.RGBToHSV(savedColor, out float hue, out float sat, out float val);

        hueSlider.ForceSetValue(hue * hueSlider.maxValue, false);
        
        // Set color with saved value on startup
        SelectColor(hue);

        colorPicker.OnColorValueChanged += ColorChanged;
        hueSlider.ActionToInvokeOnPointerRelease += OnSliderReleased;
    }

    public void SelectColor(float hue)
    {
        playerPreviewColorManager.SetColor(hue);
        playerPreviewColorManager.SetParticleSystemStartColorHue(previewSlimeParticleSystem, hue);
        SetImageHue(previewSlimeImage, hue);
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

        SelectColor(hue);
    }
    
    void SetImageHue(Image image, float desiredHue)
    {
        Color.RGBToHSV(image.color, out float hue, out float saturation, out float value);
        Color newColor = Color.HSVToRGB(desiredHue, saturation, value);
        image.color = new Color(newColor.r, newColor.g, newColor.b, image.color.a);
    }
}

public enum PlayerColor
{
    Green,
    Red,
    Yellow,
    Blue
}
