using UnityEngine;
using System;
using TMPro;

public class StatisticsController : MonoBehaviour
{
    public static float Score;
    public static float timeInGame;
    public static TMP_Text text;

    private const string TOTAL_SCORE_KEY = "TotalScore";
    private const string TOTAL_TIME_KEY = "TotalTimeInGame";

    void Start()
    {
        LoadStatistics();
    }

    public static float GetTotalScore()
    {
        return PlayerPrefs.GetFloat(TOTAL_SCORE_KEY, 0);
    }

    public static float GetTotalTimeInGame()
    {
        return PlayerPrefs.GetFloat(TOTAL_TIME_KEY, 0);
    }


    public static void SetTotalScore(float value)
    {
        PlayerPrefs.SetFloat(TOTAL_SCORE_KEY, value);
        PlayerPrefs.Save();
    }

    public static void SetTotalTimeInGame(float value)
    {
        PlayerPrefs.SetFloat(TOTAL_TIME_KEY, value);
        PlayerPrefs.Save();
    }

    public static void AddTotalScore(float value)
    {
        float current = GetTotalScore();
        SetTotalScore(current + value);
    }

    public static void AddTotalTimeInGame(float value)
    {
        float current = GetTotalTimeInGame();
        SetTotalTimeInGame(current + value);
    }

    public static void SaveCurrentStatistics()
    {
        AddTotalScore(Score);
        AddTotalTimeInGame(timeInGame);
        
        Score = 0;
        timeInGame = 0;
    }

    private static void LoadStatistics()
    {
    }

    void OnApplicationQuit()
    {
        SaveCurrentStatistics();
    }
}
