using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] float saveDelay = 0.5f;
    [SerializeField] Slider[] settingsSliders;
    [SerializeField] float defaultSliderValue = 50f;
    [Header("Audio Settings")]
    [SerializeField] GameObject backgroundVolume;
    float backgroundVolumeValue;
    [SerializeField] GameObject sfxVolume;
    float sfxVolumeValue;
    [Header("Controls Settings")]
    [SerializeField] GameObject turnSensitivity;
    float turnSensitivityValue;
    [Header("Key Bindings")]
    [SerializeField] GameObject fire1;
    [SerializeField] GameObject fire2;
    [SerializeField] GameObject kill1;
    [SerializeField] GameObject kill2;
    [SerializeField] GameObject pause1;
    [SerializeField] GameObject pause2;


    void Awake()
    {
        LoadSettings();
        UpdateBackgroundVolume();
        UpdateSFXVolume();
    }

    void Start()
    {
        
    }

    public void OpenSettingsMenu()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        StartCoroutine(CloseMenuSequence());
    }

    IEnumerator CloseMenuSequence()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        SaveSettings();
        yield return new WaitForSecondsRealtime(saveDelay);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat("Background Volume", backgroundVolume.GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("SFX Volume", sfxVolume.GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("Turn Sensitivity", turnSensitivity.GetComponent<Slider>().value);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        backgroundVolumeValue = PlayerPrefs.GetFloat("Background Volume", defaultSliderValue) / 100f;
        settingsSliders[0].value = PlayerPrefs.GetFloat("Background Volume", defaultSliderValue);
        sfxVolumeValue = PlayerPrefs.GetFloat("SFX Volume", defaultSliderValue) / 100f;
        settingsSliders[1].value = PlayerPrefs.GetFloat("SFX Volume", defaultSliderValue);
        turnSensitivityValue = PlayerPrefs.GetFloat("Turn Sensitivity", defaultSliderValue) * -4;
        settingsSliders[2].value = PlayerPrefs.GetFloat("Turn Sensitivity", defaultSliderValue);
    }

    public void ResetSettings()
    {
        foreach (Slider slider in settingsSliders)
        {
            slider.value = defaultSliderValue;
        }
    }

    public void UpdateBackgroundVolume()
    {
        AudioSource[] sources = FindObjectOfType<Camera>().gameObject.GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            source.volume = backgroundVolumeValue;
        }
    }

    public void SetBackgroundVolume(float value)
    {
        backgroundVolumeValue = value / 100f;
    }

    public float GetBackgroundVolume()
    {
        return backgroundVolumeValue;
    }

    public void UpdateSFXVolume()
    {
        FindObjectOfType<MenuManager>().gameObject.GetComponent<AudioSource>().volume = sfxVolumeValue;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolumeValue = value / 100f;
    }

    public float GetSFXVolume()
    {
        return sfxVolumeValue;
    }

    public void SetTurnSensitivity(float value)
    {
        turnSensitivityValue = value * -4;
    }

    public float GetTurnSensitivity()
    {
        return turnSensitivityValue;
    }
}