using TMPro;
using UnityEngine;

public class PerekrestokLevelController : MonoBehaviour
{
    public static bool IsGameStarted = false;
    public static  bool IsGameEnded = false;

    public static  float timeInGame = 0f;
    public TMP_Text scoreText;
    public TMP_Text recordText;
    public TMP_Text timeText;
    public TMP_Text goalText;
    public TMP_Text state_text;

    private void Start()
    {
        recordText.text = $"Рекорд {StatisticsController.GetBestScore(GameType.CarGame)}";
        goalText.text = $"В следующей игре попробуй набрать {Mathf.Round(StatisticsController.GetBestScore(GameType.CarGame) * 1.1f)} очков";
        state_text.text = "Играешь";
        timeInGame = 0;
    }
    private void Update()
    {
        if (IsGameStarted && !IsGameEnded)
        {
            timeInGame += Time.deltaTime;
        }
        if (IsGameStarted)
        {
            state_text.text = "Играешь";
        }
        StatisticsController.SetTotalScore(Counter.GetScore(GameType.CarGame), GameType.CarGame);
        if (IsGameEnded)
        {
            state_text.text = "Проиграл";
            if (StatisticsController.GetBestScore(GameType.CarGame) < Counter.GetScore(GameType.CarGame))
            {
                StatisticsController.SetBestScore(Counter.GetScore(GameType.CarGame), GameType.CarGame);
            }
            goalText.text = $"В следующей игре попробуй набрать {Mathf.Round(StatisticsController.GetBestScore(GameType.CarGame) * 1.1f)} очков";
        }

        int minutes = Mathf.FloorToInt(timeInGame / 60f);

        int seconds = Mathf.FloorToInt(timeInGame % 60f);
        timeText.text = $"Время: {minutes.ToString("D2")}:{seconds.ToString("D2")}";
        scoreText.text = $"Счет: {Counter.GetScore(GameType.CarGame)}";
        recordText.text = $"Рекорд {StatisticsController.GetBestScore(GameType.CarGame)}";
    }
}
