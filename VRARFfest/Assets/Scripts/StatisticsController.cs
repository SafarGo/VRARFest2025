using UnityEngine;



public enum GameType

{

    CastleDefense,

    CarGame

}



public class StatisticsController : MonoBehaviour

{

    private const string CASTLE_TOTAL_SCORE_KEY = "Castle_TotalScore";

    private const string CASTLE_BEST_SCORE_KEY = "Castle_BestScore";

    private const string CASTLE_BEST_TIME_KEY = "Castle_BestTime";



    private const string CAR_TOTAL_SCORE_KEY = "Car_TotalScore";

    private const string CAR_BEST_SCORE_KEY = "Car_BestScore";

    private const string CAR_BEST_TIME_KEY = "Car_BestTime";



    public static float GetTotalScore(GameType gameType)

    {

        string key = gameType == GameType.CastleDefense ? CASTLE_TOTAL_SCORE_KEY : CAR_TOTAL_SCORE_KEY;

        return PlayerPrefs.GetFloat(key, 0);

    }



    public static void SetTotalScore(float value, GameType gameType)

    {

        string key = gameType == GameType.CastleDefense ? CASTLE_TOTAL_SCORE_KEY : CAR_TOTAL_SCORE_KEY;

        PlayerPrefs.SetFloat(key, value);

        PlayerPrefs.Save();

    }

    public static float GetBestScore(GameType gameType)

    {

        string key = gameType == GameType.CastleDefense ? CASTLE_BEST_SCORE_KEY : CAR_BEST_SCORE_KEY;

        return PlayerPrefs.GetFloat(key, 0);

    }



    public static void SetBestScore(float value, GameType gameType)

    {

        string key = gameType == GameType.CastleDefense ? CASTLE_BEST_SCORE_KEY : CAR_BEST_SCORE_KEY;

        PlayerPrefs.SetFloat(key, value);

        PlayerPrefs.Save();

    }



    public static float GetBestTime(GameType gameType)

    {

        string key = gameType == GameType.CastleDefense ? CASTLE_BEST_TIME_KEY : CAR_BEST_TIME_KEY;

        return PlayerPrefs.GetFloat(key, 0);

    }



    public static void SetBestTime(float value, GameType gameType)

    {

        string key = gameType == GameType.CastleDefense ? CASTLE_BEST_TIME_KEY : CAR_BEST_TIME_KEY;

        PlayerPrefs.SetFloat(key, value);

        PlayerPrefs.Save();

    }

}