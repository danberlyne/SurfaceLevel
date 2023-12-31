using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GunController gunController;
    int remainingFragments;

    [Header("UI")]
    [SerializeField] GameObject overlayCanvas;
    // [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] GameObject effectsCanvas;

    [Header("Startup")]
    [SerializeField] float startDelay = 4.0f;
    [SerializeField] float loadDelay = 0.5f;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverGraphic;
    [SerializeField] GameObject gameOverButtons;
    [SerializeField] float gameOverFadeDuration = 3.0f;
    [SerializeField] float redFadeAmount = 0.5f;

    [Header("Effects")]
    [SerializeField] EffectButtons effectButtons;

    [Header("Debugging")]
    [SerializeField] bool slowTime = false;
    [SerializeField] float slowMultiplier = 0.1f;

    void Start()
    {
        remainingFragments = GameObject.FindGameObjectsWithTag("Fragment").Length;
        effectButtons.DisableInactiveEffectButtons();
        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        float brightness = 0f;
        Light2D[] lightSources = FindObjectsOfType<Light2D>();

        while (brightness < 1.0f)
        {
            foreach (Light2D lightSource in lightSources)
            {
                lightSource.intensity = brightness;
            }
            brightness += Time.deltaTime / startDelay;
            yield return null;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level1-1")
        {
            if (overlayCanvas)
            {
                overlayCanvas.SetActive(true);
            }
            effectsCanvas.SetActive(true);
        }
    }

    void Update()
    {
        if (slowTime)
        {
            Time.timeScale *= slowMultiplier;
            slowTime = false;
        }

        if (remainingFragments == 0)
        {
            StartCoroutine(LoadNextLevel());
        }
    }

    public void CheckForLevelEnd()
    {
        int remainingEnergy = FindObjectOfType<SliderController>().GetRemainingEnergy();
        int energyCost = FindObjectOfType<GunController>().GetEnergyCost();
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        if (remainingEnergy < energyCost && projectiles.Length <= 1)
        {
            FindObjectOfType<GunController>().DisableControls();

            Image childImage = gameOverGraphic.GetComponentInChildren<Image>();
            Color childColour = childImage.color;
            childColour.a = 0f;
            childImage.color = childColour;
            TMP_Text childText = gameOverGraphic.GetComponentInChildren<TMP_Text>();
            childText.overrideColorTags = true;
            Color childTextColour = childText.color;
            childTextColour.a = 0f;
            childText.color = childTextColour;

            gameOverGraphic.SetActive(true);

            StartCoroutine(FadeIn(gameOverFadeDuration));

            StartCoroutine(InitiateGameOver());
        }
    }

    IEnumerator FadeIn(float duration)
    {
        float transparency = 0f;
        Image childImage = gameOverGraphic.GetComponentInChildren<Image>();
        Color childColour = childImage.color;
        TMP_Text childText = gameOverGraphic.GetComponentInChildren<TMP_Text>();
        childText.overrideColorTags = true;
        Color childTextColour = childText.color;

        while (transparency < 1.0f)
        {
            childColour.a = transparency * redFadeAmount;
            childImage.color = childColour;
            childTextColour.a = transparency;
            childText.color = childTextColour;
            transparency += Time.deltaTime / duration;
            yield return null;
        }
    }

    IEnumerator InitiateGameOver()
    {
        yield return new WaitForSecondsRealtime(gameOverFadeDuration);
        gameOverButtons.SetActive(true);
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(loadDelay);
        // Go to Success screen, calculate bonus, then prompt to go to next level or save and quit. If we reached the last level, go to Congratulations screen and show high scores.
        GameSession gameSession = FindObjectOfType<GameSession>();
        gameSession.UpdateScoreAtLevelStart();
        FindObjectOfType<SliderController>().ResetEnergy();
        FindObjectOfType<ScoreKeeper>().UpdateLevel();
        SaveGameData(gameSession);

        effectButtons.EnableEffectButtons();
        effectButtons.ToggleAllOff();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(nextSceneIndex);
    }

    void SaveGameData(GameSession session)
    {
        SaveManager saveManager = FindObjectOfType<SaveManager>();
        JSONReader reader = FindObjectOfType<JSONReader>();
        saveManager.SetCurrentScore(session.GetScoreAtLevelStart());
        reader.SetCurrentScore(session.GetScoreAtLevelStart());
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string pathToNextScene = SceneUtility.GetScenePathByBuildIndex(currentSceneIndex + 1);
        string nextSceneName = Path.GetFileNameWithoutExtension(pathToNextScene);
        if (nextSceneName[..5] == "Level")
        {
            int l = nextSceneName.Length;
            int stage = Int32.Parse(nextSceneName[l - 3].ToString());
            int level = Int32.Parse(nextSceneName[l - 1].ToString());
            saveManager.SetCurrentLevel((stage, level));
            reader.SetCurrentLevel((stage, level));
            saveManager.UpdateLevelProgression((stage, level));
        }
        saveManager.SaveToJson();
    }

    public void RestartLevel()
    {
        FindObjectOfType<SliderController>().ResetEnergy();
        FindObjectOfType<ScoreKeeper>().RollbackScore();
        effectButtons.EnableEffectButtons();
        effectButtons.ToggleAllOff();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void UpdateRemainingFragments()
    {
        remainingFragments--;
    }

    public int GetRemainingFragments()
    {
        return remainingFragments;
    }
}
