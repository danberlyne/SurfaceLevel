using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.ComponentModel;

public class SaveManager : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;
    CurrentGameData _GameData = new CurrentGameData();
    ProgressionData _ProgressionData = new ProgressionData();
    HighScoreData _ScoreData = new HighScoreData();
    JSONReader reader;
    CurrentGameData defaultGameData = new CurrentGameData();
    ProgressionData defaultProgressionData = new ProgressionData();
    HighScoreData defaultScoreData = new HighScoreData();
    [SerializeField] int firstLevelBuildIndex = 1;
    string gameDataJSON, progressionDataJSON, scoreDataJSON;
    string gameDataPath, progressionDataPath, scoreDataPath;

    void Awake()
    {
        reader = GetComponent<JSONReader>();
        gameDataPath = Application.persistentDataPath + "/CurrentGameData.json";
        progressionDataPath = Application.persistentDataPath + "/ProgressionData.json";
        scoreDataPath = Application.persistentDataPath + "/HighScoreData.json";
        InitialiseData();
    }

    void InitialiseData()
    {
        gameDataJSON = (File.Exists(gameDataPath)) ? File.ReadAllText(gameDataPath) : null;
        progressionDataJSON = (File.Exists(progressionDataPath)) ? File.ReadAllText(progressionDataPath) : null;
        scoreDataJSON = (File.Exists(scoreDataPath)) ? File.ReadAllText(scoreDataPath) : null;

        // Allows proper deserialisation of tuples as dictionary keys.
        TypeDescriptor.AddAttributes(typeof((int, int)), new TypeConverterAttribute(typeof(TupleConverter<int, int>)));

        _GameData = (File.Exists(gameDataPath)) ? JsonConvert.DeserializeObject<CurrentGameData>(gameDataJSON) : GetDefaultGameData();
        _ProgressionData = (File.Exists(progressionDataPath)) ? JsonConvert.DeserializeObject<ProgressionData>(progressionDataJSON) : GetDefaultProgressionData();
        _ScoreData = (File.Exists(scoreDataPath)) ? JsonConvert.DeserializeObject<HighScoreData>(scoreDataJSON) : GetDefaultScoreData();
    }

    public void SaveToJson()
    {
        string gameDataString = JsonConvert.SerializeObject(_GameData);
        string progressionDataString = JsonConvert.SerializeObject(_ProgressionData);
        string scoreDataString = JsonConvert.SerializeObject(_ScoreData);
        File.WriteAllText(Application.persistentDataPath + "/CurrentGameData.json", gameDataString);
        File.WriteAllText(Application.persistentDataPath + "/ProgressionData.json", progressionDataString);
        File.WriteAllText(Application.persistentDataPath + "/HighScoreData.json", scoreDataString);
    }

    public void SaveToJson(CurrentGameData gameData, ProgressionData progressionData, HighScoreData scoreData)
    {
        string gameDataString = JsonConvert.SerializeObject(gameData);
        string progressionDataString = JsonConvert.SerializeObject(progressionData);
        string scoreDataString = JsonConvert.SerializeObject(scoreData);
        File.WriteAllText(Application.persistentDataPath + "/CurrentGameData.json", gameDataString);
        File.WriteAllText(Application.persistentDataPath + "/ProgressionData.json", progressionDataString);
        File.WriteAllText(Application.persistentDataPath + "/HighScoreData.json", scoreDataString);
    }

    public void LoadCurrentGame()
    {
        StartCoroutine(LoadCurrentLevel());
    }

    IEnumerator LoadCurrentLevel()
    {
        yield return new WaitForSeconds(loadDelay);
        string currentLevel = $"Level{reader.GetCurrentLevel().Item1}-{reader.GetCurrentLevel().Item2}";
        SceneManager.LoadScene(currentLevel);
    }

    public void SetCurrentScore(int score)
    {
        _GameData.scoreAtLevelStart = score;
    }

    public void SetCurrentLevel((int, int) level)
    {
        _GameData.currentLevel = level;
    }

    public void UpdateOverallHighScores(int score)
    {
        List<int> highScores = reader.GetOverallHighScores();
        highScores.Add(score);
        highScores.Sort();
        highScores.RemoveAt(0);
        _ScoreData.overallHighScores = highScores;
    }

    public void UpdateLevelProgression((int, int) level)
    {
        if (level.Item1 > _ProgressionData.levelProgression.Item1)
        {
            _ProgressionData.levelProgression = level;
        }
        else if (level.Item1 == _ProgressionData.levelProgression.Item1 && level.Item2 > _ProgressionData.levelProgression.Item2)
        {
            _ProgressionData.levelProgression = level;
        }
    }

    public CurrentGameData GetDefaultGameData()
    {
        defaultGameData.currentLevel = (1, 1);
        defaultGameData.scoreAtLevelStart = 0;
        return defaultGameData;
    }

    public ProgressionData GetDefaultProgressionData()
    {
        defaultProgressionData.levelProgression = (1, 1);
        return defaultProgressionData;
    }

    public HighScoreData GetDefaultScoreData()
    {
        defaultScoreData.overallHighScores = new List<int>() { 0, 0, 0 };
        defaultScoreData.levelHighScores = new Dictionary<(int, int), (int, string)>();
        for (int i = firstLevelBuildIndex; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(pathToScene);
            int l = sceneName.Length;
            int stage = Int32.Parse(sceneName[l - 3].ToString());
            int level = Int32.Parse(sceneName[l - 1].ToString());
            defaultScoreData.levelHighScores.Add((stage, level), (0, "-"));
        }
        return defaultScoreData;
    }
}

/* 
Load the state at the beginning of the level the player saved on.
Need to know the stage and level to determine the scene to load.
Need to know the score the player had at the start of the level.
*/
[System.Serializable]
[JsonObject]
public class CurrentGameData
{
    public (int, int) currentLevel; 
    public int scoreAtLevelStart;
}

[System.Serializable]
[JsonObject]
public class ProgressionData
{
    public (int, int) levelProgression; // Highest stage and level completed.
}

[System.Serializable]
[JsonObject]
public class HighScoreData
{
    // List of top 3 overall scores.
    public List<int> overallHighScores = new List<int>();
    // Keys are stage-level tuples, values are score-rank tuples.
    public Dictionary<(int, int), (int, string)> levelHighScores = new Dictionary<(int, int), (int, string)>();
}