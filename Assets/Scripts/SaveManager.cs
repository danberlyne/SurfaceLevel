using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        reader = GetComponent<JSONReader>();
    }

    public void InitialiseDefaults()
    {
        defaultGameData.currentLevel = new Tuple<int, int>(1, 1);
        defaultGameData.scoreAtLevelStart = 0;
        defaultProgressionData.levelProgression = new Tuple<int, int>(1, 1);
        defaultScoreData.overallHighScores = new List<int>() { 0, 0, 0 };
        defaultScoreData.levelHighScores = new Dictionary<Tuple<int, int>, Tuple<int, string>>();
        for (int i = firstLevelBuildIndex; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(pathToScene);
            int l = sceneName.Length;
            int stage = (int) sceneName[l-3];
            int level = (int) sceneName[l-1];
            defaultScoreData.levelHighScores.Add(new Tuple<int, int>(stage, level), new Tuple<int, string>(0, "-"));
        }
    }

    public void SaveToJson()
    {
        string gameData = JsonUtility.ToJson(_GameData);
        string progressionData = JsonUtility.ToJson(_ProgressionData);
        string scoreData = JsonUtility.ToJson(_ScoreData);
        File.WriteAllText(Application.persistentDataPath + "/CurrentGameData.json", gameData);
        File.WriteAllText(Application.persistentDataPath + "/ProgressionData.json", progressionData);
        File.WriteAllText(Application.persistentDataPath + "/HighScoreData.json", scoreData);
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

    public void SetCurrentLevel(Tuple<int, int> level)
    {
        _GameData.currentLevel = level;
    }

    public void UpdateLevelProgression(Tuple<int, int> level)
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
        return defaultGameData;
    }

    public ProgressionData GetDefaultProgressionData()
    {
        return defaultProgressionData;
    }

    public HighScoreData GetDefaultScoreData()
    {
        return defaultScoreData;
    }
}

/* 
Load the state at the beginning of the level the player saved on.
Need to know the stage and level to determine the scene to load.
Need to know the score the player had at the start of the level.
*/
[System.Serializable]
public class CurrentGameData
{
    public Tuple<int, int> currentLevel; 
    public int scoreAtLevelStart;
}

[System.Serializable]
public class ProgressionData
{
    public Tuple<int, int> levelProgression; // Highest stage and level completed.
}

[System.Serializable]
public class HighScoreData
{
    // List of top 3 overall scores.
    public List<int> overallHighScores = new List<int>();
    // Keys are stage-level tuples, values are score-rank tuples.
    public Dictionary<Tuple<int, int>, Tuple<int, string>> levelHighScores = new Dictionary<Tuple<int, int>, Tuple<int, string>>();
}