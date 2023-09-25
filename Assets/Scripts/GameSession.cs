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

    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            scoreAtLevelStart = 0;
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
}
