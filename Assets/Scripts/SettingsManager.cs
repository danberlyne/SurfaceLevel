using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] float saveDelay = 0.5f;
    [SerializeField] Slider[] settingsSliders;
    [SerializeField] float defaultSliderValue = 50f;
    [SerializeField] GameObject[] bindings;
    [SerializeField] string[] defaultPaths;
    [Header("Audio Settings")]
    [SerializeField] GameObject backgroundVolume;
    float backgroundVolumeValue;
    [SerializeField] GameObject sfxVolume;
    float sfxVolumeValue;
    [Header("Controls Settings")]
    [SerializeField] GameObject turnSensitivity;
    float turnSensitivityValue;
    float minTurnSensitivity = 50f;
    [Header("Key Bindings")]
    [SerializeField] InputActionAsset inputActionAsset;
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
        UpdateBindings();
        UpdateControls();
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
        PlayerPrefs.SetString("Fire 1", GetPath("Fire", 0));
        PlayerPrefs.SetString("Fire 2", GetPath("Fire", 1));
        PlayerPrefs.SetString("Kill 1", GetPath("Kill", 0));
        PlayerPrefs.SetString("Kill 2", GetPath("Kill", 1));
        PlayerPrefs.SetString("Pause 1", GetPath("Pause", 0));
        PlayerPrefs.SetString("Pause 2", GetPath("Pause", 1));
        PlayerPrefs.Save();
    }

    string GetPath(string actionName, int bindingNumber)
    {
        return inputActionAsset[actionName].bindings[bindingNumber].effectivePath;
    }

    void LoadSettings()
    {
        backgroundVolumeValue = PlayerPrefs.GetFloat("Background Volume", defaultSliderValue) / 100f;
        settingsSliders[0].value = PlayerPrefs.GetFloat("Background Volume", defaultSliderValue);
        sfxVolumeValue = PlayerPrefs.GetFloat("SFX Volume", defaultSliderValue) / 100f;
        settingsSliders[1].value = PlayerPrefs.GetFloat("SFX Volume", defaultSliderValue);
        turnSensitivityValue = minTurnSensitivity + PlayerPrefs.GetFloat("Turn Sensitivity", defaultSliderValue) * 2;
        settingsSliders[2].value = PlayerPrefs.GetFloat("Turn Sensitivity", defaultSliderValue);
        fire1.GetComponent<TextMeshProUGUI>().text = BindingPathToString(PlayerPrefs.GetString("Fire 1", "<Keyboard>/Space"));
        fire2.GetComponent<TextMeshProUGUI>().text = BindingPathToString(PlayerPrefs.GetString("Fire 2"));
        kill1.GetComponent<TextMeshProUGUI>().text = BindingPathToString(PlayerPrefs.GetString("Kill 1", "<Keyboard>/K"));
        kill2.GetComponent<TextMeshProUGUI>().text = BindingPathToString(PlayerPrefs.GetString("Kill 2"));
        pause1.GetComponent<TextMeshProUGUI>().text = BindingPathToString(PlayerPrefs.GetString("Pause 1", "<Keyboard>/Escape"));
        pause2.GetComponent<TextMeshProUGUI>().text = BindingPathToString(PlayerPrefs.GetString("Pause 2", "<Keyboard>/P"));
    }

    string BindingPathToString(string bindingPath)
    {
        if (bindingPath == null || bindingPath == "")
        {
            bindingPath = "None";
        }
        else
        {
            bindingPath = InputControlPath.ToHumanReadableString(bindingPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        return bindingPath;
    }

    public void ResetSettings()
    {
        foreach (Slider slider in settingsSliders)
        {
            slider.value = defaultSliderValue;
        }

        for (int i = 0; i < bindings.Length; i++)
        {
            bindings[i].GetComponent<TextMeshProUGUI>().text = BindingPathToString(defaultPaths[i]);
        }

        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Fire"], 0, defaultPaths[0]);
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Fire"], 1, defaultPaths[1]);
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Kill"], 0, defaultPaths[2]);
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Kill"], 1, defaultPaths[3]);
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Pause"], 0, defaultPaths[4]);
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Pause"], 1, defaultPaths[5]);
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
        turnSensitivityValue = minTurnSensitivity + value * 2;
    }

    public float GetTurnSensitivity()
    {
        return turnSensitivityValue;
    }

    public void UpdateControls()
    {
        if (FindObjectOfType<GunController>())
        {
            FindObjectOfType<GunController>().SetTurnSensitivity(turnSensitivityValue);
        }
    }

    public void UpdateBindings()
    {
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Fire"], 0, PlayerPrefs.GetString("Fire 1"));
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Fire"], 1, PlayerPrefs.GetString("Fire 2"));
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Kill"], 0, PlayerPrefs.GetString("Kill 1"));
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Kill"], 1, PlayerPrefs.GetString("Kill 2"));
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Pause"], 0, PlayerPrefs.GetString("Pause 1"));
        InputActionRebindingExtensions.ApplyBindingOverride(inputActionAsset["Pause"], 1, PlayerPrefs.GetString("Pause 2"));
    }
}