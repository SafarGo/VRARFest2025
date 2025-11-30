using UnityEngine;
using TMPro;

public class ScoreTrigger : MonoBehaviour
{
    public AudioClip scoreSound;
    public ParticleSystem scoreVFX;
    public AudioSource audioSource;

    private int score = 0;
    private int highScore = 0;

    public TMP_Text score_text;
    public TMP_Text highScore_text;

    private const string HighScoreKey = "BasketHighScore";

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        score_text.text = "ћ€чей заброшено: " + score;
        UpdateHighScoreText();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
        {
            score++;
            score_text.text = "ћ€чей заброшено: " + score;
            CheckForNewHighScore();

            if (audioSource != null && scoreSound != null) audioSource.PlayOneShot(scoreSound);
            if (scoreVFX != null) scoreVFX.Play();
        }
    }

    void CheckForNewHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }
    }

    void UpdateHighScoreText()
    {
        if (highScore_text != null)
        {
            highScore_text.text = "–екорд: " + highScore;
        }
    }
}