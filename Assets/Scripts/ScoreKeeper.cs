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
    [SerializeField] GameObject multihitAmount;
    [SerializeField] float multihitFlashDuration = 3f;
    int multihitBonus = 0;

    public void UpdateScore()
    {
        int destroyedThisShot = FindObjectOfType<ProjectileBehaviour>().GetDestroyedThisShot();
        // Perform a left-shift on 1 to give 2^destroyedThisShot.
        multihitBonus = 1 << destroyedThisShot;
        
        score += multihitBonus;

        if (destroyedThisShot > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FlashMultihitBonus(multihitBonus));
        }

        TextMeshProUGUI scoreText = scoreAmount.GetComponent<TextMeshProUGUI>();
        scoreText.text = score.ToString();
    }

    IEnumerator FlashMultihitBonus(int multihitBonus)
    {
        multihitAmount.SetActive(true);
        TextMeshProUGUI multihitText = multihitAmount.GetComponent<TextMeshProUGUI>();
        multihitText.text = "Multihit\nBonus x" + multihitBonus;
        yield return new WaitForSeconds(multihitFlashDuration);
        multihitAmount.SetActive(false);
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
