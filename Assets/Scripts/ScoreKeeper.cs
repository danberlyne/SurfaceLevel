using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] GameObject scoreAmount;
    int score;

    public void UpdateScore()
    {
        int destroyedThisShot = FindObjectOfType<ProjectileBehaviour>().GetDestroyedThisShot();
        // Perform a left-shift on 1 to give 2^destroyedThisShot.
        score += 1 << destroyedThisShot;
        TextMeshProUGUI scoreText = scoreAmount.GetComponent<TextMeshProUGUI>();
        scoreText.text = score.ToString();
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
