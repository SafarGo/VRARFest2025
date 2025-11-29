using TMPro;
using UnityEngine;

public class PerekrestokLevelController : MonoBehaviour
{
    public static bool IsGameStarted = false;
    public static  bool IsGameEnded = false;

    public  float timeInGame = 0f;
    public TMP_Text scoreText;
    public TMP_Text recordText;
    public TMP_Text timeText;

    private void Update()
    {
        if (IsGameStarted && !IsGameEnded)
        {
            timeInGame += Time.deltaTime;
        }
        StatisticsController.SetTotalScore(Counter.GetScore(GameType.CarGame), GameType.CarGame);
        if (IsGameEnded)
        {
            if (StatisticsController.GetBestScore(GameType.CarGame) < Counter.GetScore(GameType.CarGame))
            {
                StatisticsController.SetBestScore(Counter.GetScore(GameType.CarGame), GameType.CarGame);
            }
            recordText.text = $"Record + {StatisticsController.GetBestScore(GameType.CarGame)}";
        }
        int minutes = Mathf.FloorToInt(timeInGame / 60f);

        int seconds = Mathf.FloorToInt(timeInGame % 60f);
        timeText.text = $"Time: {minutes.ToString("D2")}:{seconds.ToString("D2")}";
        scoreText.text = $"Score: {Counter.GetScore(GameType.CarGame)}";

    }
}
