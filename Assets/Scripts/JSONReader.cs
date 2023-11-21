using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel;

public class JSONReader : MonoBehaviour
{
    string gameDataJSON, progressionDataJSON, scoreDataJSON;
    (int, int) currentLevel;
    int currentScore;
    (int, int) levelProgression;
    List<int> overallHighScores;
    Dictionary<(int, int), (int, string)> levelHighScores;
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

        // Allows proper deserialisation of tuples as dictionary keys.
        TypeDescriptor.AddAttributes(typeof((int, int)), new TypeConverterAttribute(typeof(TupleConverter<int, int>)));

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

    public (int, int) GetCurrentLevel() 
    { 
        return currentLevel; 
    }
    public int GetCurrentScore() 
    { 
        return currentScore; 
    }
    public (int, int) GetLevelProgression() 
    { 
        return levelProgression; 
    }
    public List<int> GetOverallHighScores() 
    { 
        return overallHighScores; 
    }
    public Dictionary<(int, int), (int, string)> GetLevelHighScores() 
    { 
        return levelHighScores; 
    }
}

