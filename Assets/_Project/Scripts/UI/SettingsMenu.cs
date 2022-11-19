using Michsky.UI.ModernUIPack;
using UnityEngine;

public class SettingsMenu : Menu
{
    [SerializeField] private Transform homeButton;

    [SerializeField] private SwitchManager musicSwitch;
    [SerializeField] private SwitchManager sfxSwitch;

    private bool isMusicSwitchON;
    private bool isSFXSwitchON;

    private void Start()
    {
        musicSwitch.Start();
        sfxSwitch.Start();
        
        isMusicSwitchON = musicSwitch.isOn;
        isSFXSwitchON = sfxSwitch.isOn;
        
        AudioManager.Instance.UpdateMusicEnabled(isMusicSwitchON);
        AudioManager.Instance.UpdateSoundFXEnabled(isSFXSwitchON);
    }

    public void EnableHomeButton(bool enabled)
    {
        homeButton.gameObject.SetActive(enabled);
    }
}
