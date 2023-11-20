using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class JSONReader : MonoBehaviour
{
    string gameDataJSON, progressionDataJSON, scoreDataJSON;
    Tuple<int, int> currentLevel;
    int currentScore;
    Tuple<int, int> levelProgression;
    List<int> overallHighScores;
    Dictionary<Tuple<int, int>, Tuple<int, string>> levelHighScores;
    SaveManager saveManager;

    void Awake()
    {
        saveManager = GetComponent<SaveManager>();

        string gameDataPath = Application.persistentDataPath + "/CurrentGameData.json";
        string progressionDataPath = Application.persistentDataPath + "/ProgressionData.json";
        string scoreDataPath = Application.persistentDataPath + "/HighScoreData.json";

        gameDataJSON = (File.Exists(gameDataPath)) ? File.ReadAllText(gameDataPath) : null;
        progressionDataJSON = (File.Exists(progressionDataPath)) ? File.ReadAllText(progressionDataPath) : null;
        scoreDataJSON = (File.Exists(scoreDataPath)) ? File.ReadAllText(scoreDataPath) : null;

        CurrentGameData gameDataInJson = (File.Exists(gameDataPath)) ? JsonConvert.DeserializeObject<CurrentGameData>(gameDataJSON) : saveManager.GetDefaultGameData();
        ProgressionData progressionDataInJson = (File.Exists(progressionDataPath)) ? JsonConvert.DeserializeObject<ProgressionData>(progressionDataJSON) : saveManager.GetDefaultProgressionData();
        HighScoreData scoreDataInJson = (File.Exists(scoreDataPath)) ? JsonConvert.DeserializeObject<HighScoreData>(scoreDataJSON) : saveManager.GetDefaultScoreData();

        currentLevel = gameDataInJson.currentLevel;
        currentScore = gameDataInJson.scoreAtLevelStart;

        levelProgression = progressionDataInJson.levelProgression;

        overallHighScores = scoreDataInJson.overallHighScores;
        levelHighScores = scoreDataInJson.levelHighScores;

        saveManager.SaveToJson(gameDataInJson, progressionDataInJson, scoreDataInJson);
    }

    public Tuple<int, int> GetCurrentLevel() 
    { 
        return currentLevel; 
    }
    public int GetCurrentScore() 
    { 
        return currentScore; 
    }
    public Tuple<int, int> GetLevelProgression() 
    { 
        return levelProgression; 
    }
    public List<int> GetOverallHighScores() 
    { 
        return overallHighScores; 
    }
    public Dictionary<Tuple<int, int>, Tuple<int, string>> GetLevelHighScores() 
    { 
        return levelHighScores; 
    }
}

