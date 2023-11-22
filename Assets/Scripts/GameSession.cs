using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    bool tryAgain = false;
    bool saveQuit = false;
    int scoreAtLevelStart;
    [SerializeField] ScoreKeeper scoreKeeper;
    static GameSession instance;

    void Awake()
    {
        scoreAtLevelStart = GetPreviousScore();

        if (instance != null)
        {
            instance.GetScoreKeeper().SetScore(scoreAtLevelStart);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            scoreKeeper.SetScore(scoreAtLevelStart);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    int GetPreviousScore()
    {
        string gameDataPath = Application.persistentDataPath + "/CurrentGameData.json";
        string gameDataJSON = (File.Exists(gameDataPath)) ? File.ReadAllText(gameDataPath) : null;
        TypeDescriptor.AddAttributes(typeof((int, int)), new TypeConverterAttribute(typeof(TupleConverter<int, int>)));
        return (File.Exists(gameDataPath)) ? JsonConvert.DeserializeObject<CurrentGameData>(gameDataJSON).scoreAtLevelStart : 0;
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
        int stage = Int32.Parse(sceneName[l - 3].ToString());
        int level = Int32.Parse(sceneName[l - 1].ToString());
        return (stage, level);
    }

    public ScoreKeeper GetScoreKeeper()
    {
        return scoreKeeper;
    }
}
