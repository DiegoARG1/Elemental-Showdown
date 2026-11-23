using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Controles de Audio")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteMusicToggle;

    private void OnEnable()
    {
        if (AudioManager.Instance == null) return;

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        if (muteMusicToggle != null)
        {
            muteMusicToggle.SetIsOnWithoutNotify(AudioManager.Instance.IsMusicMuted);
            muteMusicToggle.onValueChanged.AddListener(OnMuteToggleChanged);
        }
    }

    private void OnDisable()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        if (muteMusicToggle != null) muteMusicToggle.onValueChanged.RemoveListener(OnMuteToggleChanged);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnMuteToggleChanged(bool isMuted)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicMuted(isMuted);
    }
}