using Michsky.UI.ModernUIPack;
using UnityEngine;

public class SettingsMenu : Menu
{
    [SerializeField] private Transform homeButton;

    [SerializeField] private SwitchManager musicSwitch;
    [SerializeField] private SwitchManager sfxSwitch;
    [SerializeField] private SwitchManager cameraShakeSwitch;

    private bool isMusicSwitchON;
    private bool isSFXSwitchON;
    private bool isCameraShakeSwitchON;

    private void Start()
    {
        musicSwitch.Start();
        sfxSwitch.Start();
        cameraShakeSwitch.Start();
        
        isMusicSwitchON       = musicSwitch.isOn;
        isSFXSwitchON         = sfxSwitch.isOn;
        isCameraShakeSwitchON = cameraShakeSwitch.isOn;
        
        AudioManager.Instance.UpdateMusicEnabled(isMusicSwitchON);
        AudioManager.Instance.UpdateSoundFXEnabled(isSFXSwitchON);
        CinemachineShake.Instance.UpdateCameraShakeEnabled(isCameraShakeSwitchON);
    }

    public void EnableHomeButton(bool enabled)
    {
        homeButton.gameObject.SetActive(enabled);
    }
}
