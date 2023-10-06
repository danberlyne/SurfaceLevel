using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] GameObject scoreAmount;
    int score;
    [SerializeField] GameObject levelHeader;
    int currentStage = 1;

    public void UpdateScore()
    {
        int destroyedThisShot = FindObjectOfType<ProjectileBehaviour>().GetDestroyedThisShot();
        // Perform a left-shift on 1 to give 2^destroyedThisShot.
        score += 1 << destroyedThisShot;
        TextMeshProUGUI scoreText = scoreAmount.GetComponent<TextMeshProUGUI>();
        scoreText.text = score.ToString();
    }

    public void UpdateLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        TextMeshProUGUI levelText = levelHeader.GetComponent<TextMeshProUGUI>();
        levelText.text = $"Level {currentStage}-{currentSceneIndex+1}";
    }

    public void UpdateStage()
    {
        currentStage += 1;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddBonus()
    {
        SliderController sliderController = FindObjectOfType<SliderController>();
        score += sliderController.GetRemainingEnergy();
    }

    public void ResetScore()
    {
        score = 0;
        TextMeshProUGUI scoreText = scoreAmount.GetComponent<TextMeshProUGUI>();
        scoreText.text = score.ToString();
    }

    public void RollbackScore()
    {
        score = FindObjectOfType<GameSession>().GetScoreAtLevelStart();
        TextMeshProUGUI scoreText = scoreAmount.GetComponent<TextMeshProUGUI>();
        scoreText.text = score.ToString();
    }

}
