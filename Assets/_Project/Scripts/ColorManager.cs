using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [SerializeField] Color green;
    [SerializeField] Color red;
    [SerializeField] Color yellow;
    [SerializeField] Color blue;

    [Header("Player Particles")]
    [SerializeField] ParticleSystem playerTrail;
    [SerializeField] ParticleSystem playerSlimeParticleSystem;
    
    [Header("Player Images")]
    [SerializeField] SpriteRenderer mainImage;
    [SerializeField] SpriteRenderer outlineImage;
    [SerializeField] SpriteRenderer leftEyePupil;
    [SerializeField] SpriteRenderer leftEyeBack;
    [SerializeField] SpriteRenderer rightEyePupil;
    [SerializeField] SpriteRenderer rightEyeBack;
    [SerializeField] SpriteRenderer mouth;
    
    float selectedHue;

    [SerializeField] Color defaultColor;
    
    void Start()
    {
        string colorHtmlString = PlayerPrefs.GetString("PlayerColor", "#" + ColorUtility.ToHtmlStringRGB(defaultColor));
        ColorUtility.TryParseHtmlString(colorHtmlString, out Color savedColor);
        Color.RGBToHSV(savedColor, out float hue, out float sat, out float val);

        SetColor(hue);
    }

    public void SetColor(float hue)
    {
        selectedHue = hue;

        SetSpriteHue(mainImage, selectedHue);

        SetSpriteHue(outlineImage, selectedHue);

        SetSpriteHue(leftEyePupil, selectedHue);

        SetSpriteHue(leftEyeBack, selectedHue);

        SetSpriteHue(rightEyePupil, selectedHue);

        SetSpriteHue(rightEyeBack, selectedHue);

        SetSpriteHue(mouth, selectedHue);

        SetParticleSystemStartColorHue(playerTrail, selectedHue);
        
        SetParticleSystemStartColorHue(playerSlimeParticleSystem, selectedHue);
    }

    void SetSpriteHue(SpriteRenderer sprite, float desiredHue)
    {
        Color.RGBToHSV(sprite.color, out float hue, out float saturation, out float value);
        Color newColor = Color.HSVToRGB(desiredHue, saturation, value);
        sprite.color = new Color(newColor.r, newColor.g, newColor.b, sprite.color.a);
    }
    
    public void SetParticleSystemStartColorHue(ParticleSystem particleSys, float desiredHue)
    {
        var particleSystemMain = particleSys.main;
        Color originalColor = particleSystemMain.startColor.color;

        Color.RGBToHSV(originalColor, out float hue, out float saturation, out float value);
        Color newColor = Color.HSVToRGB(desiredHue, saturation, value);
        particleSystemMain.startColor = new Color(newColor.r, newColor.g, newColor.b, originalColor.a);
    }

    public float GetSelectedHue()
    {
        return selectedHue;
    }

    public Color GetDefaultColor()
    {
        return defaultColor;
    }
}
