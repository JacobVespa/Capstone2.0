using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject backButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private AudioMixer audioMixer;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
    }

    public void SetMasterVolume()
    {
        volumeSlider.value = 80;
        float volume = volumeSlider.value;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        sfxSlider.value = 80;
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", volume);
    }


}
