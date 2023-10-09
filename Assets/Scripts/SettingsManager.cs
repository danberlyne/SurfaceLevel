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
    [SerializeField] GameObject sfxVolume;
    [Header("Controls Settings")]
    [SerializeField] GameObject turnSensitivity;
    [Header("Key Bindings")]
    [SerializeField] GameObject fire1;
    [SerializeField] GameObject fire2;
    [SerializeField] GameObject kill1;
    [SerializeField] GameObject kill2;
    [SerializeField] GameObject pause1;
    [SerializeField] GameObject pause2;


    void Awake()
    {
        int numSettingsManagers = FindObjectsOfType<SettingsManager>().Length;
        if (numSettingsManagers > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
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
        yield return new WaitForSeconds(saveDelay);
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

    public void ResetSettings()
    {
        foreach (Slider slider in settingsSliders)
        {
            slider.value = defaultSliderValue;
        }
    }

}