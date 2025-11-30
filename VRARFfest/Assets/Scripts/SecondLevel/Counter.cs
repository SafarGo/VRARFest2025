using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Counter : MonoBehaviour
{
    [Header("Настройки уровня")]
    [Tooltip("Тип игры для этого счетчика")]
    public GameType gameType;

    public static int castleScore = 0;
    public static int carScore = 0;

    public static int GetScore(GameType gameType)
    {
        return gameType == GameType.CastleDefense ? castleScore : carScore;
    }

    public static void AddScore(int bonus, GameType gameType)
    {
        if (gameType == GameType.CastleDefense)
        {
            castleScore += bonus;
        }
        else
        {
            carScore += bonus;
        }
    }

    public static void ResetScore(GameType gameType)
    {
        if (gameType == GameType.CastleDefense)
        {
            castleScore = 0;
        }
        else
        {
            carScore = 0;
        }
    }
}
