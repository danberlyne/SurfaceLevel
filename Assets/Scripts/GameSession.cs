using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    bool tryAgain = false;
    bool saveQuit = false;
    int scoreAtLevelStart;
    ScoreKeeper scoreKeeper;
    static GameSession instance;

    void Awake()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            scoreAtLevelStart = FindObjectOfType<JSONReader>().GetCurrentScore();
        }
    }

    void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateScoreAtLevelStart()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        scoreAtLevelStart = scoreKeeper.GetScore();
    }

    public int GetScoreAtLevelStart()
    {
        return scoreAtLevelStart;
    }

    public void ResetInstance()
    {
        instance = null;
    }

    public (int, int) GetCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int l = sceneName.Length;
        int stage = (int)sceneName[l - 3];
        int level = (int)sceneName[l - 1];
        return (stage, level);
    }
}
