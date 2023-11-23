using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;
    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip disabledSound;
    [SerializeField] GameObject[] menuButtons;
    [SerializeField] bool isSuccessScreen = false;
    [SerializeField] GameObject successText;
    [SerializeField] float successFadeDuration = 3f;
    Transform successTransform;
    SaveManager saveManager;
    JSONReader reader;
    GameSession gameSession;
    ScoreKeeper scoreKeeper;
    [SerializeField] GameObject continueButton;


    void Awake()
    {
        saveManager = FindObjectOfType<SaveManager>();
        reader = FindObjectOfType<JSONReader>();
        gameSession = FindObjectOfType<GameSession>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
    }

    void Start()
    {
        if (isSuccessScreen)
        {
            gameSession.transform.GetChild(0).gameObject.SetActive(false);
            successTransform = successText.transform;
            InitialiseText();
            StartCoroutine(FadeIn(successFadeDuration));
            StartCoroutine(EnableButtons());
        }
        else if (SceneManager.GetActiveScene().buildIndex == 0 && reader.GetCurrentLevel().Item1 == 1 && reader.GetCurrentLevel().Item2 == 1)
        {
            continueButton.SetActive(false);
        }
    }

    void InitialiseText()
    {
        for (int i = 0; i < successTransform.childCount; i++)
        {
            TMP_Text childText = successTransform.GetChild(i).GetComponent<TMP_Text>();
            childText.overrideColorTags = true;
            Color childTextColour = childText.color;
            childTextColour.a = 0f;
            childText.color = childTextColour;
            childText.gameObject.SetActive(true);
        }
    }

    IEnumerator FadeIn(float duration)
    {
        float transparency;

        for (int i = 0; i < successTransform.childCount; i++)
        {
            transparency = 0f;
            TMP_Text text = successTransform.GetChild(i).GetComponent<TMP_Text>();
            text.overrideColorTags = true;
            Color textColour = text.color;

            while (transparency < 1.0f)
            {
                textColour.a = transparency;
                text.color = textColour;
                transparency += Time.deltaTime / duration;
                yield return null;
            }
        }
    }

    IEnumerator EnableButtons()
    {
        yield return new WaitForSecondsRealtime(successFadeDuration * (successText.transform.childCount + 0.5f));
        
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].SetActive(true);
        }
    }

    public void StartNewGame()
    {
        saveManager.SetCurrentScore(0);
        saveManager.SetCurrentLevel((1, 1));
        saveManager.SaveToJson();
        StartCoroutine(LoadFirstLevel());
    }

    IEnumerator LoadFirstLevel()
    {
        yield return new WaitForSeconds(loadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void ContinueGame()
    {
        saveManager.LoadCurrentGame();
    }

    public void QuitGame()
    {
        if (isSuccessScreen)
        {
            saveManager.UpdateOverallHighScores(scoreKeeper.GetScore());
            saveManager.UpdateLevelProgression((100, 100));
            saveManager.SetCurrentScore(0);
            saveManager.SetCurrentLevel((1, 1));
            saveManager.SaveToJson();
        }
        else
        {
            SaveCurrentGame();
            SaveProgress();
        }
        StartCoroutine(LoadMainMenu());
    }

    void SaveCurrentGame()
    {
        (int, int) currentLevel = gameSession.GetCurrentLevel();
        saveManager.SetCurrentScore(gameSession.GetScoreAtLevelStart());
        saveManager.SetCurrentLevel(currentLevel);
        saveManager.SaveToJson();
    }

    void SaveProgress()
    {
        (int, int) currentLevel = gameSession.GetCurrentLevel();
        saveManager.UpdateLevelProgression(currentLevel);
        saveManager.SaveToJson();
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(loadDelay);
        GameSession gameSession = FindObjectOfType<GameSession>();
        gameSession.ResetInstance();
        Destroy(gameSession.gameObject);
        SceneManager.LoadScene(0);
    }

    public void PlayButtonSound(int index)
    {
        Button button = menuButtons[index].GetComponent<Button>();
        AudioSource audioSource = GetComponent<AudioSource>();
        
        if (button.interactable)
        {
            audioSource.PlayOneShot(selectSound);
        }
        else
        {
            audioSource.PlayOneShot(disabledSound);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
