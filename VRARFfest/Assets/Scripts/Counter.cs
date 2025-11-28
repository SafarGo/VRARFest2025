using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Counter : MonoBehaviour
{
    public static int score = 0;
    public TMP_Text text;

    public static int GetScore()
    {
        return score;
    }

    public static void AddScore(int bonus)
    {
        score += bonus;
    }

    private void Update()
    {
        text.text = score.ToString();
    }
}
