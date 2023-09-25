using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    ProjectileBehaviour projectileBehaviour;
    int remainingFragments;
    [SerializeField] float loadDelay = 0.5f;
    [SerializeField] GameObject gameOverGraphic;
    [SerializeField] GameObject gameOverButtons;
    [SerializeField] float gameOverFadeDuration = 3.0f;
    [SerializeField] float redFadeAmount = 0.5f;

    void Start()
    {
        projectileBehaviour = FindObjectOfType<ProjectileBehaviour>();
        remainingFragments = GameObject.FindGameObjectsWithTag("Fragment").Length;
    }

    void Update()
    {
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
        FindObjectOfType<GameSession>().UpdateScoreAtLevelStart();
        FindObjectOfType<SliderController>().ResetEnergy();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void RestartLevel()
    {
        FindObjectOfType<SliderController>().ResetEnergy();
        FindObjectOfType<ScoreKeeper>().RollbackScore();
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
